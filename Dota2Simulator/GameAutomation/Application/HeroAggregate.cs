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

    /// <summary>Phase 20D: 命石子聚合 — 拆出 ConditionSlotSet 内 special-case StoneProbe/StoneChoice 双字段 (polling 语义, 与 event-driven ConditionSlot 区分).</summary>
    public StoneState Stone { get; } = new();

    /// <summary>Phase 26 A1: 不应期子聚合 — 命名不应期 SSOT, 防"按键 atomic 段 + grace period" 期间其他 probe 误触 (切假腿中间态循环根治).</summary>
    public RefractoryState Refractory { get; } = new();

    /// <summary>Phase 26 D1: 延迟命令队列子聚合 — 命令入队条件满足才出队执行, 防"窗口期错失" (吹风/无敌期间命令丢失, buff 结束瞬间秒接).</summary>
    public DeferredQueue Deferred { get; } = new();

    /// <summary>Phase 8 C3: 当前英雄技能数量（4/5/6）——取代 Skill._技能数量 static。</summary>
    public int SkillCount { get; set; }

    /// <summary>Phase 8 C3: 当前英雄持有阿哈利姆神杖——取代 Item._是否神杖 static。</summary>
    public bool HasAghanim { get; set; }

    /// <summary>Phase 8 C3: 当前英雄持有阿哈利姆魔晶——取代 Item._是否魔晶 static。</summary>
    public bool HasShard { get; set; }
}

#endif
