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
    private readonly ImmutableArray<SetupAction>.Builder _setups = ImmutableArray.CreateBuilder<SetupAction>();

    private Keys? _pendingTrigger;
    private Keys? _pendingSkill;
    private AggGuard _pendingGuard = AggGuard.None;
    private int? _repeatThreshold;

    private HeroPlanBuilder() { }

    public static HeroPlanBuilder New() => new();

    /// <summary>开一个新子句: 声明触发按键 (Form2 Hook_KeyDown 收到的键).</summary>
    public HeroPlanBuilder OnKey(Keys triggerKey)
    {
        if (_pendingTrigger is not null)
        {
            throw new InvalidOperationException($"OnKey({triggerKey}): 上一个 OnKey({_pendingTrigger}) 未终结 (缺 CastSkill + AfterX, 或 SetupAction).");
        }
        _pendingTrigger = triggerKey;
        _pendingGuard = AggGuard.None;
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

    /// <summary>该 clause/setup 仅当 HeroAggregate.HasAghanim==true 才触发.</summary>
    public HeroPlanBuilder WhenHasAghanim()
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("WhenHasAghanim: 需先调 OnKey.");
        }
        _pendingGuard = AggGuard.HasAghanim;
        return this;
    }

    /// <summary>该 clause/setup 仅当 HeroAggregate.HasShard==true 才触发.</summary>
    public HeroPlanBuilder WhenHasShard()
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("WhenHasShard: 需先调 OnKey.");
        }
        _pendingGuard = AggGuard.HasShard;
        return this;
    }

    /// <summary>
    /// 终结 OnKey 链为 SetupAction (不挂 Probe, 仅副作用) — 当前 Kind = AdjustLegSwap.
    /// 例: <c>.OnKey(F1).WhenHasAghanim().AdjustLegSwap(F, true)</c> 表示按 F1 + 持神杖时改 LegSwap.修改配置(F, true).
    /// </summary>
    public HeroPlanBuilder AdjustLegSwap(Keys paramKey, bool paramBool)
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("AdjustLegSwap: 需先调 OnKey.");
        }
        _setups.Add(new SetupAction(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            Guard: _pendingGuard,
            Kind: SetupActionKind.AdjustLegSwap,
            ParamKey: paramKey,
            ParamBool: paramBool));
        _pendingTrigger = null;
        _pendingSkill = null;
        _pendingGuard = AggGuard.None;
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
    /// 终结子句为 Toggle 形态: 触发键 = D3/D4 数字键 toggle ConditionSlot.Active = !Active + TTS 播报;
    /// Probe 自循环 (释放 <paramref name="skillKey"/> 技能 mode=2 后返 Active, 直到再次按下 toggle 退出).
    /// 用于小仙女 D3 (续暗影/不续暗影) / 小强 D3 (循环爆裂/终止循环) / 瘟疫法师 D3 (循环脉冲/终止循环) 等形态.
    /// </summary>
    public HeroPlanBuilder ToggleSlot(Keys skillKey, string speakOn, string speakOff)
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("ToggleSlot: 需先调 OnKey.");
        }

        _clauses.Add(new HeroPlanClause(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            SkillKey: skillKey,
            Mode: CastMode.WhenReady,
            ContinueAttack: false,
            ContinueKey: Keys.None,
            PostDelayMs: 0,
            Guard: _pendingGuard,
            IsToggle: true,
            SpeakOn: speakOn,
            SpeakOff: speakOff));

        _pendingTrigger = null;
        _pendingSkill = null;
        _pendingGuard = AggGuard.None;
        return this;
    }

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
            PostDelayMs: 0,
            Guard: _pendingGuard));

        _pendingTrigger = null;
        _pendingSkill = null;
        _pendingGuard = AggGuard.None;
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
            PostDelayMs: postDelayMs,
            Guard: _pendingGuard));

        _pendingTrigger = null;
        _pendingSkill = null;
        _pendingGuard = AggGuard.None;
        return this;
    }

    /// <summary>累加一个 LegSwap 条目 (按键 → alwaysSwap), 与子句独立.</summary>
    public HeroPlanBuilder LegSwap(Keys key, bool alwaysSwap)
    {
        _legSwap.Add(new LegSwapEntry(key, alwaysSwap));
        return this;
    }

    /// <summary>
    /// 设 SkillEngine 按键重复执行间隔阈值 (毫秒); Plan.Apply 时一次性写入 _skill.重复按键执行间隔阈值.
    /// 用于沙王/天怒 等 OnActivate 设阈值的形态.
    /// </summary>
    public HeroPlanBuilder RepeatThreshold(int milliseconds)
    {
        _repeatThreshold = milliseconds;
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
        return new HeroPlan(_clauses.ToImmutable(), _legSwap.ToImmutable(), _setups.ToImmutable(), _repeatThreshold);
    }
}

#endif
