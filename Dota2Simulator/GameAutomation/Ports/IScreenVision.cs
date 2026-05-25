using System;
using System.Collections.Generic;
using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「屏幕视觉」的需求——截图、找图、取色。
/// 由 Vision BC 的 adapter 实现（Rust / findpoints）。
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

    /// <summary>
    /// Phase 18 V3/V4 引入：业务侧用 <c>Dota2_Pictrue.X.Y</c> (ImageHandle) 直接传入跳过 Template→ImageHandle 反查。
    /// 端口仍泄漏 Vision 内部类型 ImageHandle 是技术债——SG 改造（同步生成 Template 同名静态属性）后切走，列入 Phase 19 候选。
    /// </summary>
    [Obsolete("SG 改造同步生成 Template 静态属性后切走；改用 Find(Template, ScreenRegion, MatchRate, Tolerance)。", error: false)]
    FindResult Find(ImageHandle needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>
    /// Phase 18 V4 引入：同 <see cref="Find(ImageHandle, ScreenRegion, MatchRate, Tolerance)"/>，用于 Silt BC RPG 模式 FindAll 场景。
    /// </summary>
    [Obsolete("SG 改造同步生成 Template 静态属性后切走；改用 FindAll(Template, ScreenRegion, MatchRate, Tolerance)。", error: false)]
    IReadOnlyList<ScreenPoint> FindAll(ImageHandle needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);
}
