using System.Collections.Generic;
using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「屏幕视觉」的需求——截图、找图、取色。
/// 由 Vision BC 的 adapter 实现（Rust / findpoints）。
/// </summary>
public interface IScreenVision
{
    /// <summary>按指定模式截取屏幕到内部缓冲。</summary>
    void Capture(CaptureMode mode);

    /// <summary>在当前缓冲中查找模板，返回首个命中或 Miss。</summary>
    FindResult Find(Template needle, MatchRate rate, Tolerance tolerance);

    /// <summary>在当前缓冲中查找模板的所有命中位置。</summary>
    IReadOnlyList<ScreenPoint> FindAll(Template needle, MatchRate rate, Tolerance tolerance);

    /// <summary>读取当前缓冲中指定屏幕坐标的颜色。</summary>
    Color PixelAt(ScreenPoint point);
}
