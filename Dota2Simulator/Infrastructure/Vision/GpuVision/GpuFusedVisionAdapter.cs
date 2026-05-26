// Infrastructure/Vision/GpuVision/GpuFusedVisionAdapter.cs
// epic Phase 24A C5+C6: GpuFusedVisionAdapter 真实现 — wire DxgiCaptureSession + GpuVisionContext + DXGI 单源 (C6 消双截屏).
#if GpuVision
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Infrastructure.Vision;
using Dota2Simulator.Vision;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Dota2Simulator.Infrastructure.Vision.GpuVision;

/// <summary>
/// IScreenVision 的 GPU 端实现 (epic Phase 24A 全段终态 C1+C2+C3+C5+C6).
///
/// 装配开关: csproj DefineConstants `GpuVision` → AdapterFactory.CreateVision 走本 adapter; 否则 RustVisionAdapter.
///
/// Phase 24A C6 — DXGI 单源 (替代 C5 conservative 双截屏方案):
///   一次 DXGI 截全屏帧 → 同 GPU texture 喂两 sink:
///     sink1: CopyResource → GpuVisionContext._mainTex (SRV-bind) — compute shader 用, 零回传
///     sink2: CopyResource → 本类 _stagingTex (CPU-readable) → Map + 裁剪 mode.Region → byte[] →
///            GlobalScreenCapture.WriteBgraFrameAndCommit → _tripleBuffer (业务侧 GetCurrentHandle/GetColor 透明用)
///   不再调 GDI ModifyGraphics.CaptureScreenToHandle — 业务侧 12 处直调 GlobalScreenCapture 仍 work (透明拿 DXGI 数据).
///
/// 端到端路径:
///   - Capture: DXGI Acquire (主显示器全屏) → 双 sink CopyResource → ReleaseFrame
///   - Find/FindAll: LazyImageLoader 拿 template byte[] → GpuVisionContext.FindInRegion (compute shader region 化)
///   - PixelAt: GlobalScreenCapture.GetColor (内部读 _tripleBuffer = DXGI 帧裁剪后的数据)
///
/// 错误恢复:
///   - DXGI AccessLost: Capture 内 catch InvalidOperationException, 本帧 skip (业务侧 _tripleBuffer 保留上次帧)
///   - 模板加载失败: Find 返 Miss / FindAll 返空
///   - GpuVisionContext.FindInRegion 抛 (UploadMainTexture 未调用): catch 返 Miss
///
/// 已知 limitation:
///   - 单显示器 (DxgiCaptureSession 锁 output 0); 多 monitor 留 Phase 25+
///   - DXGI AccessLost 简单 skip 兜底 (生产升级: catch+ReInit session)
///   - 同步 Map staging buffer 回读: GPU/CPU pipeline stall; C4 fence 异步 epic deferred
/// </summary>
public sealed class GpuFusedVisionAdapter : IScreenVision, IDisposable
{
    private readonly GpuDevice _device;
    private readonly DeviceContext _ctx;
    private readonly DxgiCaptureSession _capture;
    private readonly GpuVisionContext _context;

    // C6 staging texture (DXGI 全屏帧 CPU 回读用)
    private Texture2D? _stagingTex;
    private int _stagingW;
    private int _stagingH;

    // 裁剪 mode.Region 后的 byte[] 缓存 (避免每帧 new 24MB GC pressure)
    private byte[]? _regionBgraCache;

    private bool _disposed;
    private bool _hasUploadedFrame;

    public GpuFusedVisionAdapter()
    {
        _device = new GpuDevice();
        try
        {
            _ctx = _device.ImmediateContext;
            _capture = new DxgiCaptureSession(_device);
            _context = new GpuVisionContext(_device);
        }
        catch
        {
            _context?.Dispose();
            _capture?.Dispose();
            _stagingTex?.Dispose();
            _device.Dispose();
            throw;
        }
    }

    public void Capture(CaptureMode mode)
    {
        try
        {
            if (!_capture.TryAcquireFrame(out Texture2D? frame))
            {
                // 超时 = 桌面静止. _tripleBuffer 与 _context._mainTex 都保留上次帧, 业务侧 GetCurrentHandle 仍合法.
                return;
            }

            try
            {
                // sink1: CopyResource → compute shader SRV (零回传, Find/FindAll 用)
                _context.UploadMainTexture(frame!);
                _hasUploadedFrame = true;

                // sink2: CopyResource → staging → Map → 裁剪 mode.Region → byte[] → _tripleBuffer
                var desc = frame!.Description;
                EnsureStagingTexture(desc.Width, desc.Height);
                _ctx.CopyResource(frame, _stagingTex);
                byte[] regionBytes = CropDxgiFrameToRegion(mode.Region);
                GlobalScreenCapture.WriteBgraFrameAndCommit(
                    regionBytes, mode.Region.Width, mode.Region.Height,
                    mode.Region.X, mode.Region.Y);
            }
            finally
            {
                frame?.Dispose();
                _capture.ReleaseFrame();
            }
        }
        catch (InvalidOperationException)
        {
            // DXGI AccessLost: 本帧 skip, 下次 Capture 再试. 生产升级: Dispose + ReInit DxgiCaptureSession.
        }
    }

