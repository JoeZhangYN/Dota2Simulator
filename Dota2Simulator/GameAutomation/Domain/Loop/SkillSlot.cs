using System.Drawing;

namespace Dota2Simulator.GameAutomation.Domain.Loop;

/// <summary>
/// 单个技能槽的不可变状态：把原 Main.cs 四个并行 static 字段
/// （_全局时间X / _全局模式X / _全局步骤X / _指定地点X）聚为一个值对象。
///
/// 值不可变——更新经 <c>with</c> 表达式产生新值；可变的是持有它的 <see cref="SkillSlotSet"/>。
/// 这是「不可变值对象 + 可变聚合」，未回到已否决的位标志 / 连续内存状态机。
/// </summary>
public readonly record struct SkillSlot(long TimeMs, int Mode, int Step, Point TargetPoint);
