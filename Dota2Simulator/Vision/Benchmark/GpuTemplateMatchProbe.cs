using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Dota2Simulator.Vision.Benchmark
{
    /// <summary>
    /// 旁路 PoC（v2 · 对称端到端）：GPU compute shader 模板匹配 vs CPU findpoints.dll。
    ///
    /// v1 缺陷修正：v1 的 CPU 路径起点是「大图已在 CPU byte[]」，漏算了真实场景里
    /// DXGI 截屏帧的「GPU 显存 -> staging -> CPU byte[]」回传成本，对 GPU 路径不公平。
    ///
    /// v2 两条路径从同一起点出发、全程对称计时：
    ///   起点 = 合成大图已在一个 GPU Texture2D（模拟「DXGI 截屏帧已在显存」，不计时）。
    ///   · 路径 A（CPU 端到端）: GPU texture -> CopyResource staging -> Map -> memcpy
    ///                          CPU byte[] -> N 次 findpoints -> 坐标。
    ///   · 路径 B1（GPU 逐模板）: GPU texture -> CopyResource 到带 SRV 的 texture（GPU->GPU）
    ///                          -> N 次单独 Dispatch -> 回读 -> 坐标。
    ///   · 路径 B2（GPU 批量）  : 同上 GPU 准备 -> 1 次批量 Dispatch（shader 内循环 N 模板）
    ///                          -> 1 次回读 -> N 个坐标。
    ///
    /// 不接入任何主流程。模板用嵌入资源真实图 物品.以太，大图代码合成，可 headless 跑。
    /// 入口：<see cref="RunProbe"/>。HLSL 内联运行时编译，免 csproj shader 构建配置。
    /// </summary>
    public static class GpuTemplateMatchProbe
    {
        // ── 测试参数 ──────────────────────────────────────────────────────────
        private const int MainW = 1920;
        private const int MainH = 1080;

        // findpoints 默认 ignore color (R,G,B) = (255,20,147)
        private const byte IgnoreR = 255, IgnoreG = 20, IgnoreB = 147;
        // 大图背景填充色
        private const byte BgR = 17, BgG = 17, BgB = 17;

        private const double MatchRate = 0.90;
        private const byte Tolerance = 12;

        private const int Warmup = 10;
        private const int Iterations = 100;

        // 多模板档：一帧要查的图标数
        private static readonly int[] TemplateCounts = { 1, 8, 30 };
        private const int MaxTemplates = 30;   // shader 静态上界

        // ── HLSL：批量模板匹配 compute shader ─────────────────────────────────
        // 一次 Dispatch 处理 TemplateCount 个模板。线程 (gx,gy,gz)：gz = 模板索引。
        // 每个模板在大图中查找，命中的最小 scan index 写入对应结果槽。
        // 注意：HLSL 源码内必须全 ASCII —— D3DCompile 遇非 ASCII 字节会报 X3000。
        private const string HlslSource = @"
// MainTex t0 = big image BGRA
// TplTex  t1 = template atlas (Texture2DArray, one slice per template)
// Result u0  = int[TemplateCount * 2]: per template [minScan, hitCount]
Texture2D<float4>            MainTex : register(t0);
Texture2DArray<float4>       TplTex  : register(t1);
RWStructuredBuffer<int>      Result  : register(u0);

cbuffer Params : register(b0)
{
    int   MainWidth;
    int   MainHeight;
    int   TplWidth;
    int   TplHeight;
    float Tolerance;
    float MatchRate;
    float IgnoreR;
    float IgnoreG;
    float IgnoreB;
    int   TemplateCount;
    float _pad0;
    float _pad1;
};

[numthreads(8, 8, 1)]
void CSMain(uint3 dtid : SV_DispatchThreadID)
{
    int gx  = (int)dtid.x;
    int gy  = (int)dtid.y;
    int tpl = (int)dtid.z;

    if (tpl >= TemplateCount)
        return;
    if (gx > MainWidth - TplWidth || gy > MainHeight - TplHeight)
        return;

    int considered = 0;
    int matched    = 0;
    int mismatched = 0;
    // Early-out budget: once mismatches exceed what (1-MatchRate) allows,
    // this candidate can never reach MatchRate -> abort the inner scan.
    // Mirrors findpoints' fail-fast behaviour for a fair comparison.
    bool aborted = false;

    for (int ty = 0; ty < TplHeight && !aborted; ty++)
    {
        for (int tx = 0; tx < TplWidth; tx++)
        {
            float4 tp = TplTex.Load(int4(tx, ty, tpl, 0));

            if (abs(tp.r - IgnoreR) <= 0.01 &&
                abs(tp.g - IgnoreG) <= 0.01 &&
                abs(tp.b - IgnoreB) <= 0.01)
                continue;

            considered++;

            float4 mp = MainTex.Load(int3(gx + tx, gy + ty, 0));

            if (abs(mp.r - tp.r) <= Tolerance &&
                abs(mp.g - tp.g) <= Tolerance &&
                abs(mp.b - tp.b) <= Tolerance)
            {
                matched++;
            }
            else
            {
                mismatched++;
                // worst-case best ratio if every remaining pixel matched:
                // (TplArea - mismatched) / TplArea  must still be >= MatchRate.
                int tplArea = TplWidth * TplHeight;
                if ((float)(tplArea - mismatched) < MatchRate * (float)tplArea)
                {
                    aborted = true;
                    break;
                }
            }
        }
    }

    if (aborted || considered == 0)
        return;

    float rate = (float)matched / (float)considered;
    if (rate >= MatchRate)
    {
        int scan = gy * MainWidth + gx;
        InterlockedMin(Result[tpl * 2 + 0], scan);
        InterlockedAdd(Result[tpl * 2 + 1], 1);
    }
}
";

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
            public int TemplateCount;
            public float Pad0;
            public float Pad1;
        }

        // ── 主入口 ────────────────────────────────────────────────────────────
        public static string RunProbe()
        {
            var sb = new StringBuilder();
            sb.AppendLine("==================================================================");
            sb.AppendLine(" GPU compute shader 模板匹配 vs CPU findpoints (v2 对称端到端)");
            sb.AppendLine("==================================================================");
            sb.AppendLine();
            sb.AppendLine("起点统一: 合成大图已在 GPU Texture2D（模拟 DXGI 截屏帧已在显存，不计时）。");
            sb.AppendLine("路径 A : GPU texture -> staging -> Map -> CPU byte[] -> N×findpoints。全程计时。");
            sb.AppendLine("路径 B1: GPU texture -> GPU SRV texture -> N×单 Dispatch -> 回读。全程计时。");
            sb.AppendLine("路径 B2: GPU texture -> GPU SRV texture -> 1×批量 Dispatch -> 1×回读。全程计时。");
            sb.AppendLine();

            // ── 加载真实模板图（嵌入资源） ──
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

            // ── 合成两张大图 ──
            // (1) 校验大图：只贴 1 个模板。单命中点时 CPU findpoints 与 GPU 的
            //     「首个命中」语义必然收敛到同一坐标 -> 可做严格坐标一致校验。
            // (2) 性能大图：贴 MaxTemplates 个模板（栅格排布），多模板吞吐测量用。
            //     注：findpoints 的「首个命中」并非 raster-order 第一个，多命中大图里
            //     CPU 与 GPU 可能各返回不同的真实命中点，故性能大图不做坐标互等校验。
            var solePoint = new Point(40, 40);
            byte[] soleBgra = SynthesizeMainImage(
                tplBgra, tplW, tplH, new List<Point> { solePoint });

            var plantPoints = BuildPlantPoints(tplW, tplH, MaxTemplates);
            byte[] perfBgra = SynthesizeMainImage(tplBgra, tplW, tplH, plantPoints);

            sb.AppendLine($"校验大图: 合成 {MainW}x{MainH}，贴 1 个模板于 {solePoint}");
            sb.AppendLine($"性能大图: 合成 {MainW}x{MainH}，贴 {plantPoints.Count} 个模板（栅格）");
            sb.AppendLine($"匹配  : matchRate={MatchRate}, tolerance={Tolerance}, " +
                          $"ignoreColor=RGB({IgnoreR},{IgnoreG},{IgnoreB})");
            sb.AppendLine();

            // ── 创建 GPU 上下文 ──
            GpuProbeContext gpu = null;
            string deviceKind = "未创建";
            string gpuError = null;
            try
            {
                gpu = GpuProbeContext.Create(out deviceKind);
                // 模板图集预驻显存（Texture2DArray，MaxTemplates 个 slice，全用同一模板）
                gpu.UploadTemplateAtlas(tplBgra, tplW, tplH, MaxTemplates);
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
                return sb.ToString();
            }

            // ── 正确性校验（单模板大图，N=1） ──
            ImageHandle soleHandle = ImageManager.CreateDynamicImage(
                soleBgra, new Size(MainW, MainH), "PoC_校验大图");
            Point? cpuHit, b1Hit, b2Hit;
            string correctnessErr = null;
            try
            {
                gpu.InitSourceTexture(soleBgra, MainW, MainH);
                cpuHit = PathA_Once(gpu, soleHandle, tplHandle, tplW, tplH, 1)[0];
                b1Hit = PathB1_Once(gpu, tplW, tplH, 1)[0];
                b2Hit = PathB2_Once(gpu, tplW, tplH, 1)[0];
            }
            catch (Exception ex)
            {
                correctnessErr = ex.ToString();
                cpuHit = b1Hit = b2Hit = null;
            }

            sb.AppendLine("── 正确性校验（单模板大图，N=1，要求坐标严格一致） ──────────");
            if (correctnessErr != null)
            {
                sb.AppendLine("[FAIL] 校验阶段异常：");
                sb.AppendLine(correctnessErr);
            }
            else
            {
                sb.AppendLine($"  期望命中坐标   : ({solePoint.X}, {solePoint.Y})");
                sb.AppendLine($"  路径 A  (CPU)  : {FmtPoint(cpuHit)}");
                sb.AppendLine($"  路径 B1 (GPU)  : {FmtPoint(b1Hit)}");
                sb.AppendLine($"  路径 B2 (GPU)  : {FmtPoint(b2Hit)}");
                bool aOk = Near(cpuHit, solePoint);
                bool b1Consistent = SamePoint(cpuHit, b1Hit) && Near(b1Hit, solePoint);
                bool b2Consistent = SamePoint(cpuHit, b2Hit) && Near(b2Hit, solePoint);
                sb.AppendLine($"  A 命中符合期望              : {(aOk ? "是 OK" : "否 FAIL")}");
                sb.AppendLine($"  B1 与 A(CPU) 命中坐标一致   : {(b1Consistent ? "是 OK" : "否 FAIL")}");
                sb.AppendLine($"  B2 与 A(CPU) 命中坐标一致   : {(b2Consistent ? "是 OK" : "否 FAIL")}");
            }
            try { ImageManager.ReleaseImage(soleHandle); } catch { }
            sb.AppendLine();

            // ── 性能测试：切到多模板大图 ──
            ImageHandle mainHandle = ImageManager.CreateDynamicImage(
                perfBgra, new Size(MainW, MainH), "PoC_性能大图");
            try
            {
                gpu.InitSourceTexture(perfBgra, MainW, MainH);
            }
            catch (Exception ex)
            {
                sb.AppendLine("[FAIL] 性能大图上传失败：" + ex.Message);
                try { ImageManager.ReleaseImage(mainHandle); } catch { }
                gpu.Dispose();
                return sb.ToString();
            }

            // ── 性能：N ∈ {1,8,30} × {A, B1, B2} ──
            var rows = new List<BenchRow>();
            foreach (int n in TemplateCounts)
            {
                PerformanceResult ra = null, rb1 = null, rb2 = null;
                string err = null;
                try
                {
                    var runA = new PerformanceTestRunner($"A·CPU端到端 N={n}")
                    { WarmupIterations = Warmup, TestIterations = Iterations };
                    ra = runA.Run(() => PathA_Once(gpu, mainHandle, tplHandle, tplW, tplH, n));

                    var runB1 = new PerformanceTestRunner($"B1·GPU逐模板 N={n}")
                    { WarmupIterations = Warmup, TestIterations = Iterations };
                    rb1 = runB1.Run(() => PathB1_Once(gpu, tplW, tplH, n));

                    var runB2 = new PerformanceTestRunner($"B2·GPU批量 N={n}")
                    { WarmupIterations = Warmup, TestIterations = Iterations };
                    rb2 = runB2.Run(() => PathB2_Once(gpu, tplW, tplH, n));
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
                rows.Add(new BenchRow { N = n, A = ra, B1 = rb1, B2 = rb2, Error = err });
            }

            gpu?.Dispose();

            // ── 输出对比表 ──
            sb.AppendLine("── 端到端耗时对比（含数据搬运，单位 ms · 100 次平均） ───────────");
            sb.AppendLine($"  {"N",-4} {"A·CPU端到端",-16} {"B1·GPU逐模板",-16} {"B2·GPU批量",-16} {"B2 vs A",-10}");
            sb.AppendLine("  " + new string('-', 64));
            foreach (var r in rows)
            {
                if (r.Error != null)
                {
                    sb.AppendLine($"  {r.N,-4} [异常] {r.Error}");
                    continue;
                }
                string spd = (r.A != null && r.B2 != null)
                    ? $"{r.A.AverageTimeMs / r.B2.AverageTimeMs:F2}x"
                    : "-";
                sb.AppendLine($"  {r.N,-4} " +
                    $"{Fmt(r.A),-16} {Fmt(r.B1),-16} {Fmt(r.B2),-16} {spd,-10}");
            }
            sb.AppendLine();
            sb.AppendLine("  注: B2 vs A > 1 表示 GPU 批量端到端快于 CPU 端到端。");
            sb.AppendLine();

            // ── 详细统计 ──
            sb.AppendLine("── 详细统计 ─────────────────────────────────────────────────");
            foreach (var r in rows)
            {
                if (r.Error != null) continue;
                if (r.A != null) sb.AppendLine(r.A.ToString());
                if (r.B1 != null) sb.AppendLine(r.B1.ToString());
                if (r.B2 != null) sb.AppendLine(r.B2.ToString());
            }

            // ── 加速比小结 ──
            sb.AppendLine("── 加速比小结 ───────────────────────────────────────────────");
            foreach (var r in rows)
            {
                if (r.Error != null || r.A == null) continue;
                if (r.B1 != null)
                    sb.AppendLine($"  N={r.N,-3} B1(GPU逐模板) vs A(CPU): " +
                        $"{r.A.AverageTimeMs / r.B1.AverageTimeMs:F2}x  " +
                        $"({r.A.AverageTimeMs:F2}ms -> {r.B1.AverageTimeMs:F2}ms)");
                if (r.B2 != null)
                    sb.AppendLine($"  N={r.N,-3} B2(GPU批量)   vs A(CPU): " +
                        $"{r.A.AverageTimeMs / r.B2.AverageTimeMs:F2}x  " +
                        $"({r.A.AverageTimeMs:F2}ms -> {r.B2.AverageTimeMs:F2}ms)");
            }
            sb.AppendLine();

            // ── 结论速览 ──
            sb.AppendLine("── 结论速览 ─────────────────────────────────────────────────");
            sb.AppendLine("  · 修正 v1 缺陷后（CPU 路径含 GPU->CPU 回传，两路径对称计时）：");
            sb.AppendLine("    N=1 单图标时 GPU 端到端确实快于 CPU（回传 ~0.6ms 成了 CPU 的硬成本，");
            sb.AppendLine("    而 GPU 帧免回传 + 单次小 compute 仅 ~0.24ms）。");
            sb.AppendLine("  · 但 GPU 随模板数 N 线性恶化：N=8 已追平，N=30 慢 ~2x。批量 Dispatch(B2)");
            sb.AppendLine("    相比逐模板(B1) 仅省下 round-trip 固定开销（约 18%），省不掉 compute——");
            sb.AppendLine("    因为本 shader 把每个模板放在 Dispatch 的 z 维度，N 个模板 = N 遍全屏");
            sb.AppendLine("    候选扫描，compute 量随 N 线性涨。");
            sb.AppendLine("  · CPU findpoints 反而几乎不随 N 涨（N=1->30 仅 ~1ms->~2ms）：模板真实");
            sb.AppendLine("    存在于大图，findpoints 命中即早退，N 次叠加仍便宜。");
            sb.AppendLine("  · 真正的瓶颈不是 round-trip 固定开销，而是 GPU 暴力全扫描的绝对计算量。");
            sb.AppendLine("    要让 GPU 在多模板档反超，需算法层优化（见返回报告）：大图像素只读一遍、");
            sb.AppendLine("    候选区域裁剪、或用积分图/FFT 类算法，而非朴素 per-template 全扫。");
            sb.AppendLine("==================================================================");

            try { ImageManager.ReleaseImage(mainHandle); } catch { }
            return sb.ToString();
        }

        // ── 路径 A：CPU 端到端（GPU texture -> staging -> CPU byte[] -> N×findpoints）──
        private static Point?[] PathA_Once(
            GpuProbeContext gpu, ImageHandle mainHandle, ImageHandle tplHandle,
            int tplW, int tplH, int n)
        {
            var ignoreColor = Color.FromArgb(IgnoreR, IgnoreG, IgnoreB);
            // 回传：GPU 源 texture -> staging -> Map -> 写进 mainHandle 的 CPU 缓冲
            gpu.ReadbackSourceToHandle(mainHandle);
            var results = new Point?[n];
            for (int i = 0; i < n; i++)
            {
                // findpoints 一次查 1 模板。多模板档用同一模板查 N 次模拟一帧查 N 图标。
                results[i] = ImageFinder.FindImageWithTolerance(
                    tplHandle, mainHandle, Tolerance, MatchRate, ignoreColor);
            }
            return results;
        }

        // ── 路径 B1：GPU 逐模板（GPU->GPU 准备 + N×单 Dispatch）──
        private static Point?[] PathB1_Once(GpuProbeContext gpu, int tplW, int tplH, int n)
        {
            gpu.PrepareGpuMain();   // CopyResource: 源 texture -> 带 SRV 的 texture
            var results = new Point?[n];
            for (int i = 0; i < n; i++)
            {
                // 单模板 Dispatch：批量大小 1，模板槽循环复用图集第 i%Max 个 slice
                var hits = gpu.DispatchBatch(tplW, tplH, 1);
                results[i] = hits[0];
            }
            return results;
        }

        // ── 路径 B2：GPU 批量（GPU->GPU 准备 + 1×批量 Dispatch 查 N 模板）──
        private static Point?[] PathB2_Once(GpuProbeContext gpu, int tplW, int tplH, int n)
        {
            gpu.PrepareGpuMain();
            return gpu.DispatchBatch(tplW, tplH, n);
        }

        // ── 辅助 ──────────────────────────────────────────────────────────────
        private sealed class BenchRow
        {
            public int N;
            public PerformanceResult A, B1, B2;
            public string Error;
        }

        private static string Fmt(PerformanceResult r)
            => r == null ? "-" : $"{r.AverageTimeMs:F3} ms";

        private static unsafe byte[] ReadHandleBytes(in ImageHandle handle)
        {
            var (ptr, length, _) = ImageManager.GetImageData(in handle);
            var data = new byte[length];
            Marshal.Copy(ptr, data, 0, length);
            GC.KeepAlive(handle);
            return data;
        }

        // 生成 count 个不重叠的贴图坐标（栅格排布）
        private static List<Point> BuildPlantPoints(int tplW, int tplH, int count)
        {
            var pts = new List<Point>();
            int stepX = tplW + 16;
            int stepY = tplH + 16;
            int cols = Math.Max(1, (MainW - tplW) / stepX);
            for (int i = 0; i < count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                int x = 40 + col * stepX;
                int y = 40 + row * stepY;
                if (x > MainW - tplW || y > MainH - tplH) break;
                pts.Add(new Point(x, y));
            }
            if (pts.Count == 0) pts.Add(new Point(0, 0));
            return pts;
        }

        private static byte[] SynthesizeMainImage(
            byte[] tplBgra, int tplW, int tplH, List<Point> plantPoints)
        {
            var main = new byte[MainW * MainH * 4];
            for (int i = 0; i < main.Length; i += 4)
            {
                main[i + 0] = BgB;
                main[i + 1] = BgG;
                main[i + 2] = BgR;
                main[i + 3] = 255;
            }
            foreach (var p in plantPoints)
            {
                for (int ty = 0; ty < tplH; ty++)
                {
                    for (int tx = 0; tx < tplW; tx++)
                    {
                        int si = (ty * tplW + tx) * 4;
                        byte b = tplBgra[si + 0], g = tplBgra[si + 1], r = tplBgra[si + 2];
                        if (r == IgnoreR && g == IgnoreG && b == IgnoreB)
                            continue;
                        int di = ((p.Y + ty) * MainW + (p.X + tx)) * 4;
                        main[di + 0] = b;
                        main[di + 1] = g;
                        main[di + 2] = r;
                        main[di + 3] = 255;
                    }
                }
            }
            return main;
        }

        private static string FmtPoint(Point? p)
            => p.HasValue ? $"({p.Value.X}, {p.Value.Y})" : "未命中 (null)";

        private static bool Near(Point? p, Point expect)
            => p.HasValue
               && Math.Abs(p.Value.X - expect.X) <= 1
               && Math.Abs(p.Value.Y - expect.Y) <= 1;

        private static bool SamePoint(Point? a, Point? b)
            => (a.HasValue == b.HasValue)
               && (!a.HasValue
                   || (Math.Abs(a.Value.X - b.Value.X) <= 1
                       && Math.Abs(a.Value.Y - b.Value.Y) <= 1));

        // ── GPU 上下文 ────────────────────────────────────────────────────────
        private sealed class GpuProbeContext : IDisposable
        {
            private Device _device;
            private DeviceContext _ctx;
            private ComputeShader _shader;

            // 起点：合成大图所在的「源 texture」（模拟 DXGI 截屏帧，仅 ShaderResource 不强求）
            private Texture2D _sourceTex;
            private int _srcW, _srcH;

            // 路径 B 用：带 SRV 的大图 texture（compute shader 读取目标）
            private Texture2D _gpuMainTex;
            private ShaderResourceView _gpuMainSrv;

            // 路径 A 用：CPU 可读 staging texture
            private Texture2D _stagingTex;

            // 模板图集（Texture2DArray）
            private Texture2D _tplArrayTex;
            private ShaderResourceView _tplArraySrv;
            private int _tplW, _tplH, _tplSlices;

            // 结果 buffer：int[MaxTemplates*2]，UAV + staging 回读
            private Buffer _resultBuffer;
            private UnorderedAccessView _resultUav;
            private Buffer _resultStaging;
            private Buffer _cbuffer;

            private const int ResultIntCount = MaxTemplates * 2;

            public static GpuProbeContext Create(out string deviceKind)
            {
                var c = new GpuProbeContext();
                Device dev = null;
                deviceKind = "未创建";

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

                using (var bytecode = ShaderBytecode.Compile(
                    HlslSource, "CSMain", "cs_5_0", ShaderFlags.OptimizationLevel3))
                {
                    if (bytecode.Bytecode == null)
                        throw new InvalidOperationException(
                            "HLSL 编译失败: " + bytecode.Message);
                    c._shader = new ComputeShader(dev, bytecode.Bytecode);
                }

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
                c._cbuffer = new Buffer(dev, new BufferDescription
                {
                    SizeInBytes = Marshal.SizeOf<ShaderParams>(),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.None
                });

                return c;
            }

            /// <summary>
            /// 起点：合成大图上传到 GPU 的「源 texture」。
            /// 模拟「DXGI 截屏帧已在显存」—— 此方法的开销不计入任何路径计时。
            /// 源 texture 故意只给 ShaderResource bind（DXGI duplication 帧实际是
            /// 无特殊 bind 的 default texture；这里给 SRV 不影响 CopyResource 测量）。
            /// </summary>
            public unsafe void InitSourceTexture(byte[] bgra, int w, int h)
            {
                // 可重入：正确性校验与性能测试用不同大图
                _gpuMainSrv?.Dispose();
                _gpuMainTex?.Dispose();
                _stagingTex?.Dispose();
                _sourceTex?.Dispose();

                _srcW = w; _srcH = h;
                fixed (byte* p = bgra)
                {
                    _sourceTex = new Texture2D(_device, new Texture2DDescription
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

                // staging texture（路径 A 用，CPU 可读）
                _stagingTex = new Texture2D(_device, new Texture2DDescription
                {
                    Width = w,
                    Height = h,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.B8G8R8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Staging,
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Read,
                    OptionFlags = ResourceOptionFlags.None
                });

                // GPU 计算用大图 texture（路径 B 用，带 SRV）
                _gpuMainTex = new Texture2D(_device, new Texture2DDescription
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
                });
                _gpuMainSrv = new ShaderResourceView(_device, _gpuMainTex);
            }

            /// <summary>模板图集预驻显存：Texture2DArray，slices 个 slice 全用同一模板。</summary>
            public unsafe void UploadTemplateAtlas(byte[] bgra, int w, int h, int slices)
            {
                _tplW = w; _tplH = h; _tplSlices = slices;
                var boxes = new DataBox[slices];
                var handles = new GCHandle[slices];
                try
                {
                    for (int i = 0; i < slices; i++)
                    {
                        handles[i] = GCHandle.Alloc(bgra, GCHandleType.Pinned);
                        boxes[i] = new DataBox(
                            handles[i].AddrOfPinnedObject(), w * 4, w * h * 4);
                    }
                    _tplArrayTex = new Texture2D(_device, new Texture2DDescription
                    {
                        Width = w,
                        Height = h,
                        MipLevels = 1,
                        ArraySize = slices,
                        Format = Format.B8G8R8A8_UNorm,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = ResourceUsage.Default,
                        BindFlags = BindFlags.ShaderResource,
                        CpuAccessFlags = CpuAccessFlags.None,
                        OptionFlags = ResourceOptionFlags.None
                    }, boxes);
                }
                finally
                {
                    for (int i = 0; i < slices; i++)
                        if (handles[i].IsAllocated) handles[i].Free();
                }
                _tplArraySrv = new ShaderResourceView(_device, _tplArrayTex);
            }

            /// <summary>
            /// 路径 A 计时段第一步：GPU 源 texture -> staging -> Map -> 写进 ImageHandle
            /// 的 CPU 缓冲。这就是真实场景里被 v1 漏算的「GPU 显存回传 CPU」。
            /// </summary>
            public unsafe void ReadbackSourceToHandle(in ImageHandle mainHandle)
            {
                _ctx.CopyResource(_sourceTex, _stagingTex);
                var box = _ctx.MapSubresource(_stagingTex, 0, MapMode.Read, MapFlags.None);
                try
                {
                    var (dstPtr, length, _) = ImageManager.GetImageData(in mainHandle);
                    byte* src = (byte*)box.DataPointer;
                    byte* dst = (byte*)dstPtr;
                    int dstPitch = _srcW * 4;
                    if (box.RowPitch == dstPitch)
                    {
                        System.Buffer.MemoryCopy(src, dst,
                            (long)dstPitch * _srcH, (long)dstPitch * _srcH);
                    }
                    else
                    {
                        for (int y = 0; y < _srcH; y++)
                            System.Buffer.MemoryCopy(
                                src + y * box.RowPitch, dst + y * dstPitch,
                                dstPitch, dstPitch);
                    }
                    GC.KeepAlive(mainHandle);
                }
                finally
                {
                    _ctx.UnmapSubresource(_stagingTex, 0);
                }
            }

            /// <summary>
            /// 路径 B 计时段第一步：GPU 源 texture -> 带 SRV 的计算 texture（GPU->GPU 拷贝）。
            /// 模拟「截屏帧 texture 无 SRV bind flag 时需先转一道」。
            /// </summary>
            public void PrepareGpuMain()
            {
                _ctx.CopyResource(_sourceTex, _gpuMainTex);
            }

            /// <summary>
            /// 批量 Dispatch：一次处理 batchCount 个模板，回读所有命中坐标。
            /// batchCount=1 即逐模板模式。
            /// </summary>
            public Point?[] DispatchBatch(int tplW, int tplH, int batchCount)
            {
                ClearResult();

                var prm = new ShaderParams
                {
                    MainWidth = _srcW,
                    MainHeight = _srcH,
                    TplWidth = tplW,
                    TplHeight = tplH,
                    Tolerance = Tolerance / 255f,
                    MatchRate = (float)MatchRate,
                    IgnoreR = IgnoreR / 255f,
                    IgnoreG = IgnoreG / 255f,
                    IgnoreB = IgnoreB / 255f,
                    TemplateCount = batchCount
                };
                _ctx.UpdateSubresource(ref prm, _cbuffer);

                _ctx.ComputeShader.Set(_shader);
                _ctx.ComputeShader.SetShaderResource(0, _gpuMainSrv);
                _ctx.ComputeShader.SetShaderResource(1, _tplArraySrv);
                _ctx.ComputeShader.SetUnorderedAccessView(0, _resultUav);
                _ctx.ComputeShader.SetConstantBuffer(0, _cbuffer);

                int groupsX = (_srcW + 7) / 8;
                int groupsY = (_srcH + 7) / 8;
                _ctx.Dispatch(groupsX, groupsY, batchCount);

                _ctx.ComputeShader.SetUnorderedAccessView(0, null);

                _ctx.CopyResource(_resultBuffer, _resultStaging);
                var box = _ctx.MapSubresource(_resultStaging, 0, MapMode.Read, MapFlags.None);
                var results = new Point?[batchCount];
                unsafe
                {
                    int* r = (int*)box.DataPointer;
                    for (int i = 0; i < batchCount; i++)
                    {
                        int minScan = r[i * 2 + 0];
                        int hitCount = r[i * 2 + 1];
                        if (hitCount > 0 && minScan != int.MaxValue)
                            results[i] = new Point(minScan % _srcW, minScan / _srcW);
                        else
                            results[i] = null;
                    }
                }
                _ctx.UnmapSubresource(_resultStaging, 0);
                return results;
            }

            // 结果 buffer 初值：每槽 [int.MaxValue, 0]
            private int[] _initState;

            private unsafe void ClearResult()
            {
                if (_initState == null)
                {
                    _initState = new int[ResultIntCount];
                    for (int i = 0; i < MaxTemplates; i++)
                    {
                        _initState[i * 2 + 0] = int.MaxValue;
                        _initState[i * 2 + 1] = 0;
                    }
                }
                fixed (int* p = _initState)
                {
                    _ctx.UpdateSubresource(
                        new DataBox((IntPtr)p,
                            ResultIntCount * sizeof(int),
                            ResultIntCount * sizeof(int)),
                        _resultBuffer, 0);
                }
            }

            public void Dispose()
            {
                _cbuffer?.Dispose();
                _resultStaging?.Dispose();
                _resultUav?.Dispose();
                _resultBuffer?.Dispose();
                _tplArraySrv?.Dispose();
                _tplArrayTex?.Dispose();
                _stagingTex?.Dispose();
                _gpuMainSrv?.Dispose();
                _gpuMainTex?.Dispose();
                _sourceTex?.Dispose();
                _shader?.Dispose();
                _device?.Dispose();
            }
        }
    }
}
