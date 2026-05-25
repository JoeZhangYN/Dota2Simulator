#if DOTA2
using Dota2Simulator.GameAutomation.Domain;
using Dota2Simulator.GameAutomation.Domain.Combat;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 单个英雄的运行态聚合——组合四个子状态容器：技能槽、条件槽、攻击计时、切假腿。
/// 取代 Main.cs 原本四个独立 static 字段 _槽 / _条件集 / _攻击 + Item.cs 原 8 个切假腿 static。
/// Phase 18 V6a：删 ctor IScreenVision 参数——委托签名 () 无参后 ConditionSlotSet 不再依赖 vision，
/// 聚合本身回到纯领域无 port 依赖形态。
/// </summary>
public sealed class HeroAggregate
{
    /// <summary>七个技能槽（原 _槽）。</summary>
    public SkillSlotSet Skills { get; } = new();

    /// <summary>15 个条件槽 + 命石委托。Phase 18 V6a 删 ctor vision 依赖。</summary>
    public ConditionSlotSet Conditions { get; } = new();

    /// <summary>攻击计时与走 A 状态（原 _攻击）。</summary>
    public AttackProfile Attack { get; } = new();

    /// <summary>Phase 8 C1: 切假腿子聚合（原 Item.cs 8 个 static 字段：配置 + 假腿按键 + 6 bool flag）。</summary>
    public LegSwapState LegSwap { get; } = new();

    /// <summary>Phase 8 C3: 当前英雄技能数量（4/5/6）——取代 Skill._技能数量 static。</summary>
    public int SkillCount { get; set; }

    /// <summary>Phase 8 C3: 当前英雄持有阿哈利姆神杖——取代 Item._是否神杖 static。</summary>
    public bool HasAghanim { get; set; }

    /// <summary>Phase 8 C3: 当前英雄持有阿哈利姆魔晶——取代 Item._是否魔晶 static。</summary>
    public bool HasShard { get; set; }
}

#endif
