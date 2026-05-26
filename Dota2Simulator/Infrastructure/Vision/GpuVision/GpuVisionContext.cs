// Infrastructure/Vision/GpuVision/GpuVisionContext.cs
// epic Phase 24A C2 — D3D11 compute shader template matching context (production 化, PoC GpuProbeContext 迁移).
//
// 与 PoC (Vision/Benchmark/GpuTemplateMatchProbe.GpuProbeContext) 的差异:
//   1) region 化   : cbuffer 加 RegionX/Y/W/H, CSMain 起始坐标 = (gx + RegionX, gy + RegionY),
//                    上界判定 = gx > RegionW - TplWidth, Dispatch groupsX/Y 按 region 算 (不全屏).
//   2) 单模板路径  : 删除 PoC 多模板 batch z 维 (TplArray slice 数 = 1, Dispatch z=1, HLSL 删 tpl 索引).
//                    C2 epic 范围只服务 IScreenVision.Find/FindAll 单 needle 调用; 多模板批量留 Phase 25+ 后再议.
//   3) Template 缓存: Dictionary<string, ShaderResourceView> 按 Template.Name 缓存模板 SRV
//                    (单 slice ArraySize=1), miss 时上传新 Texture, 命中复用. PoC 是临时上传 30 个 slice.
//   4) FindAll 支持: result buffer = int[1 + MaxHits]
//                    槽 0      = hitCount (atomic counter)
//                    槽 1..N   = 命中点的 scan index (gy_actual * MainWidth + gx_actual)
//                    HLSL 用 InterlockedAdd(Result[0], 1, slot) 原子取 slot, 截断超出 MaxHits 的命中.
//   5) 同步路径    : DispatchBatch 完即同步 CopyResource + Map staging 回读 (Phase 24A C4 fence 异步独立 epic).
//
// 共享 Device: ctor 注入 GpuDevice (C2/C3 共享 D3D11 device, 实现 zero-copy 端到端).
//
// 集成路径: C5 epic 由主 lead 在 GpuFusedVisionAdapter ctor 内 new GpuVisionContext(_device),
//           Find / FindAll 接到这里 FindInRegion / FindAllInRegion.
#if GpuVision
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Dota2Simulator.GameAutomation.Domain.Perception;

namespace Dota2Simulator.Infrastructure.Vision.GpuVision;

/// <summary>
/// GPU compute shader 模板匹配上下文 (production).
///
/// 生命周期: GpuFusedVisionAdapter 单例持有, 进程内一份, Dispose 时释放所有 D3D 资源.
///
/// 调用形态:
///   1) UploadMainTexture(bgra, w, h) — 每帧由 DxgiCaptureSession 调用 (Phase 24A C5 集成时若已产 GPU
///      texture 可加 overload 直接复用 SRV, 跳过 CPU 中转).
///   2) FindInRegion(template, tplBgra, tplW, tplH, region, rate, tol) — 单命中, 返绝对坐标或 null.
///   3) FindAllInRegion(...) — 全命中, 返 List&lt;Point&gt; (绝对坐标, 空表无命中).
///
/// 模板缓存: Template.Name 是缓存键. 第一次见 = 上传 GPU texture (单 slice) + 缓存 SRV; 后续见 = 复用 SRV.
///
/// 不变量:
///   * MainWidth/MainHeight 由最近一次 UploadMainTexture 决定; 同分辨率帧无须重建 texture.
///   * Region 必须在 MainWidth/MainHeight 内 (调用方保证, 本类不再校验越界 — 越界 HLSL 越界判定自然 cull).
///   * MaxHits = 64; 命中超过即截断 (FindAll 用例多为状态栏 icon, 单 region 64 个上限够用).
/// </summary>
public sealed class GpuVisionContext : IDisposable
{
    // ── HLSL: region 化 + FindAll 单模板 compute shader ────────────────────────
    private const int MaxHits = 64;

    private const string HlslSource = @"
// MainTex t0 = big image BGRA (full screen)
// TplTex  t1 = template (Texture2DArray, ArraySize=1, slice 0)
// Result u0  = int[1 + MaxHits]
//                slot 0     : hitCount (atomic, capped at MaxHits)
//                slot 1..N  : scan index (gy_actual * MainWidth + gx_actual)
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
    int   RegionX;
    int   RegionY;
    int   RegionW;
    int   RegionH;
    int   MaxHitsShader;
    float _pad0;
    float _pad1;
};

