using System.Collections.Generic;
using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「屏幕视觉」的需求——截图、找图、取色。
/// 由 Vision BC 的 adapter 实现（Rust / findpoints）。
/// Phase 19A 终态：4 核心方法纯 Template 语义；Phase 18 V3/V4 引入的 2 个 ImageHandle [Obsolete] 重载已删 (SG +_Tpl emit 后业务全切).
/// </summary>
public interface IScreenVision
{
    /// <summary>按指定模式截取屏幕到内部缓冲。</summary>
    void Capture(CaptureMode mode);

    /// <summary>在指定屏幕区域内查找模板，返回首个命中或 Miss。Phase 18 V1 主力路径。区域坐标 = 1920×1080 桌面绝对坐标。</summary>
    FindResult Find(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>在指定屏幕区域内查找模板的所有命中位置。</summary>
    IReadOnlyList<ScreenPoint> FindAll(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>读取当前缓冲中指定屏幕坐标的颜色。</summary>
    Color PixelAt(ScreenPoint point);
}