    public Color PixelAt(ScreenPoint point)
    {
        // _tripleBuffer 内是 DXGI 帧裁剪后的数据 (C6 单源), GetColor 内部坐标偏移转换合法.
        return GlobalScreenCapture.GetColor(point.X, point.Y);
    }

    /// <summary>Phase 25A C3: typestate frame scope — 走 GlobalScreenFrame singleton, 与 PixelAt 同一底层 (_tripleBuffer 单例 = DXGI 帧裁剪数据).</summary>
    public T WithFrame<T>(Func<IScreenFrame, T> read) => read(GlobalScreenFrame.Instance);

    public FindResult Find(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        if (!_hasUploadedFrame) return FindResult.Miss;
        if (!TryLoadTemplate(needle, out byte[]? bgra, out int tplW, out int tplH))
            return FindResult.Miss;

        Rectangle rect = new(region.X, region.Y, region.Width, region.Height);
        Point? hit;
        try
        {
            hit = _context.FindInRegion(needle, bgra!, tplW, tplH, rect, rate.Value, tolerance.Value);
        }
        catch (InvalidOperationException)
        {
            return FindResult.Miss;
        }
        return hit is null ? FindResult.Miss : FindResult.Hit(new ScreenPoint(hit.Value.X, hit.Value.Y));
    }

    public IReadOnlyList<ScreenPoint> FindAll(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        if (!_hasUploadedFrame) return Array.Empty<ScreenPoint>();
        if (!TryLoadTemplate(needle, out byte[]? bgra, out int tplW, out int tplH))
            return Array.Empty<ScreenPoint>();

        Rectangle rect = new(region.X, region.Y, region.Width, region.Height);
        List<Point> hits;
        try
        {
            hits = _context.FindAllInRegion(needle, bgra!, tplW, tplH, rect, rate.Value, tolerance.Value);
        }
        catch (InvalidOperationException)
        {
            return Array.Empty<ScreenPoint>();
        }
        var result = new ScreenPoint[hits.Count];
        for (int i = 0; i < hits.Count; i++)
            result[i] = new ScreenPoint(hits[i].X, hits[i].Y);
        return result;
    }

    private void EnsureStagingTexture(int width, int height)
    {
        if (_stagingTex != null && _stagingW == width && _stagingH == height) return;
        _stagingTex?.Dispose();
        _stagingTex = new Texture2D(_device.Native, new Texture2DDescription
        {
            Width = width,
            Height = height,
            MipLevels = 1,
            ArraySize = 1,
            Format = Format.B8G8R8A8_UNorm,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Staging,
            BindFlags = BindFlags.None,
            CpuAccessFlags = CpuAccessFlags.Read,
            OptionFlags = ResourceOptionFlags.None
        });
        _stagingW = width;
        _stagingH = height;
    }

    private unsafe byte[] CropDxgiFrameToRegion(Rectangle region)
    {
        int neededLength = region.Width * region.Height * 4;
        if (_regionBgraCache == null || _regionBgraCache.Length != neededLength)
            _regionBgraCache = new byte[neededLength];

        var box = _ctx.MapSubresource(_stagingTex, 0, MapMode.Read, MapFlags.None);
        try
        {
            byte* src = (byte*)box.DataPointer;
            int srcPitch = box.RowPitch;
            int dstPitch = region.Width * 4;
            fixed (byte* dst = _regionBgraCache)
            {
                for (int y = 0; y < region.Height; y++)
                {
                    byte* srcRow = src + (region.Y + y) * srcPitch + region.X * 4;
                    byte* dstRow = dst + y * dstPitch;
                    System.Buffer.MemoryCopy(srcRow, dstRow, dstPitch, dstPitch);
                }
            }
        }
        finally
        {
            _ctx.UnmapSubresource(_stagingTex, 0);
        }
        return _regionBgraCache;
    }

    private static bool TryLoadTemplate(Template needle, out byte[]? bgra, out int tplW, out int tplH)
    {
        bgra = null;
        tplW = 0;
        tplH = 0;
        ImageHandle handle = LazyImageLoader.GetImage(needle.Name);
        if (!handle.IsValid) return false;

        var (ptr, length, size) = ImageManager.GetImageData(in handle);
        bgra = new byte[length];
        Marshal.Copy(ptr, bgra, 0, length);
        GC.KeepAlive(handle);
        tplW = (int)size.x;
        tplH = (int)size.y;
        return true;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _stagingTex?.Dispose();
        _context?.Dispose();
        _capture?.Dispose();
        _device?.Dispose();
    }
}
#endif
