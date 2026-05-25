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

/// <summary>聚合状态守卫 (按键触发时检查): 决定该 clause/setup 是否触发.</summary>
public enum AggGuard
{
    /// <summary>无守卫, 无条件触发 (默认).</summary>
    None,
    /// <summary>仅当 HeroAggregate.HasAghanim == true 才触发.</summary>
    HasAghanim,
    /// <summary>仅当 HeroAggregate.HasShard == true 才触发.</summary>
    HasShard,
}

/// <summary>单个技能行为子句 (按键 → 技能 + 释放模式 + 后续动作 + 可选 Guard).</summary>
public readonly record struct HeroPlanClause(
    VirtualKey TriggerKey,
    Keys SkillKey,
    CastMode Mode,
    bool ContinueAttack,
    Keys ContinueKey,
    int PostDelayMs,
    AggGuard Guard);

/// <summary>假腿配置条目 (按键 → alwaysSwap 标志, OnActivate 时一次性应用).</summary>
public readonly record struct LegSwapEntry(Keys Key, bool AlwaysSwap);

/// <summary>
/// 按键触发 + Guard 的副作用 (运行期, 非 OnActivate 一次性) — 当前仅支持 AdjustLegSwap.
/// 例: F1 键 + HasAghanim → LegSwap.修改配置(F, true) (TB 模板; 业务概念「神杖切假腿配置」).
/// </summary>
public readonly record struct SetupAction(
    VirtualKey TriggerKey,
    AggGuard Guard,
    SetupActionKind Kind,
    Keys ParamKey,
    bool ParamBool);

/// <summary>SetupAction 副作用种类.</summary>
public enum SetupActionKind
{
    /// <summary>调 LegSwap.配置.修改配置(ParamKey, ParamBool).</summary>
    AdjustLegSwap,
}

/// <summary>
/// 英雄技能行为表 — 由 <see cref="HeroPlanBuilder"/> 终结返回, 不可变.
/// 业务 Strategy 用 <c>private static readonly HeroPlan _plan = HeroPlanBuilder.New()...Done()</c> 声明,
/// <c>OnActivate</c> 调 <see cref="Apply"/>; <c>OnKeyAsync</c> 调 <see cref="DispatchAsync"/>.
/// </summary>
public sealed class HeroPlan
{
    private readonly ImmutableArray<HeroPlanClause> _clauses;
    private readonly ImmutableArray<LegSwapEntry> _legSwap;
    private readonly ImmutableArray<SetupAction> _setups;
    private readonly int? _repeatThreshold;

    internal HeroPlan(
        ImmutableArray<HeroPlanClause> clauses,
        ImmutableArray<LegSwapEntry> legSwap,
        ImmutableArray<SetupAction> setups,
        int? repeatThreshold)
    {
        if (clauses.Length > 9)
        {
            // C1..C9 是数字槽; 超过 9 个 clause 需扩到 Z/X/C/V/B/Space 字母槽, 当前 builder 仅占数字段.
            throw new InvalidOperationException($"HeroPlan: 子句数 {clauses.Length} 超出 C1..C9 数字槽容量; 字母槽 Z/X/C/V/B/Space 需后续扩展.");
        }
        _clauses = clauses;
        _legSwap = legSwap;
        _setups = setups;
        _repeatThreshold = repeatThreshold;
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

        // OnActivate 一次性 SkillEngine 配置: 沙王/天怒 等设按键重复执行间隔阈值的形态.
        if (_repeatThreshold.HasValue)
        {
            skill.重复按键执行间隔阈值 = _repeatThreshold.Value;
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

        // 1. 先跑 SetupAction (按键 + Guard → 副作用, 如 F1+HasAghanim → LegSwap.修改配置).
        foreach (SetupAction setup in _setups)
        {
            if (setup.TriggerKey == key && CheckGuard(setup.Guard, ctx))
            {
                switch (setup.Kind)
                {
                    case SetupActionKind.AdjustLegSwap:
                        ctx.Aggregate.LegSwap.配置.修改配置(setup.ParamKey, setup.ParamBool);
                        break;
                }
            }
        }

        // 2. 再跑 Clause 激活 (按键 → ConditionSlot.Active, 受 Guard 控制).
        for (int i = 0; i < _clauses.Length; i++)
        {
            if (_clauses[i].TriggerKey == key && CheckGuard(_clauses[i].Guard, ctx))
            {
                ctx.Aggregate.Conditions[(ConditionSlotKey)i].Active = true;
                return;
            }
        }
        // 未命中: no-op (等价旧手写 if/else 链最后无 else 默认行为).
    }

    private static bool CheckGuard(AggGuard guard, HeroContext ctx) => guard switch
    {
        AggGuard.None => true,
        AggGuard.HasAghanim => ctx.Aggregate.HasAghanim,
        AggGuard.HasShard => ctx.Aggregate.HasShard,
        _ => true,
    };
}

#endif