[numthreads(8, 8, 1)]
void CSMain(uint3 dtid : SV_DispatchThreadID)
{
    int rx = (int)dtid.x;
    int ry = (int)dtid.y;

    // region 内合法候选起点上界: rx <= RegionW - TplWidth 且 ry <= RegionH - TplHeight.
    if (rx > RegionW - TplWidth || ry > RegionH - TplHeight)
        return;

    // 候选起点 = region 偏移 + 局部坐标 (绝对桌面坐标系)
    int gx = rx + RegionX;
    int gy = ry + RegionY;

    int considered = 0;
    int matched    = 0;
    int mismatched = 0;
    bool aborted   = false;

    for (int ty = 0; ty < TplHeight && !aborted; ty++)
    {
        for (int tx = 0; tx < TplWidth; tx++)
        {
            float4 tp = TplTex.Load(int4(tx, ty, 0, 0));

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
        // 原子取 slot, 截断超出 MaxHits 的命中
        int slot;
        InterlockedAdd(Result[0], 1, slot);
        if (slot < MaxHitsShader)
        {
            int scan = gy * MainWidth + gx;
            Result[1 + slot] = scan;
        }
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
        public int RegionX;
        public int RegionY;
        public int RegionW;
        public int RegionH;
        public int MaxHitsShader;
        public float Pad0;
        public float Pad1;
    }

    // ── 注入依赖 ────────────────────────────────────────────────────────────
    private readonly Device _device;
    private readonly DeviceContext _ctx;
    // 不拥有 _device, 不在 Dispose 释放 (由 GpuFusedVisionAdapter 持有 GpuDevice 释放).

    // ── compute shader + 常量 buffer ───────────────────────────────────────
    private readonly ComputeShader _shader;
    private readonly Buffer _cbuffer;

    // ── 结果 buffer (UAV + staging 回读) ────────────────────────────────────
    // 布局: int[1 + MaxHits], 槽 0 = hitCount, 槽 1..MaxHits = scan index.
    private const int ResultIntCount = 1 + MaxHits;
    private readonly Buffer _resultBuffer;
    private readonly UnorderedAccessView _resultUav;
    private readonly Buffer _resultStaging;
    private readonly int[] _zeroResultState; // 全零初值, 每次 Dispatch 前 UpdateSubresource 重置

    // ── 主图 texture (UploadMainTexture 调用上传) ─────────────────────────
    private Texture2D? _mainTex;
    private ShaderResourceView? _mainSrv;
    private int _mainW;
    private int _mainH;

    // ── 模板缓存 (Template.Name -> 单 slice Texture2DArray SRV) ────────────
    private readonly Dictionary<string, CachedTemplate> _templateCache = new();

    private sealed class CachedTemplate : IDisposable
    {
        public Texture2D Texture { get; }
        public ShaderResourceView Srv { get; }
        public int Width { get; }
        public int Height { get; }

        public CachedTemplate(Texture2D tex, ShaderResourceView srv, int w, int h)
        {
            Texture = tex;
            Srv = srv;
            Width = w;
            Height = h;
        }

        public void Dispose()
        {
            Srv?.Dispose();
            Texture?.Dispose();
        }
    }

    private bool _disposed;

    public GpuVisionContext(GpuDevice device)
    {
        if (device is null) throw new ArgumentNullException(nameof(device));
        _device = device.Native;
        _ctx = device.ImmediateContext;

        // ── 编译 HLSL (内联运行时编译, 免 csproj shader 构建配置) ─────────
        using (var bytecode = ShaderBytecode.Compile(
            HlslSource, "CSMain", "cs_5_0", ShaderFlags.OptimizationLevel3))
        {
            if (bytecode.Bytecode == null)
                throw new InvalidOperationException(
                    "GpuVisionContext: HLSL 编译失败 — " + bytecode.Message);
            _shader = new ComputeShader(_device, bytecode.Bytecode);
        }

        // ── 结果 buffer ─────────────────────────────────────────────────
        _resultBuffer = new Buffer(_device, new BufferDescription
        {
            SizeInBytes = ResultIntCount * sizeof(int),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.UnorderedAccess,
            OptionFlags = ResourceOptionFlags.BufferStructured,
            StructureByteStride = sizeof(int),
            CpuAccessFlags = CpuAccessFlags.None
        });
        _resultUav = new UnorderedAccessView(_device, _resultBuffer,
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
        _resultStaging = new Buffer(_device, new BufferDescription
        {
            SizeInBytes = ResultIntCount * sizeof(int),
            Usage = ResourceUsage.Staging,
            BindFlags = BindFlags.None,
            CpuAccessFlags = CpuAccessFlags.Read,
            OptionFlags = ResourceOptionFlags.None
        });
        _zeroResultState = new int[ResultIntCount]; // C# 默认全 0

        // ── 常量 buffer ──
        _cbuffer = new Buffer(_device, new BufferDescription
        {
            SizeInBytes = Marshal.SizeOf<ShaderParams>(),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.ConstantBuffer,
            CpuAccessFlags = CpuAccessFlags.None
        });
    }

    /// <summary>
    /// 上传当前帧大图到 GPU. 同分辨率连续调用复用 texture; 分辨率变化销毁重建.
    ///
    /// Phase 24A C5 集成时若 DxgiCaptureSession 已产 GPU texture, 主 lead 可加 overload
    /// 接 Texture2D 直接复用 SRV (跳过本方法 CPU bytes 上传).
    /// </summary>
    public unsafe void UploadMainTexture(byte[] bgra, int width, int height)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(GpuVisionContext));
        if (bgra is null) throw new ArgumentNullException(nameof(bgra));
        if (bgra.Length < width * height * 4)
            throw new ArgumentException(
                $"bgra 长度 {bgra.Length} 小于 {width}*{height}*4 = {width * height * 4}", nameof(bgra));

        // 分辨率变化或首次上传 — 重建 texture
        if (_mainTex == null || _mainW != width || _mainH != height)
        {
            _mainSrv?.Dispose();
            _mainTex?.Dispose();

            fixed (byte* p = bgra)
            {
                _mainTex = new Texture2D(_device, new Texture2DDescription
                {
                    Width = width,
                    Height = height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.B8G8R8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                }, new DataRectangle((IntPtr)p, width * 4));
            }
            _mainSrv = new ShaderResourceView(_device, _mainTex);
            _mainW = width;
            _mainH = height;
        }
        else
        {
            // 同分辨率 — UpdateSubresource 覆写像素
            fixed (byte* p = bgra)
            {
                _ctx.UpdateSubresource(
                    new DataBox((IntPtr)p, width * 4, width * height * 4),
                    _mainTex, 0);
            }
        }
    }

    /// <summary>
    /// 单 region 单模板首命中. 返绝对桌面坐标或 null. 命中 = 在 region 内 raster-order 最小 scan index.
    /// </summary>
    public Point? FindInRegion(
        Template template, byte[] templateBgra, int tplW, int tplH,
        Rectangle region, double matchRate, byte tolerance)
    {
        var hits = DispatchInternal(template, templateBgra, tplW, tplH, region, matchRate, tolerance);
        if (hits.Count == 0) return null;

        // 找最小 scan (raster-order 首命中 == PoC InterlockedMin 语义复刻)
        int minScan = int.MaxValue;
        foreach (int scan in hits)
            if (scan < minScan) minScan = scan;

        return new Point(minScan % _mainW, minScan / _mainW);
    }

    /// <summary>单 region 单模板全命中. 返绝对桌面坐标列表 (raster order, 命中超 MaxHits 截断).</summary>
    public List<Point> FindAllInRegion(
        Template template, byte[] templateBgra, int tplW, int tplH,
        Rectangle region, double matchRate, byte tolerance)
    {
        var hits = DispatchInternal(template, templateBgra, tplW, tplH, region, matchRate, tolerance);
        var result = new List<Point>(hits.Count);
        // 排序成 raster order (按 scan index 升序) — HLSL InterlockedAdd 取 slot 是无序的
        hits.Sort();
        foreach (int scan in hits)
            result.Add(new Point(scan % _mainW, scan / _mainW));
        return result;
    }

    // ── 核心: Dispatch + 回读, 返 scan index list ──────────────────────────
    private List<int> DispatchInternal(
        Template template, byte[] templateBgra, int tplW, int tplH,
        Rectangle region, double matchRate, byte tolerance)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(GpuVisionContext));
        if (_mainTex == null || _mainSrv == null)
            throw new InvalidOperationException(
                "GpuVisionContext: 必须先调用 UploadMainTexture 后再 Find/FindAll.");
        if (templateBgra is null) throw new ArgumentNullException(nameof(templateBgra));
        if (tplW <= 0 || tplH <= 0)
            throw new ArgumentException($"非法模板尺寸 {tplW}x{tplH}");

        // ── 模板缓存命中? 否则上传 ──
        var tpl = GetOrUploadTemplate(template, templateBgra, tplW, tplH);

        // ── 常量 buffer 填值 ──
        var prm = new ShaderParams
        {
            MainWidth = _mainW,
            MainHeight = _mainH,
            TplWidth = tplW,
            TplHeight = tplH,
            Tolerance = tolerance / 255f,
            MatchRate = (float)matchRate,
            // 与 findpoints / PoC 一致: ignoreColor = RGB(255,20,147), 转 0..1 float
            IgnoreR = 255f / 255f,
            IgnoreG = 20f / 255f,
            IgnoreB = 147f / 255f,
            RegionX = region.X,
            RegionY = region.Y,
            RegionW = region.Width,
            RegionH = region.Height,
            MaxHitsShader = MaxHits
        };
        _ctx.UpdateSubresource(ref prm, _cbuffer);

        // ── 清零 result buffer (hitCount + slots) ──
        ClearResult();

        // ── 绑定 + Dispatch ──
        _ctx.ComputeShader.Set(_shader);
        _ctx.ComputeShader.SetShaderResource(0, _mainSrv);
        _ctx.ComputeShader.SetShaderResource(1, tpl.Srv);
        _ctx.ComputeShader.SetUnorderedAccessView(0, _resultUav);
        _ctx.ComputeShader.SetConstantBuffer(0, _cbuffer);

        // groupsX/Y 按 region 算 — 不全屏扫. 8x8 numthreads => ceil(W/8), ceil(H/8).
        int groupsX = (region.Width + 7) / 8;
        int groupsY = (region.Height + 7) / 8;
        if (groupsX <= 0) groupsX = 1;
        if (groupsY <= 0) groupsY = 1;
        _ctx.Dispatch(groupsX, groupsY, 1);

        // 解绑 UAV (避免下次 Dispatch 撞 hazard warning)
        _ctx.ComputeShader.SetUnorderedAccessView(0, null);

        // ── 同步回读 ──
        _ctx.CopyResource(_resultBuffer, _resultStaging);
        var box = _ctx.MapSubresource(_resultStaging, 0, MapMode.Read, MapFlags.None);
        var hits = new List<int>();
        try
        {
            unsafe
            {
                int* r = (int*)box.DataPointer;
                int hitCount = r[0];
                int cap = hitCount > MaxHits ? MaxHits : hitCount;
                for (int i = 0; i < cap; i++)
                    hits.Add(r[1 + i]);
            }
        }
        finally
        {
            _ctx.UnmapSubresource(_resultStaging, 0);
        }
        return hits;
    }

    private unsafe void ClearResult()
    {
        fixed (int* p = _zeroResultState)
        {
            _ctx.UpdateSubresource(
                new DataBox((IntPtr)p,
                    ResultIntCount * sizeof(int),
                    ResultIntCount * sizeof(int)),
                _resultBuffer, 0);
        }
    }

    /// <summary>模板缓存查 / 上传. 同名 Template 复用; 名不同即新 Texture (slice=1).</summary>
    private unsafe CachedTemplate GetOrUploadTemplate(
        Template template, byte[] bgra, int w, int h)
    {
        if (_templateCache.TryGetValue(template.Name, out var cached))
        {
            // 名相同, 尺寸不同 = 业务侧改了模板 -> 重建
            if (cached.Width == w && cached.Height == h) return cached;
            cached.Dispose();
            _templateCache.Remove(template.Name);
        }

        Texture2D tex;
        fixed (byte* p = bgra)
        {
            // 单 slice Texture2DArray (HLSL TplTex 类型固定 Texture2DArray)
            var box = new DataBox((IntPtr)p, w * 4, w * h * 4);
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
            }, new[] { box });
        }
        var srv = new ShaderResourceView(_device, tex);
        var fresh = new CachedTemplate(tex, srv, w, h);
        _templateCache[template.Name] = fresh;
        return fresh;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var t in _templateCache.Values) t.Dispose();
        _templateCache.Clear();

        _mainSrv?.Dispose();
        _mainTex?.Dispose();
        _cbuffer?.Dispose();
        _resultStaging?.Dispose();
        _resultUav?.Dispose();
        _resultBuffer?.Dispose();
        _shader?.Dispose();
        // _device 由 GpuFusedVisionAdapter 释放, 这里不动.
    }
}
#endif
