using Dota2Simulator.GameAutomation.Domain.Combat;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 单个英雄的运行态聚合——组合三个子状态容器：技能槽、条件槽、攻击计时。
/// 取代 Main.cs 原本三个独立 static 字段 _槽 / _条件集 / _攻击。
/// </summary>
public sealed class HeroAggregate
{
    /// <summary>七个技能槽（原 _槽）。</summary>
    public SkillSlotSet Skills { get; } = new();

    /// <summary>15 个条件槽 + 命石委托（原 _条件集）。</summary>
    public ConditionSlotSet Conditions { get; } = new();

    /// <summary>攻击计时与走 A 状态（原 _攻击）。</summary>
    public AttackProfile Attack { get; } = new();
}
