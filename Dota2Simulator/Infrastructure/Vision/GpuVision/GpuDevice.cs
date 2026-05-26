// Infrastructure/Vision/GpuVision/GpuDevice.cs
// epic Phase 24 C2/C3 共享前置依赖 — D3D11 Device + DeviceContext + Hardware/Warp fallback.
// 为 GpuVisionContext (C2 compute shader) 与 DxgiCaptureSession (C3 DXGI dup) 提供共享 Device,
// 保证两者 zero-copy 路径 (DXGI 输出 texture 直接喂 compute shader SRV).
#if GpuVision
using System;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace Dota2Simulator.Infrastructure.Vision.GpuVision;

/// <summary>
/// 共享 D3D11 Device wrapper.
///
/// GpuFusedVisionAdapter ctor 内创建一个 GpuDevice 实例, 注入给 GpuVisionContext 与 DxgiCaptureSession.
/// 端到端 GPU 零回传必须共享 Device (跨 Device 必走 staging memory 回 CPU + 再上传, 失去 zero-copy 优势).
///
/// fallback 策略:
/// 1. Hardware (FeatureLevel 11_1 → 11_0) — 优先, 真 GPU 加速
/// 2. Warp 软件光栅器 — 失败兜底, headless CI / 无独显环境
/// 3. 全失败 → 抛 InvalidOperationException, GpuFusedVisionAdapter ctor 早炸,
///    用户切回 csproj 默认 (RustVisionAdapter) 即恢复.
/// </summary>
public sealed class GpuDevice : IDisposable
{
    public Device Native { get; }
    public DeviceContext ImmediateContext { get; }
    public string Kind { get; }

    public GpuDevice()
    {
        Device? dev = null;
        string kind;
        try
        {
            dev = new Device(DriverType.Hardware, DeviceCreationFlags.None,
                FeatureLevel.Level_11_1, FeatureLevel.Level_11_0);
            kind = $"Hardware (FeatureLevel {dev.FeatureLevel})";
        }
        catch
        {
            dev?.Dispose();
            dev = null;
            kind = "Warp 软件光栅器 fallback";
        }

        if (dev is null)
        {
            try
            {
                dev = new Device(DriverType.Warp, DeviceCreationFlags.None,
                    FeatureLevel.Level_11_1, FeatureLevel.Level_11_0);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "GpuDevice: D3D11 device 创建失败 (Hardware + Warp 均不可用). " +
                    "用户切回 csproj 默认 (无 GpuVision define) → RustVisionAdapter 即恢复.", ex);
            }
        }

        Native = dev;
        ImmediateContext = dev.ImmediateContext;
        Kind = kind;
    }

    public void Dispose()
    {
        ImmediateContext?.Dispose();
        Native?.Dispose();
    }
}
#endif
