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

    /// <summary>在当前缓冲中查找模板，返回首个命中或 Miss。Phase 18 V5 计划真删此重载——业务侧应改用带 <see cref="ScreenRegion"/> 的重载，强制候选区域裁剪。</summary>
    FindResult Find(Template needle, MatchRate rate, Tolerance tolerance);

    /// <summary>在当前缓冲中查找模板的所有命中位置。Phase 18 V5 计划真删此重载——业务侧应改用带 <see cref="ScreenRegion"/> 的重载。</summary>
    IReadOnlyList<ScreenPoint> FindAll(Template needle, MatchRate rate, Tolerance tolerance);

    /// <summary>在指定屏幕区域内查找模板，返回首个命中或 Miss。Phase 18 V1 主力路径。区域坐标 = 1920×1080 桌面绝对坐标。</summary>
    FindResult Find(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>在指定屏幕区域内查找模板的所有命中位置。</summary>
    IReadOnlyList<ScreenPoint> FindAll(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>读取当前缓冲中指定屏幕坐标的颜色。</summary>
    Color PixelAt(ScreenPoint point);

    /// <summary>在指定屏幕区域内查找模板，命中返回 true。Phase 18 V5 计划真删——改用 <see cref="Find(Template, ScreenRegion, MatchRate, Tolerance)"/>。</summary>
    [Obsolete("Phase 18 V5 真删；改用 Find(Template, ScreenRegion, MatchRate, Tolerance) 拿 FindResult。", error: false)]
    bool FindInRegion(Template needle, ScreenRegion region, MatchRate rate);

    /// <summary>
    /// Phase 18 V3 临时妥协：业务侧 92 Strategy 用 <c>Dota2_Pictrue.Buff.X</c> (ImageHandle 类型) 不便切 Template。
    /// 与 <see cref="GetCurrentFrame"/> 同属 Vision 类型泄漏端口边界的临时形态，V6 委托链路重做 + SG 改造（生成 Template 同名静态属性）后统一删除。
    /// </summary>
    [Obsolete("Phase 18 V6 真删；改用 Find(Template, ScreenRegion, MatchRate, Tolerance) 配合 SG 生成的 Template 静态属性。", error: false)]
    FindResult Find(ImageHandle needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>
    /// 获取当前帧的 Vision 内部句柄，供 ConditionDelegateBitmap 委托链路使用。
    /// </summary>
    /// <remarks>
    /// Phase 6 临时妥协：ConditionDelegateBitmap 委托签名当前接收 <see cref="Vision.ImageHandle"/>，
    /// 让 Port 边界泄漏 Vision 内部类型。Phase 7+ 把委托签名改为接收 IScreenVision 自身后移除。
    /// </remarks>
    [Obsolete("Phase 7+ 改 ConditionDelegateBitmap 签名后移除；新代码应直接调 Capture / Find / FindInRegion。", error: false)]
    ImageHandle GetCurrentFrame();
}
