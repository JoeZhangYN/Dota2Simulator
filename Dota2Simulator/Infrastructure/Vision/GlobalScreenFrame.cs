using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Vision;

namespace Dota2Simulator.Infrastructure.Vision;

/// <summary>
/// Phase 25A C3: <see cref="IScreenFrame"/> 共享实现 — singleton 因 frame 是无状态 wrapper (PixelAt 直读 GlobalScreenCapture._tripleBuffer 单例);
/// RustVisionAdapter / GpuFusedVisionAdapter 的 <c>WithFrame</c> 都返此 instance, 与 PixelAt 端口同一底层 (行为等价, 同帧保证由 _tripleBuffer 单例提供).
/// </summary>
internal sealed class GlobalScreenFrame : IScreenFrame
{
    public static readonly GlobalScreenFrame Instance = new();

    private GlobalScreenFrame() { }

    public Color PixelAt(ScreenPoint point) => GlobalScreenCapture.GetColor(point.X, point.Y);
}
