// Infrastructure/Vision/GpuVision/DxgiCaptureSession.cs
// epic Phase 24A C3 — Desktop Duplication 桌面截屏直接产 GPU Texture2D.
//
// 路径定位:
//   GDI 路径 (RustVisionAdapter 用): Vision/Capture/GlobalScreenCapture.cs → Bitmap → byte[] → Rust SIMD
//   GPU 路径 (GpuFusedVisionAdapter C5 集成): DxgiCaptureSession → Texture2D → 直喂 compute shader SRV (零回传)
//
// 与 GpuDevice 协作:
//   - ctor 接 GpuDevice 注入, 不自己 new Device (跨 Device 必走 staging memory, 失 zero-copy)
//   - QueryInterface chain: D3D11.Device → DXGI.Device → DXGI.Adapter → DXGI.Output → Output1 → OutputDuplication
//
// 与 C2 GpuVisionContext / C5 GpuFusedVisionAdapter 的边界:
//   - 本文件只负责 "拿到一帧 Texture2D + 释放", 不做 compute shader / fence / dispatch
//   - C5 集成阶段由主 lead wire DxgiCaptureSession → GpuVisionContext SRV
#if GpuVision
using System;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using D3D11Device = SharpDX.Direct3D11.Device;
using DxgiDevice = SharpDX.DXGI.Device;
using DxgiResource = SharpDX.DXGI.Resource;

namespace Dota2Simulator.Infrastructure.Vision.GpuVision;

/// <summary>
/// DXGI Desktop Duplication session — 主显示器 (output 0) 截屏直产 D3D11 Texture2D.
///
/// 使用契约 (DXGI API 强制):
///   1. <see cref="TryAcquireFrame"/> 返 true 后, 调用方用完 frame 必须调 <see cref="ReleaseFrame"/>
///   2. 不 ReleaseFrame 直接再 Acquire → DXGI_ERROR_INVALID_CALL
///   3. timeout (DXGI_ERROR_WAIT_TIMEOUT) 是 "桌面无变化" 的正常情况, 非异常
///   4. AccessLost (DXGI_ERROR_ACCESS_LOST) 抛 InvalidOperationException →
///      上游 GpuFusedVisionAdapter (C5) catch 后 Dispose + 重建本 session
///
/// 已知 limitation (主 lead 注意, C5 集成前确认):
///   - 仅捕获 output index 0 (主显示器); 多 monitor 场景需扩 ctor 接 outputIndex 参数
///   - Windows 锁屏 / UAC 安全桌面 / 部分全屏独占 D3D 应用 → AccessLost (上游需重建容错)
///   - 跨用户会话 / 远程桌面 (RDP) 不保证可用
///   - 需与目标进程同等或更高权限 (Dota2 反作弊 / 管理员窗口 → 本进程也须管理员, 项目 csproj 已要求)
/// </summary>
public sealed class DxgiCaptureSession : IDisposable
{
    // DXGI HRESULT 常量 (SharpDX 未集中导出, 此处手写以保错误链路清晰)
    private const int DXGI_ERROR_WAIT_TIMEOUT = unchecked((int)0x887A0027);
    private const int DXGI_ERROR_ACCESS_LOST = unchecked((int)0x887A0026);

    private readonly DxgiDevice _dxgiDevice;
    private readonly Adapter _adapter;
    private readonly Output _output;
    private readonly Output1 _output1;
    private readonly OutputDuplication _outputDup;

    /// <summary>主显示器桌面像素宽 (DesktopBounds.Right - Left).</summary>
    public int DesktopWidth { get; }

    /// <summary>主显示器桌面像素高 (DesktopBounds.Bottom - Top).</summary>
    public int DesktopHeight { get; }

    public DxgiCaptureSession(GpuDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        DxgiDevice? dxgiDevice = null;
        Adapter? adapter = null;
        Output? output = null;
        Output1? output1 = null;
        OutputDuplication? outputDup = null;
        try
        {
            dxgiDevice = device.Native.QueryInterface<DxgiDevice>();
            adapter = dxgiDevice.GetParent<Adapter>();
            output = adapter.GetOutput(0);
            output1 = output.QueryInterface<Output1>();
            outputDup = output1.DuplicateOutput(device.Native);

            var bounds = output.Description.DesktopBounds;
            DesktopWidth = bounds.Right - bounds.Left;
            DesktopHeight = bounds.Bottom - bounds.Top;

            _dxgiDevice = dxgiDevice;
            _adapter = adapter;
            _output = output;
            _output1 = output1;
            _outputDup = outputDup;
        }
        catch
        {
            // ctor 中途任一步失败 → 释放已建好的 COM 对象 (按构造逆序), 透传原异常
            outputDup?.Dispose();
            output1?.Dispose();
            output?.Dispose();
            adapter?.Dispose();
            dxgiDevice?.Dispose();
            throw;
        }
    }

    /// <summary>
    /// 尝试获取下一帧桌面.
    /// </summary>
    /// <param name="frame">成功时含新帧 Texture2D (调用方负责 Dispose 该 texture, 或仅 ReleaseFrame 由 DXGI 回收 — 见 DXGI 文档).</param>
    /// <param name="timeoutMs">阻塞等待新帧最长毫秒数, 默认 50ms.</param>
    /// <returns>true=拿到新帧 (调用方必须用完调 ReleaseFrame); false=超时无新帧 (桌面静止, 非异常).</returns>
    /// <exception cref="InvalidOperationException">DXGI_ERROR_ACCESS_LOST — 上游应 Dispose + 重建.</exception>
    public bool TryAcquireFrame(out Texture2D? frame, int timeoutMs = 50)
    {
        frame = null;
        try
        {
            var result = _outputDup.TryAcquireNextFrame(timeoutMs, out _, out DxgiResource? resource);
            if (result.Failure)
            {
                if (result.Code == DXGI_ERROR_WAIT_TIMEOUT)
                {
                    return false;
                }
                if (result.Code == DXGI_ERROR_ACCESS_LOST)
                {
                    throw new InvalidOperationException(
                        "DXGI duplication access lost, GpuFusedVisionAdapter 应 ReInit");
                }
                // 其他失败码透传为 SharpDXException
                throw new SharpDXException(result);
            }

            if (resource is null)
            {
                return false;
            }

            try
            {
                frame = resource.QueryInterface<Texture2D>();
                return true;
            }
            finally
            {
                resource.Dispose();
            }
        }
        catch (SharpDXException ex) when (ex.ResultCode.Code == DXGI_ERROR_ACCESS_LOST)
        {
            throw new InvalidOperationException(
                "DXGI duplication access lost, GpuFusedVisionAdapter 应 ReInit", ex);
        }
    }

    /// <summary>
    /// 释放上次 TryAcquireFrame 拿到的帧. DXGI 契约: Acquire 返 true 后必须配对调用.
    /// </summary>
    public void ReleaseFrame()
    {
        _outputDup.ReleaseFrame();
    }

    public void Dispose()
    {
        // 按构造逆序释放; Device 自身由 GpuDevice 持, 不在此 Dispose
        _outputDup?.Dispose();
        _output1?.Dispose();
        _output?.Dispose();
        _adapter?.Dispose();
        _dxgiDevice?.Dispose();
    }
}
#endif
