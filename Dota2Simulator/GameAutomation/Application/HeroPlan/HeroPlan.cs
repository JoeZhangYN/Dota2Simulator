// Phase 12 Chunk 3: 英雄技能行为表 — 不可变值对象, 由 HeroPlanBuilder 链式构造.
// 业务概念: 一组「按键 → 技能释放策略」子句 + 一组「按键 → 假腿配置」副作用; 取代 92 策略类
// 手写 OnActivate (Probe ??= helper + LegSwap.修改配置) + OnKeyAsync (if/else key 映射 ConditionSlot.Active = true) 同构样板.
#if DOTA2

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Domain.Actuation;

namespace Dota2Simulator.GameAutomation.Application.HeroPlans;

/// <summary>单个技能行为子句 (按键 → 技能 + 释放模式 + 后续动作).</summary>
public readonly record struct HeroPlanClause(
    VirtualKey TriggerKey,
    Keys SkillKey,
    CastMode Mode,
    bool ContinueAttack,
    Keys ContinueKey,
    int PostDelayMs);

/// <summary>假腿配置条目 (按键 → alwaysSwap 标志).</summary>
public readonly record struct LegSwapEntry(Keys Key, bool AlwaysSwap);

/// <summary>
/// 英雄技能行为表 — 由 <see cref="HeroPlanBuilder"/> 终结返回, 不可变.
/// 业务 Strategy 用 <c>private static readonly HeroPlan _plan = HeroPlanBuilder.New()...Done()</c> 声明,
/// <c>OnActivate</c> 调 <see cref="Apply"/>; <c>OnKeyAsync</c> 调 <see cref="DispatchAsync"/>.
/// </summary>
public sealed class HeroPlan
{
    private readonly ImmutableArray<HeroPlanClause> _clauses;
    private readonly ImmutableArray<LegSwapEntry> _legSwap;

    internal HeroPlan(ImmutableArray<HeroPlanClause> clauses, ImmutableArray<LegSwapEntry> legSwap)
    {
        if (clauses.Length > 9)
        {
            // C1..C9 是数字槽; 超过 9 个 clause 需扩到 Z/X/C/V/B/Space 字母槽, 当前 builder 仅占数字段.
            throw new InvalidOperationException($"HeroPlan: 子句数 {clauses.Length} 超出 C1..C9 数字槽容量; 字母槽 Z/X/C/V/B/Space 需后续扩展.");
        }
        _clauses = clauses;
        _legSwap = legSwap;
    }

    /// <summary>子句数 (用于诊断 / 测试).</summary>
    public int ClauseCount => _clauses.Length;

    /// <summary>
    /// 应用到聚合 — 注册每 clause 的 Probe 到 ConditionSlotKey.C{i+1}, 应用 LegSwap 配置.
    /// 取代手写 OnActivate 中 N 处 <c>_main._聚合.Conditions[Cn].Probe ??= helper</c> + LegSwap.修改配置 重复.
    /// </summary>
    public void Apply(HeroContext ctx, SkillEngine skill)
    {
        if (ctx is null) throw new ArgumentNullException(nameof(ctx));
        if (skill is null) throw new ArgumentNullException(nameof(skill));

        for (int i = 0; i < _clauses.Length; i++)
        {
            HeroPlanClause clause = _clauses[i];
            ConditionSlotKey slotKey = (ConditionSlotKey)i;  // C1=0, C2=1, ..., C9=8

            // NoProbe sentinel: 仅占槽, 不挂 Probe — 用于按键 Active 但无技能释放委托的边缘形态.
            if (clause.SkillKey == Keys.None)
            {
                continue;
            }

            ctx.Aggregate.Conditions[slotKey].Probe ??= async _ =>
                await skill.技能通用判断(
                    clause.SkillKey,
                    (int)clause.Mode,
                    clause.ContinueAttack,
                    clause.ContinueKey,
                    clause.PostDelayMs).ConfigureAwait(true);
        }

        foreach (LegSwapEntry leg in _legSwap)
        {
            ctx.Aggregate.LegSwap.配置.修改配置(leg.Key, leg.AlwaysSwap);
        }
    }

    /// <summary>
    /// 按键派发 — 先调 ItemEngine 通用逻辑 (假腿切换 / 技能数量同步等), 再按 TriggerKey 命中 clause 激活对应 ConditionSlot.
    /// 取代手写 OnKeyAsync 中 1 处 _item.根据按键判断技能释放前通用逻辑 + N 处 if/else (VirtualKey → ConditionSlotKey 映射).
    /// </summary>
    public async Task DispatchAsync(KeyTrigger trigger, HeroContext ctx, ItemEngine item)
    {
        if (ctx is null) throw new ArgumentNullException(nameof(ctx));
        if (item is null) throw new ArgumentNullException(nameof(item));

        VirtualKey key = trigger.Key;
        await item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        for (int i = 0; i < _clauses.Length; i++)
        {
            if (_clauses[i].TriggerKey == key)
            {
                ctx.Aggregate.Conditions[(ConditionSlotKey)i].Active = true;
                return;
            }
        }
        // 未命中: no-op (等价旧手写 if/else 链最后无 else 默认行为).
    }
}

#endif
