using System;
using System.Collections.Generic;
using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「屏幕视觉」的需求——截图、找图、取色。
/// 由 Vision BC 的 adapter 实现（Rust / findpoints）。
/// Phase 19A 终态：4 核心方法纯 Template 语义；Phase 18 V3/V4 引入的 2 个 ImageHandle [Obsolete] 重载已删 (SG +_Tpl emit 后业务全切).
/// Phase 25A C3: +<see cref="WithFrame"/> typestate frame scope (替业务侧 GetCurrentHandle + 多次 GetColor 形态; 复杂度下沉到 adapter 内部).
/// </summary>
public interface IScreenVision
{
    /// <summary>按指定模式截取屏幕到内部缓冲。</summary>
    void Capture(CaptureMode mode);

    /// <summary>在指定屏幕区域内查找模板，返回首个命中或 Miss。Phase 18 V1 主力路径。区域坐标 = 1920×1080 桌面绝对坐标。</summary>
    FindResult Find(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>在指定屏幕区域内查找模板的所有命中位置。</summary>
    IReadOnlyList<ScreenPoint> FindAll(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>
    /// 在同一屏幕区域内、**同一帧**批量查找一组模板，返回与 <paramref name="needles"/> 同序对齐的逐模板结果。
    /// 替代业务侧 <c>foreach(tpl) Find(tpl)</c> 样板：一次取帧喂全组（修多模板循环跨帧不一致），
    /// 底层可由 adapter 用单趟融合摊销 per-call 开销（business 不感知）。
    /// </summary>
    IReadOnlyList<FindResult> FindMany(IReadOnlyList<Template> needles, ScreenRegion region, MatchRate rate, Tolerance tolerance);

    /// <summary>读取当前缓冲中指定屏幕坐标的颜色。</summary>
    Color PixelAt(ScreenPoint point);

    /// <summary>
    /// Phase 25A C3: typestate frame scope — adapter 内部取帧 → 调 <paramref name="read"/> lambda → 自动释放. 业务侧 0 wire 句柄.
    /// 用于"同一帧多次 PixelAt"形态 (SkillEngine 检测点数组 / Strategy 多档颜色判断 等), 替原 <c>GlobalScreenCapture.GetCurrentHandle + 多次 ImageManager.GetColor(in 句柄, ...)</c>.
    /// 复杂度下沉: 消费方不再 wire 帧管理 (符合 L3 铁律 1d 「复杂度下沉到元工具」).
    /// </summary>
    T WithFrame<T>(Func<IScreenFrame, T> read);
}
