using System.Drawing;

namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// Phase 25A C3: typestate frame scope — 由 <see cref="Dota2Simulator.GameAutomation.Ports.IScreenVision.WithFrame"/> 提供;
/// 业务侧通过 lambda 内调 PixelAt 拿"同一帧"多个像素颜色, 不直接持 ImageHandle 句柄, 帧生命周期由 adapter 内部管理 (复杂度下沉).
/// </summary>
public interface IScreenFrame
{
    /// <summary>读取本 frame scope 内指定屏幕坐标的颜色 (同一帧多次调用保证同帧语义).</summary>
    Color PixelAt(ScreenPoint point);
}
