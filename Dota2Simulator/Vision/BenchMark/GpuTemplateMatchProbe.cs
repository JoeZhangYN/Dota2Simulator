using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Dota2Simulator.Vision.Benchmark
{
    /// <summary>
    /// 旁路 PoC：GPU compute shader 模板匹配 vs CPU findpoints.dll，端到端性能验证。
    ///
    /// 不接入任何主流程。用嵌入资源里的真实模板图 + 代码合成的大图（把模板贴到已知坐标），
    /// 可脱离游戏 / 脱离实时截屏 headless 跑。入口：<see cref="RunProbe"/>。
    ///
    /// 技术栈：D3D11 DirectCompute，HLSL 内联为字符串、运行时 ShaderBytecode.Compile 编译，
    /// 免去 csproj 的 shader 构建配置。
    /// </summary>
    public static class GpuTemplateMatchProbe
    {
        // ── 测试参数 ──────────────────────────────────────────────────────────
        private const int MainW = 1920;
        private const int MainH = 1080;

        // 模板贴入大图的已知坐标（正确性校验的期望命中位置）
        private const int PlantX = 1136;
        private const int PlantY = 943;

        // findpoints 默认 ignore color (R,G,B) = (255,20,147)（粉色，洋红）
        private const byte IgnoreR = 255, IgnoreG = 20, IgnoreB = 147;

        // 大图背景填充色（与 ignore color 不同，避免误命中）
        private const byte BgR = 17, BgG = 17, BgB = 17;

        private const double MatchRate = 0.90;
        private const byte Tolerance = 12;     // 对应 ImageFinder.FindImageWithTolerance

        private const int Warmup = 10;
        private const int Iterations = 100;

        // ── HLSL：模板匹配 compute shader ─────────────────────────────────────
        // 每个线程负责大图中一个候选左上角 (gx, gy)。遍历模板像素，跳过 ignore color，
        // 比较大图对应像素 8-bit 通道差是否都 <= tolerance，统计匹配像素比例。
        // 比例 >= matchRate 则把坐标写入 UAV 结果 buffer。
        //
        // 注意：HLSL 源码内必须全 ASCII —— D3DCompile 的 lexer 遇到非 ASCII 字节
        //       （中文注释）会报 X3000 unexpected end of file。故 shader 内注释用英文。
        private const string HlslSource = @"
// MainTex t0 = big image BGRA;  TplTex t1 = template BGRA
// Result u0 int[4]: [0]=found(0/1) [1]=x [2]=y [3]=hitCount
Texture2D<float4>           MainTex  : register(t0);
Texture2D<float4>           TplTex   : register(t1);
RWStructuredBuffer<int>     Result   : register(u0);

cbuffer Params : register(b0)
{
    int   MainWidth;
    int   MainHeight;
    int   TplWidth;
    int   TplHeight;
    float Tolerance;     // normalized 0..1
    float MatchRate;     // required matched-pixel ratio
    float IgnoreR;       // normalized 0..1 ignore color
    float IgnoreG;
    float IgnoreB;
    float _pad0;
    float _pad1;
    float _pad2;
};

[numthreads(16, 16, 1)]
void CSMain(uint3 dtid : SV_DispatchThreadID)
{
    int gx = (int)dtid.x;
    int gy = (int)dtid.y;

    // candidate top-left must fully contain the template
    if (gx > MainWidth - TplWidth || gy > MainHeight - TplHeight)
        return;

    int considered = 0;   // non-ignore template pixels
    int matched    = 0;   // pixels matched within tolerance

    for (int ty = 0; ty < TplHeight; ty++)
    {
        for (int tx = 0; tx < TplWidth; tx++)
        {
            float4 tp = TplTex.Load(int3(tx, ty, 0));

            // skip ignore color (mask / transparent pixels)
            if (abs(tp.r - IgnoreR) <= 0.01 &&
                abs(tp.g - IgnoreG) <= 0.01 &&
                abs(tp.b - IgnoreB) <= 0.01)
                continue;

            considered++;

            float4 mp = MainTex.Load(int3(gx + tx, gy + ty, 0));

            if (abs(mp.r - tp.r) <= Tolerance &&
                abs(mp.g - tp.g) <= Tolerance &&
                abs(mp.b - tp.b) <= Tolerance)
                matched++;
        }
    }

    if (considered == 0)
        return;

    float rate = (float)matched / (float)considered;
    if (rate >= MatchRate)
    {
        // Encode scan order (y*W + x); atomically keep the smallest.
        // C# side decodes x = scan % W, y = scan / W. Race-free:
        // InterlockedMin alone is deterministic, no coord publish needed.
        int scan = gy * MainWidth + gx;
        InterlockedMin(Result[0], scan);
        InterlockedAdd(Result[3], 1);
    }
}
";

        // cbuffer 参数布局（与 HLSL Params 对齐，16 字节对齐 → 48 字节）
        [StructLayout(LayoutKind.Sequential)]
        private struct ShaderParams
        {
            public int MainWidth;
            public int MainHeight;
            public int TplWidth;
            public int TplHeight;
            public float Tolerance;
            public float MatchRate;
            public float IgnoreR;
            public float IgnoreG;
            public float IgnoreB;
            public float Pad0;
            public float Pad1;
            public float Pad2;
        }

        // ── 主入口 ────────────────────────────────────────────────────────────
        public static string RunProbe()
        {
            var sb = new StringBuilder();
            sb.AppendLine("==================================================================");
            sb.AppendLine(" GPU Compute Shader 模板匹配 vs CPU findpoints —— 技术验证 PoC");
            sb.AppendLine("==================================================================");
            sb.AppendLine();

            // ── 1) 加载真实模板图（嵌入资源） ──
            ImageHandle tplHandle;
            byte[] tplBgra;
            int tplW, tplH;
            try
            {
                tplHandle = LazyImageLoader.GetImage("物品.以太");
                if (!tplHandle.IsValid)
                {
                    sb.AppendLine("[FAIL] 无法加载嵌入模板资源 '物品.以太'。");
                    return sb.ToString();
                }
                tplW = tplHandle.Size.Width;
                tplH = tplHandle.Size.Height;
                tplBgra = ReadHandleBytes(tplHandle);
                sb.AppendLine($"模板图: 物品.以太  ({tplW}x{tplH}, {tplBgra.Length / 1024.0:F1} KB BGRA)");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"[FAIL] 加载模板异常: {ex.Message}");
                return sb.ToString();
            }

            // ── 2) 合成大图：背景填充 + 模板贴到 (PlantX, PlantY) ──
            byte[] mainBgra = SynthesizeMainImage(tplBgra, tplW, tplH, out var plantX, out var plantY);
            sb.AppendLine($"大图  : 合成 {MainW}x{MainH}  (背景 RGB({BgR},{BgG},{BgB}), 模板贴入坐标 ({plantX},{plantY}))");
            sb.AppendLine($"匹配  : matchRate={MatchRate}, tolerance={Tolerance}, ignoreColor=RGB({IgnoreR},{IgnoreG},{IgnoreB})");
            sb.AppendLine();

            // 大图作为 ImageManager 动态图（供 CPU 路径 findpoints 使用）
            ImageHandle mainHandle = ImageManager.CreateDynamicImage(mainBgra, new Size(MainW, MainH), "PoC_合成大图");

            // ── 3) 创建 GPU 上下文（device + shader + 模板预驻显存） ──
            GpuProbeContext gpu = null;
            string deviceKind = "未创建";
            string gpuError = null;
            try
            {
                gpu = GpuProbeContext.Create(out deviceKind);
                gpu.UploadTemplate(tplBgra, tplW, tplH);  // 模板一次上传后复用（模拟预驻显存热加载）
                gpu.UploadMain(mainBgra, MainW, MainH);   // 大图初次上传
            }
            catch (Exception ex)
            {
                gpuError = ex.ToString();
            }

            sb.AppendLine($"GPU device: {deviceKind}");
            if (deviceKind.StartsWith("Warp"))
            {
                sb.AppendLine("  [注意] Warp 是 D3D11 的 CPU 软件光栅器 —— 此处「GPU」实为 CPU 模拟，");
                sb.AppendLine("         加速比不代表真实硬件。需在带独显/核显的机器复测。");
            }
            sb.AppendLine();
            if (gpuError != null)
            {
                sb.AppendLine("[FAIL] GPU 上下文创建失败：");
                sb.AppendLine(gpuError);
                sb.AppendLine("==================================================================");
                try { ImageManager.ReleaseImage(mainHandle); } catch { }
                return sb.ToString();
            }

            // ── 4) 场景 A：目标存在（CPU findpoints 命中后早退）──────────────────
            //     这是当前主循环最常见的情形 —— 图标在屏幕上，CPU 扫到即停。
            RunScenario(sb, "场景 A · 目标存在（CPU 命中早退）",
                tplHandle, mainHandle, gpu,
                tplW, tplH, plantX, plantY,
                expectHit: true, altMainBgra: mainBgra);

            // ── 5) 场景 B：目标不存在（CPU findpoints 必须全图扫描，无早退）────────
            //     这是 GPU 全并行最有利、CPU 最不利的情形 —— 公平体现「全扫描吞吐」。
            //     用一张纯背景大图（不贴模板），模板必然找不到。
            byte[] emptyBgra = SynthesizeEmptyImage();
            ImageHandle emptyHandle = ImageManager.CreateDynamicImage(
                emptyBgra, new Size(MainW, MainH), "PoC_空白大图");
            RunScenario(sb, "场景 B · 目标不存在（CPU 全图扫描无早退）",
                tplHandle, emptyHandle, gpu,
                tplW, tplH, -1, -1,
                expectHit: false, altMainBgra: emptyBgra);

            gpu?.Dispose();

            // ── 6) 结论 ──
            sb.AppendLine("── 结论速览 ─────────────────────────────────────────────────");
            sb.AppendLine("  · 单次小模板匹配：CPU findpoints 已是亚毫秒级，GPU 单 Dispatch 的");
            sb.AppendLine("    固定 round-trip 开销（cbuffer 更新 + Dispatch + 回读 Map 同步）");
            sb.AppendLine("    约 1ms，构成硬下限 —— 单图标匹配 GPU 难有优势。");
            sb.AppendLine("  · GPU 真正的收益点在「一次 Dispatch 批量匹配多模板」+「帧免回传」，");
            sb.AppendLine("    把固定开销摊薄到几十次匹配上。详见返回报告的可行性分析。");
            sb.AppendLine("==================================================================");

            try { ImageManager.ReleaseImage(mainHandle); } catch { }
            try { ImageManager.ReleaseImage(emptyHandle); } catch { }

            return sb.ToString();
        }

        /// <summary>
        /// 跑一个场景：CPU findpoints vs GPU(帧已在显存) vs GPU(含上传)，并做正确性校验。
        /// </summary>
        private static void RunScenario(
            StringBuilder sb, string title,
            ImageHandle tplHandle, ImageHandle mainHandle, GpuProbeContext gpu,
            int tplW, int tplH, int expectX, int expectY,
            bool expectHit, byte[] altMainBgra = null)
        {
            sb.AppendLine("##################################################################");
            sb.AppendLine($" {title}");
            sb.AppendLine("##################################################################");
            var ignoreColor = Color.FromArgb(IgnoreR, IgnoreG, IgnoreB);

            // CPU 路径
            Point? cpuHit = null;
            PerformanceResult cpuResult = null;
            bool cpuOk = false;
            try
            {
                cpuHit = ImageFinder.FindImageWithTolerance(
                    tplHandle, mainHandle, Tolerance, MatchRate, ignoreColor);
                var runner = new PerformanceTestRunner("CPU findpoints.dll")
                {
                    WarmupIterations = Warmup,
                    TestIterations = Iterations
                };
                cpuResult = runner.Run(() =>
                {
                    _ = ImageFinder.FindImageWithTolerance(
                        tplHandle, mainHandle, Tolerance, MatchRate, ignoreColor);
                });
                cpuOk = true;
            }
            catch (DllNotFoundException)
            {
                sb.AppendLine("[WARN] findpoints.dll 未找到，CPU 路径跳过。");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"[WARN] CPU 路径异常: {ex.Message}");
            }

            // GPU 路径：本场景的大图重新上传一次（场景 B 用 altMainBgra）
            Point? gpuHit = null;
            PerformanceResult gpuNoUpload = null, gpuWithUpload = null;
            string gpuErr = null;
            try
            {
                byte[] mainBytes = altMainBgra; // 本场景大图原始字节
                // 把本场景大图刷进 GPU 纹理
                gpu.UploadMain(mainBytes, MainW, MainH);

                gpuHit = gpu.Dispatch(MainW, MainH, tplW, tplH);

                var noUpRunner = new PerformanceTestRunner("GPU compute (帧已在显存)")
                {
                    WarmupIterations = Warmup,
                    TestIterations = Iterations
                };
                gpuNoUpload = noUpRunner.Run(() =>
                {
                    _ = gpu.Dispatch(MainW, MainH, tplW, tplH);
                });

                if (mainBytes != null)
                {
                    var upRunner = new PerformanceTestRunner("GPU compute (含大图上传)")
                    {
                        WarmupIterations = Warmup,
                        TestIterations = Iterations
                    };
                    gpuWithUpload = upRunner.Run(() =>
                    {
                        gpu.UploadMain(mainBytes, MainW, MainH);
                        _ = gpu.Dispatch(MainW, MainH, tplW, tplH);
                    });
                }
            }
            catch (Exception ex)
            {
                gpuErr = ex.ToString();
            }

            if (gpuErr != null)
            {
                sb.AppendLine("[FAIL] GPU 路径异常：");
                sb.AppendLine(gpuErr);
            }

            // 正确性校验
            sb.AppendLine("── 正确性校验 ───────────────────────────────────────────────");
            if (expectHit)
                sb.AppendLine($"  期望命中坐标 : ({expectX}, {expectY})");
            else
                sb.AppendLine("  期望         : 不命中 (null)");
            sb.AppendLine($"  CPU findpoints: {FmtPoint(cpuHit)}");
            sb.AppendLine($"  GPU compute  : {FmtPoint(gpuHit)}");
            bool consistent;
            if (cpuOk)
            {
                consistent = (cpuHit.HasValue == gpuHit.HasValue)
                    && (!cpuHit.HasValue ||
                        (Math.Abs(cpuHit.Value.X - gpuHit.Value.X) <= 1
                         && Math.Abs(cpuHit.Value.Y - gpuHit.Value.Y) <= 1));
                sb.AppendLine($"  GPU 与 CPU 结果一致: {(consistent ? "是 OK" : "否 FAIL")}");
            }
            else
            {
                sb.AppendLine("  GPU 与 CPU 结果一致: 无法对比（CPU 路径不可用）");
            }
            bool gpuExpected = expectHit
                ? (gpuHit.HasValue
                   && Math.Abs(gpuHit.Value.X - expectX) <= 1
                   && Math.Abs(gpuHit.Value.Y - expectY) <= 1)
                : !gpuHit.HasValue;
            sb.AppendLine($"  GPU 结果符合预期: {(gpuExpected ? "是 OK" : "否 FAIL")}");
            sb.AppendLine();

            // 性能对比
            var comparator = new PerformanceComparator();
            if (cpuResult != null) comparator.AddResult(cpuResult);
            if (gpuNoUpload != null) comparator.AddResult(gpuNoUpload);
            if (gpuWithUpload != null) comparator.AddResult(gpuWithUpload);
            sb.AppendLine(comparator.GetComparisonReport());

            sb.AppendLine("── 加速比小结 ───────────────────────────────────────────────");
            if (cpuResult != null && gpuNoUpload != null)
                sb.AppendLine($"  GPU(帧已在显存) 相对 CPU findpoints : " +
                    $"{cpuResult.AverageTimeMs / gpuNoUpload.AverageTimeMs:F2}x");
            if (cpuResult != null && gpuWithUpload != null)
                sb.AppendLine($"  GPU(含大图上传) 相对 CPU findpoints : " +
                    $"{cpuResult.AverageTimeMs / gpuWithUpload.AverageTimeMs:F2}x");
            if (gpuWithUpload != null && gpuNoUpload != null)
                sb.AppendLine($"  大图 CPU->GPU 上传单帧开销估算       : " +
                    $"{gpuWithUpload.AverageTimeMs - gpuNoUpload.AverageTimeMs:F2}ms");
            sb.AppendLine();
        }

        // ── 辅助：从 ImageHandle 读出 BGRA byte[] ─────────────────────────────
        private static unsafe byte[] ReadHandleBytes(in ImageHandle handle)
        {
            var (ptr, length, _) = ImageManager.GetImageData(in handle);
            var data = new byte[length];
            Marshal.Copy(ptr, data, 0, length);
            GC.KeepAlive(handle);
            return data;
        }

        // ── 辅助：合成大图（背景 + 模板贴入已知坐标） ─────────────────────────
        private static byte[] SynthesizeMainImage(
            byte[] tplBgra, int tplW, int tplH, out int plantX, out int plantY)
        {
            plantX = Math.Min(PlantX, MainW - tplW);
            plantY = Math.Min(PlantY, MainH - tplH);

            var main = new byte[MainW * MainH * 4];
            // 背景填充（BGRA）
            for (int i = 0; i < main.Length; i += 4)
            {
                main[i + 0] = BgB;
                main[i + 1] = BgG;
                main[i + 2] = BgR;
                main[i + 3] = 255;
            }
            // 贴入模板：ignore color 像素不贴（保留背景），模拟掩码透明区
            for (int ty = 0; ty < tplH; ty++)
            {
                for (int tx = 0; tx < tplW; tx++)
                {
                    int si = (ty * tplW + tx) * 4;
                    byte b = tplBgra[si + 0], g = tplBgra[si + 1], r = tplBgra[si + 2];
                    if (r == IgnoreR && g == IgnoreG && b == IgnoreB)
                        continue;
                    int di = ((plantY + ty) * MainW + (plantX + tx)) * 4;
                    main[di + 0] = b;
                    main[di + 1] = g;
                    main[di + 2] = r;
                    main[di + 3] = 255;
                }
            }
            return main;
        }

        // ── 辅助：纯背景大图（不贴模板，模板必然找不到，CPU 被迫全图扫描） ──────
        private static byte[] SynthesizeEmptyImage()
        {
            var main = new byte[MainW * MainH * 4];
            for (int i = 0; i < main.Length; i += 4)
            {
                main[i + 0] = BgB;
                main[i + 1] = BgG;
                main[i + 2] = BgR;
                main[i + 3] = 255;
            }
            return main;
        }

        private static string FmtPoint(Point? p)
            => p.HasValue ? $"({p.Value.X}, {p.Value.Y})" : "未命中 (null)";

        // ── GPU 上下文：device + shader + 纹理 + UAV 封装 ──────────────────────
        private sealed class GpuProbeContext : IDisposable
        {
            private Device _device;
            private DeviceContext _ctx;
            private ComputeShader _shader;

            private Texture2D _mainTex;
            private ShaderResourceView _mainSrv;
            private int _mainW, _mainH;

            private Texture2D _tplTex;
            private ShaderResourceView _tplSrv;
            private int _tplW, _tplH;

            private Buffer _resultBuffer;        // RWStructuredBuffer<int>，4 个 int
            private UnorderedAccessView _resultUav;
            private Buffer _resultStaging;       // CPU 回读用
            private Buffer _cbuffer;             // cbuffer Params

            private const int ResultIntCount = 4;

            public static GpuProbeContext Create(out string deviceKind)
            {
                var c = new GpuProbeContext();
                Device dev = null;
                deviceKind = "未创建";

                // 优先 Hardware，失败 fallback Warp
                try
                {
                    dev = new Device(DriverType.Hardware, DeviceCreationFlags.None,
                        FeatureLevel.Level_11_1, FeatureLevel.Level_11_0);
                    deviceKind = $"Hardware (FeatureLevel {dev.FeatureLevel})";
                }
                catch
                {
                    dev?.Dispose();
                    dev = null;
                }
                if (dev == null)
                {
                    try
                    {
                        dev = new Device(DriverType.Warp, DeviceCreationFlags.None,
                            FeatureLevel.Level_11_1, FeatureLevel.Level_11_0);
                        deviceKind = $"Warp 软件光栅器 (FeatureLevel {dev.FeatureLevel})";
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            "D3D11 device 创建失败（Hardware 与 Warp 均不可用）", ex);
                    }
                }

                c._device = dev;
                c._ctx = dev.ImmediateContext;

                // 运行时编译 HLSL compute shader
                using (var bytecode = ShaderBytecode.Compile(
                    HlslSource, "CSMain", "cs_5_0", ShaderFlags.OptimizationLevel3))
                {
                    if (bytecode.Bytecode == null)
                        throw new InvalidOperationException(
                            "HLSL 编译失败: " + bytecode.Message);
                    c._shader = new ComputeShader(dev, bytecode.Bytecode);
                }

                // 结果 buffer（structured，4 个 int）
                c._resultBuffer = new Buffer(dev, new BufferDescription
                {
                    SizeInBytes = ResultIntCount * sizeof(int),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.UnorderedAccess,
                    OptionFlags = ResourceOptionFlags.BufferStructured,
                    StructureByteStride = sizeof(int),
                    CpuAccessFlags = CpuAccessFlags.None
                });
                c._resultUav = new UnorderedAccessView(dev, c._resultBuffer,
                    new UnorderedAccessViewDescription
                    {
                        Format = Format.Unknown,
                        Dimension = UnorderedAccessViewDimension.Buffer,
                        Buffer = new UnorderedAccessViewDescription.BufferResource
                        {
                            FirstElement = 0,
                            ElementCount = ResultIntCount,
                            Flags = UnorderedAccessViewBufferFlags.None
                        }
                    });
                c._resultStaging = new Buffer(dev, new BufferDescription
                {
                    SizeInBytes = ResultIntCount * sizeof(int),
                    Usage = ResourceUsage.Staging,
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Read,
                    OptionFlags = ResourceOptionFlags.None
                });

                // cbuffer
                c._cbuffer = new Buffer(dev, new BufferDescription
                {
                    SizeInBytes = Marshal.SizeOf<ShaderParams>(),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.None
                });

                return c;
            }

            /// <summary>模板纹理一次上传，后续 Dispatch 复用（模拟预驻显存热加载）。</summary>
            public void UploadTemplate(byte[] bgra, int w, int h)
            {
                _tplSrv?.Dispose();
                _tplTex?.Dispose();
                (_tplTex, _tplSrv) = CreateTextureFromBgra(bgra, w, h);
                _tplW = w; _tplH = h;
            }

            /// <summary>大图上传 GPU。含上传档每帧调用一次，不含上传档仅初次调用。</summary>
            public unsafe void UploadMain(byte[] bgra, int w, int h)
            {
                if (_mainTex != null && _mainW == w && _mainH == h)
                {
                    // 复用已有纹理，仅刷新像素（UpdateSubresource）
                    fixed (byte* p = bgra)
                    {
                        _ctx.UpdateSubresource(
                            new DataBox((IntPtr)p, w * 4, w * h * 4),
                            _mainTex, 0);
                    }
                    return;
                }
                _mainSrv?.Dispose();
                _mainTex?.Dispose();
                (_mainTex, _mainSrv) = CreateTextureFromBgra(bgra, w, h);
                _mainW = w; _mainH = h;
            }

            private unsafe (Texture2D, ShaderResourceView) CreateTextureFromBgra(
                byte[] bgra, int w, int h)
            {
                Texture2D tex;
                fixed (byte* p = bgra)
                {
                    tex = new Texture2D(_device, new Texture2DDescription
                    {
                        Width = w,
                        Height = h,
                        MipLevels = 1,
                        ArraySize = 1,
                        Format = Format.B8G8R8A8_UNorm,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = ResourceUsage.Default,
                        BindFlags = BindFlags.ShaderResource,
                        CpuAccessFlags = CpuAccessFlags.None,
                        OptionFlags = ResourceOptionFlags.None
                    }, new DataRectangle((IntPtr)p, w * 4));
                }
                var srv = new ShaderResourceView(_device, tex);
                return (tex, srv);
            }

            /// <summary>执行一次 GPU 模板匹配，回读命中坐标。</summary>
            public Point? Dispatch(int mainW, int mainH, int tplW, int tplH)
            {
                // 清零结果 buffer：用一个 0 值 staging→default 拷贝
                ClearResult();

                // 更新 cbuffer
                var prm = new ShaderParams
                {
                    MainWidth = mainW,
                    MainHeight = mainH,
                    TplWidth = tplW,
                    TplHeight = tplH,
                    Tolerance = Tolerance / 255f,
                    MatchRate = (float)MatchRate,
                    IgnoreR = IgnoreR / 255f,
                    IgnoreG = IgnoreG / 255f,
                    IgnoreB = IgnoreB / 255f
                };
                _ctx.UpdateSubresource(ref prm, _cbuffer);

                // 绑定并 Dispatch
                _ctx.ComputeShader.Set(_shader);
                _ctx.ComputeShader.SetShaderResource(0, _mainSrv);
                _ctx.ComputeShader.SetShaderResource(1, _tplSrv);
                _ctx.ComputeShader.SetUnorderedAccessView(0, _resultUav);
                _ctx.ComputeShader.SetConstantBuffer(0, _cbuffer);

                int groupsX = (mainW + 15) / 16;
                int groupsY = (mainH + 15) / 16;
                _ctx.Dispatch(groupsX, groupsY, 1);

                // 解绑 UAV（否则后续无法以 staging 拷贝读取）
                _ctx.ComputeShader.SetUnorderedAccessView(0, null);

                // 回读结果（仅 4 个 int）
                // r[0] = 最小 scan index（y*W+x），r[3] = 命中计数
                _ctx.CopyResource(_resultBuffer, _resultStaging);
                var box = _ctx.MapSubresource(_resultStaging, 0,
                    MapMode.Read, MapFlags.None);
                int minScan, hitCount;
                unsafe
                {
                    int* r = (int*)box.DataPointer;
                    minScan = r[0];
                    hitCount = r[3];
                }
                _ctx.UnmapSubresource(_resultStaging, 0);

                if (hitCount <= 0 || minScan == int.MaxValue)
                    return null;
                int x = minScan % mainW;
                int y = minScan / mainW;
                return new Point(x, y);
            }

            // 初值：[0]=int.MaxValue（min scan 哨兵），[1..3]=0
            private readonly int[] _initState = { int.MaxValue, 0, 0, 0 };

            private unsafe void ClearResult()
            {
                fixed (int* p = _initState)
                {
                    _ctx.UpdateSubresource(
                        new DataBox((IntPtr)p, ResultIntCount * sizeof(int), ResultIntCount * sizeof(int)),
                        _resultBuffer, 0);
                }
            }

            public void Dispose()
            {
                _cbuffer?.Dispose();
                _resultStaging?.Dispose();
                _resultUav?.Dispose();
                _resultBuffer?.Dispose();
                _tplSrv?.Dispose();
                _tplTex?.Dispose();
                _mainSrv?.Dispose();
                _mainTex?.Dispose();
                _shader?.Dispose();
                // _ctx 是 ImmediateContext，随 device 释放
                _device?.Dispose();
            }
        }
    }
}
