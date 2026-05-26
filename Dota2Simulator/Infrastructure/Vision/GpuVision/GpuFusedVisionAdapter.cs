// Infrastructure/Vision/GpuVision/GpuFusedVisionAdapter.cs
// epic Phase 24A C5: GpuFusedVisionAdapter 真实现 — wire DxgiCaptureSession → GpuVisionContext → IScreenVision API.
// C1 骨架阶段类放在 Infrastructure/Vision/ 平铺, C5 迁子目录 Infrastructure/Vision/GpuVision/ + 改 namespace 统一.
#if GpuVision
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Vision;

namespace Dota2Simulator.Infrastructure.Vision.GpuVision;

/// <summary>
/// IScreenVision 的 GPU 端实现 (epic Phase 24A 全段终态).
///
/// 装配开关: csproj DefineConstants `GpuVision` → AdapterFactory.CreateVision 走本 adapter; 否则 RustVisionAdapter.
///
/// 端到端路径:
///   - Capture: DxgiCaptureSession.TryAcquireFrame (DXGI Desktop Duplication 直产 GPU Texture2D)
///              → GpuVisionContext.UploadMainTexture(Texture2D) (CopyResource 到 SRV-bind texture, GPU 零回传)
///              → ReleaseFrame (DXGI 契约配对)
///              + 同时调 GlobalScreenCapture.CaptureScreen 维护 GDI 帧 (业务侧 12 处直调 GlobalScreenCapture
///                GetCurrentHandle 仍 work; Phase 23 B#4 派生候选 12 处切 _vision.PixelAt 端口后可消)
///   - Find/FindAll: LazyImageLoader 拿 template byte[] → GpuVisionContext.FindInRegion/FindAllInRegion
///                   → compute shader region 化匹配 → FindResult
///   - PixelAt: GDI fallback (GlobalScreenCapture.GetColor) — GPU Map staging buffer 单点开销不优, 当前业务侧
///              PixelAt 真调用 4 处 (HeroLoopHost / SkillEngine / Silt / ProbeScreenVision), GDI 路径满足
///
/// 错误恢复:
///   - DXGI AccessLost: Capture 内 catch InvalidOperationException, 本帧 skip; 生产场景应 catch + recreate
///     session, 当前简单 skip 兜底 (下次 Capture 再试可能仍 lost, 业务侧呈现卡顿但不崩)
///   - 模板加载失败 (LazyImageLoader.GetImage IsValid=false): Find 返 Miss / FindAll 返空数组
///   - GpuVisionContext.FindInRegion 抛 InvalidOperationException (UploadMainTexture 未调用): catch 返 Miss
///
/// 已知 limitation:
///   - 单显示器 (DxgiCaptureSession 锁 output index 0); 多 monitor 场景 Phase 25+
///   - 双截屏 (DXGI + GDI 并行) 性能代价: 业务侧 12 处直调 GlobalScreenCapture.GetCurrentHandle 兼容期成本
///   - 同步 Map staging buffer 回读: GPU/CPU pipeline stall; C4 fence 异步 epic deferred 评估
/// </summary>
public sealed class GpuFusedVisionAdapter : IScreenVision, IDisposable
{
    private readonly GpuDevice _device;
    private readonly DxgiCaptureSession _capture;
    private readonly GpuVisionContext _context;
    private bool _disposed;
    private bool _hasUploadedFrame;  // UploadMainTexture 是否成功调用过 (DXGI 0 帧时 Find 应早退 Miss)

    public GpuFusedVisionAdapter()
    {
        _device = new GpuDevice();
        try
        {
            _capture = new DxgiCaptureSession(_device);
            _context = new GpuVisionContext(_device);
        }
        catch
        {
            _context?.Dispose();
            _capture?.Dispose();
            _device.Dispose();
            throw;
        }
    }

    public void Capture(CaptureMode mode)
    {
        // GDI 路径维持: 业务侧 12 处直调 GlobalScreenCapture.GetCurrentHandle (Phase 23 B#4 §52 白名单同层取像素).
        // Phase 23 B#4 派生候选: 12 处切 _vision.PixelAt 端口后, 本 GDI 截屏可删, 真端到端 GPU.
        GlobalScreenCapture.CaptureScreen(mode.Region);

        // GPU 路径: DXGI Acquire → CopyResource 到 _mainTex SRV-bind texture → ReleaseFrame.
        try
        {
            if (_capture.TryAcquireFrame(out Texture2D? frame))
            {
                try
                {
                    _context.UploadMainTexture(frame!);
                    _hasUploadedFrame = true;
                }
                finally
                {
                    frame?.Dispose();
                    _capture.ReleaseFrame();
                }
            }
            // TryAcquireFrame 返 false = 桌面静止超时, 复用上次 _mainTex (_hasUploadedFrame 保持原状)
        }
        catch (InvalidOperationException)
        {
            // DXGI AccessLost: 本帧 skip, 下次 Capture 再试 (业务呈卡顿但不崩).
            // 生产场景升级: catch + Dispose+ReInit DxgiCaptureSession (C5 简单兜底).
        }
    }

    public Color PixelAt(ScreenPoint point)
    {
        // GDI fallback: 复用 GlobalScreenCapture.GetColor (与 RustVisionAdapter 同形态).
        // 端到端 GPU PixelAt 需 Map staging buffer 单点开销 (~1ms), 与现有 GDI 路径 (~µs) 反向, 不实装.
        return GlobalScreenCapture.GetColor(point.X, point.Y);
    }

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
        _context?.Dispose();
        _capture?.Dispose();
        _device?.Dispose();
    }
}
#endif
