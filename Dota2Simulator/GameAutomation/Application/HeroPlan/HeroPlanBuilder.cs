// Phase 12 Chunk 3: HeroPlan 流式 Builder — OnKey().CastSkill().AfterX() 三阶段构造子句, LegSwap() 累加假腿条目, Done() 终结返回不可变 HeroPlan.
// 中间状态用 nullable pending field 表达 (非严格 typestate, runtime check); 5 个 AfterX/WhenReady 终结方法对应 SkillEngine.技能通用判断 5 个 magic mode (CastMode enum 1:1).
#if DOTA2

using System;
using System.Collections.Immutable;
using System.Windows.Forms;

namespace Dota2Simulator.GameAutomation.Application.HeroPlans;

/// <summary>
/// HeroPlan 流式 Builder.
///
/// 顺序: <c>HeroPlanBuilder.New().OnKey(K).CastSkill(K).AfterCast(...).OnKey(K)...LegSwap(K, b).Done()</c>.
/// 一个 OnKey-CastSkill-AfterX 三阶段累 1 子句, 顺序映射 ConditionSlotKey.C1..C9 (业务无序号意识).
/// </summary>
public sealed class HeroPlanBuilder
{
    private readonly ImmutableArray<HeroPlanClause>.Builder _clauses = ImmutableArray.CreateBuilder<HeroPlanClause>();
    private readonly ImmutableArray<LegSwapEntry>.Builder _legSwap = ImmutableArray.CreateBuilder<LegSwapEntry>();

    private Keys? _pendingTrigger;
    private Keys? _pendingSkill;

    private HeroPlanBuilder() { }

    public static HeroPlanBuilder New() => new();

    /// <summary>开一个新子句: 声明触发按键 (Form2 Hook_KeyDown 收到的键).</summary>
    public HeroPlanBuilder OnKey(Keys triggerKey)
    {
        if (_pendingTrigger is not null)
        {
            throw new InvalidOperationException($"OnKey({triggerKey}): 上一个 OnKey({_pendingTrigger}) 未终结 (缺 CastSkill + AfterX).");
        }
        _pendingTrigger = triggerKey;
        return this;
    }

    /// <summary>声明该触发按键释放的技能键 (一般同 trigger, 也可不同).</summary>
    public HeroPlanBuilder CastSkill(Keys skillKey)
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("CastSkill: 需先调 OnKey.");
        }
        _pendingSkill = skillKey;
        return this;
    }

    /// <summary>终结子句, 释放模式 = AfterEnterCD (magic 0 — 主动技能进入 CD 后接).</summary>
    public HeroPlanBuilder AfterEnterCD(bool continueAttack = true, Keys continueKey = Keys.A, int postDelayMs = 0)
        => FinishClause(CastMode.AfterEnterCD, continueAttack, continueKey, postDelayMs);

    /// <summary>终结子句, 释放模式 = AfterCast (magic 1 — 释放技能有抬手, 释放变色后接 A).</summary>
    public HeroPlanBuilder AfterCast(bool continueAttack = true, Keys continueKey = Keys.A, int postDelayMs = 0)
        => FinishClause(CastMode.AfterCast, continueAttack, continueKey, postDelayMs);

    /// <summary>终结子句, 释放模式 = WhenReady (magic 2 — CD 就绪自动按一次).</summary>
    public HeroPlanBuilder WhenReady()
        => FinishClause(CastMode.WhenReady, true, Keys.A, 0);

    /// <summary>终结子句, 释放模式 = AfterEnterCDLegOnly (magic 10 — 进入 CD 仅切回假腿).</summary>
    public HeroPlanBuilder AfterEnterCDLegOnly(bool continueAttack = true, Keys continueKey = Keys.A, int postDelayMs = 0)
        => FinishClause(CastMode.AfterEnterCDLegOnly, continueAttack, continueKey, postDelayMs);

    /// <summary>终结子句, 释放模式 = AfterCastLegOnly (magic 11 — 释放技能仅切回假腿).</summary>
    public HeroPlanBuilder AfterCastLegOnly(bool continueAttack = true, Keys continueKey = Keys.A, int postDelayMs = 0)
        => FinishClause(CastMode.AfterCastLegOnly, continueAttack, continueKey, postDelayMs);

    /// <summary>
    /// 终结子句, 仅占 ConditionSlot 不挂 Probe — 用于「按键置位 Active 但不挂技能释放委托」的边缘形态
    /// (影魔 R 键 / 历史死代码占位); CastSkill 步骤可跳过.
    /// </summary>
    public HeroPlanBuilder NoProbe()
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("NoProbe: 需先调 OnKey.");
        }

        _clauses.Add(new HeroPlanClause(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            SkillKey: Keys.None,
            Mode: CastMode.AfterEnterCD,  // sentinel — Apply 内按 SkillKey == Keys.None 跳过 Probe 注册.
            ContinueAttack: false,
            ContinueKey: Keys.None,
            PostDelayMs: 0));

        _pendingTrigger = null;
        _pendingSkill = null;
        return this;
    }

    private HeroPlanBuilder FinishClause(CastMode mode, bool continueAttack, Keys continueKey, int postDelayMs)
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException($"Finish({mode}): 需先调 OnKey.");
        }
        if (_pendingSkill is null)
        {
            throw new InvalidOperationException($"Finish({mode}): 需先调 CastSkill (OnKey={_pendingTrigger}).");
        }

        _clauses.Add(new HeroPlanClause(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            SkillKey: _pendingSkill.Value,
            Mode: mode,
            ContinueAttack: continueAttack,
            ContinueKey: continueKey,
            PostDelayMs: postDelayMs));

        _pendingTrigger = null;
        _pendingSkill = null;
        return this;
    }

    /// <summary>累加一个 LegSwap 条目 (按键 → alwaysSwap), 与子句独立.</summary>
    public HeroPlanBuilder LegSwap(Keys key, bool alwaysSwap)
    {
        _legSwap.Add(new LegSwapEntry(key, alwaysSwap));
        return this;
    }

    /// <summary>终结整个 Plan, 返回不可变 HeroPlan; 中间态 pending 未终止报错.</summary>
    public HeroPlan Done()
    {
        if (_pendingTrigger is not null || _pendingSkill is not null)
        {
            throw new InvalidOperationException(
                $"Done: pending 状态未终结 (OnKey={_pendingTrigger?.ToString() ?? "null"}, CastSkill={_pendingSkill?.ToString() ?? "null"}).");
        }
        return new HeroPlan(_clauses.ToImmutable(), _legSwap.ToImmutable());
    }
}

#endif
