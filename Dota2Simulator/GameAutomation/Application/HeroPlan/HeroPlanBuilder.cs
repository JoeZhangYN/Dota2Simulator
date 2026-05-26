// Phase 12 Chunk 3: HeroPlan 流式 Builder — OnKey().CastSkill().AfterX() 三阶段构造子句, LegSwap() 累加假腿条目, Done() 终结返回不可变 HeroPlan.
// 中间状态用 nullable pending field 表达 (非严格 typestate, runtime check); 5 个 AfterX/WhenReady 终结方法对应 SkillEngine.技能通用判断 5 个 magic mode (CastMode enum 1:1).
#if DOTA2

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;  // ConditionDelegateBitmap

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
    private readonly ImmutableArray<(ConditionSlotKey, ConditionDelegateBitmap)>.Builder _registeredProbes = ImmutableArray.CreateBuilder<(ConditionSlotKey, ConditionDelegateBitmap)>();
    private ConditionDelegateBitmap? _stoneProbe;
    // Phase 20C: 业务级即插即用抽象 — 替代 OnActivate 自定义 body 形态.
    private (double PreDelay, double Interval)? _attackTiming;
    private readonly ImmutableArray<(Domain.Loop.SlotKey, int)>.Builder _initSkillSteps = ImmutableArray.CreateBuilder<(Domain.Loop.SlotKey, int)>();

    private Keys? _pendingTrigger;
    private Keys? _pendingSkill;
    private AggGuard _pendingGuard = AggGuard.None;
    // Phase 21A: 通用谓词 Guard — 与 _pendingGuard enum 互斥 (predicate 非 null 优先). 用于 SkillStep / StoneChoice 等多值状态的 specialized Guard.
    private Func<HeroContext, bool>? _pendingGuardPredicate;
    private Action? _pendingPreActionSync;
    private Func<Task>? _pendingPreActionAsync;
    private int? _repeatThreshold;
    // Phase 16 C1b: OnEveryKey 入口 sentinel — 与 _pendingTrigger 二选一; setup-only 形态 (不能跟 clause).
    private bool _pendingIsOnEveryKey;
    // Phase 16 C2: KeyModifier 匹配 — OnKey(Keys, KeyModifiers) overload 写入此字段, 终结时注入 clause/setup.
    private Domain.Actuation.KeyModifiers _pendingModifiers = Domain.Actuation.KeyModifiers.None;
    private Domain.Actuation.KeyModifiers _lastClauseModifiers = Domain.Actuation.KeyModifiers.None;
    // Phase 16 C3: PostAction (Active 设置后的副作用, 与 Pre 对称) — Post/PostAsync 写入此字段, 终结时注入 clause.
    private Action? _pendingPostActionSync;
    private Func<Task>? _pendingPostActionAsync;

    // 缓存最后一个 FinishClause 的 TriggerKey/Guard, 供 AlsoAdjustLegSwap 追加共享的 SetupAction.
    private Keys? _lastClauseTrigger;
    private AggGuard _lastClauseGuard = AggGuard.None;

    private HeroPlanBuilder() { }

    public static HeroPlanBuilder New() => new();

    /// <summary>开一个新子句: 声明触发按键 (Form2 Hook_KeyDown 收到的键).</summary>
    public HeroPlanBuilder OnKey(Keys triggerKey)
    {
        if (_pendingTrigger is not null || _pendingIsOnEveryKey)
        {
            throw new InvalidOperationException($"OnKey({triggerKey}): 上一个 OnKey/OnEveryKey 未终结 (缺 CastSkill + AfterX, 或 SetupAction).");
        }
        _pendingTrigger = triggerKey;
        _pendingModifiers = Domain.Actuation.KeyModifiers.None;
        _pendingGuard = AggGuard.None;
        _pendingGuardPredicate = null;  // Phase 21A defensive reset
        return this;
    }

    /// <summary>
    /// Phase 16 C2: OnKey + Modifier 形态 — 触发键 + 修饰键组合 (Alt/Control/Shift) 严格匹配.
    /// 用于巫妖 W+Alt / 墨客 E+Alt / 帕克 W+Ctrl / 光法 E+Alt / 血魔 Q+Alt 形态.
    /// 默认 None 等价无修饰 (与裸 <see cref="OnKey(Keys)"/> 完全等价).
    /// </summary>
    public HeroPlanBuilder OnKey(Keys triggerKey, Domain.Actuation.KeyModifiers modifiers)
    {
        if (_pendingTrigger is not null || _pendingIsOnEveryKey)
        {
            throw new InvalidOperationException($"OnKey({triggerKey}, {modifiers}): 上一个 OnKey/OnEveryKey 未终结.");
        }
        _pendingTrigger = triggerKey;
        _pendingModifiers = modifiers;
        _pendingGuard = AggGuard.None;
        _pendingGuardPredicate = null;  // Phase 21A defensive reset
        return this;
    }

    /// <summary>
    /// Phase 16 C1b: OnEveryKey 入口 — 无 trigger key 的 setup-only 形态.
    /// 后续必须接 <see cref="AdjustLegSwap"/> / <see cref="AdjustLegSwapDynamic"/> / <see cref="Execute"/> (setup), 不能接 <see cref="CastSkill"/> + AfterX 等 clause 终结.
    /// 用途: 火枪 OnKeyAsync 入口每键无条件跑 `LegSwap.修改配置(D, HasShard)` — 动态第二参 + 无 trigger 匹配.
    /// </summary>
    public HeroPlanBuilder OnEveryKey()
    {
        if (_pendingTrigger is not null || _pendingIsOnEveryKey)
        {
            throw new InvalidOperationException($"OnEveryKey: 上一个 OnKey({_pendingTrigger?.ToString() ?? "OnEveryKey"}) 未终结.");
        }
        _pendingIsOnEveryKey = true;
        _pendingGuard = AggGuard.None;
        _pendingGuardPredicate = null;  // Phase 21A defensive reset
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
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("WhenHasAghanim: 需先调 OnKey 或 OnEveryKey.");
        }
        _pendingGuard = AggGuard.HasAghanim;
        return this;
    }

    /// <summary>该 clause/setup 仅当 HeroAggregate.HasShard==true 才触发.</summary>
    public HeroPlanBuilder WhenHasShard()
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("WhenHasShard: 需先调 OnKey 或 OnEveryKey.");
        }
        _pendingGuard = AggGuard.HasShard;
        return this;
    }

    /// <summary>Phase 19C: 反向 Guard — 该 clause/setup 仅当 HeroAggregate.HasAghanim==false 才触发. 用于小精灵/马西 反向语义.</summary>
    public HeroPlanBuilder WhenNotHasAghanim()
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("WhenNotHasAghanim: 需先调 OnKey 或 OnEveryKey.");
        }
        _pendingGuard = AggGuard.NotHasAghanim;
        return this;
    }

    /// <summary>Phase 19C: 反向 Guard — 该 clause/setup 仅当 HeroAggregate.HasShard==false 才触发.</summary>
    public HeroPlanBuilder WhenNotHasShard()
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("WhenNotHasShard: 需先调 OnKey 或 OnEveryKey.");
        }
        _pendingGuard = AggGuard.NotHasShard;
        return this;
    }

    /// <summary>
    /// Phase 21A: specialized Guard — 该 clause 仅当指定 SkillSlot.Step == 指定值时触发.
    /// 替原 override OnKeyAsync 短路形态 (船长 E 键 Step(E)==1 跳过 dispatch).
    /// </summary>
    public HeroPlanBuilder WhenStepEq(Domain.Loop.SlotKey slot, int value)
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("WhenStepEq: 需先调 OnKey 或 OnEveryKey.");
        }
        _pendingGuardPredicate = (HeroContext ctx) => ctx.Aggregate.Skills.Step(slot) == value;
        return this;
    }

    /// <summary>Phase 21A: specialized Guard — 反向 — 该 clause 仅当指定 SkillSlot.Step != 指定值时触发.</summary>
    public HeroPlanBuilder WhenStepNotEq(Domain.Loop.SlotKey slot, int value)
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("WhenStepNotEq: 需先调 OnKey 或 OnEveryKey.");
        }
        _pendingGuardPredicate = ctx => ctx.Aggregate.Skills.Step(slot) != value;
        return this;
    }

    /// <summary>
    /// Phase 21A: specialized Guard — 该 clause 仅当 StoneState.Choice == 指定值时触发.
    /// 替原 override OnKeyAsync 短路形态 (伐木机 D 键 Stone.Choice != 2 跳过 dispatch).
    /// </summary>
    public HeroPlanBuilder WhenStoneChoiceEq(int value)
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("WhenStoneChoiceEq: 需先调 OnKey 或 OnEveryKey.");
        }
        _pendingGuardPredicate = ctx => ctx.Aggregate.Stone.Choice == value;
        return this;
    }

    /// <summary>Phase 21A: specialized Guard — 反向 — 该 clause 仅当 StoneState.Choice != 指定值时触发.</summary>
    public HeroPlanBuilder WhenStoneChoiceNotEq(int value)
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("WhenStoneChoiceNotEq: 需先调 OnKey 或 OnEveryKey.");
        }
        _pendingGuardPredicate = ctx => ctx.Aggregate.Stone.Choice != value;
        return this;
    }

    /// <summary>
    /// 在 OnKey 链中间插入 PreAction (clause Active 设置前调用), 同步形态.
    /// 例: <c>.OnKey(W).Pre(() => _input.Press(VirtualKey.From(Keys.A))).CastSkill(W).AfterEnterCD()</c>
    /// 表示按 W 时, 先 Press A (PreAction) 再激活 C2 释放 W 技能 (大牛/幻刺/黑鸟 模板).
    /// </summary>
    public HeroPlanBuilder Pre(Action action)
    {
        if (_pendingTrigger is null) throw new InvalidOperationException("Pre: 需先调 OnKey.");
        if (action is null) throw new ArgumentNullException(nameof(action));
        _pendingPreActionSync = action;
        return this;
    }

    /// <summary>
    /// 在 OnKey 链中间插入 PreAction (clause Active 设置前 await), 异步形态.
    /// 例: <c>.OnKey(R).PreAsync(async () => await 大招前纷争()).CastSkill(R).AfterCast()</c>
    /// 表示按 R 时, 先 await 大招前纷争 物品组合, 再激活 clause (莱恩模板).
    /// </summary>
    public HeroPlanBuilder PreAsync(Func<Task> asyncAction)
    {
        if (_pendingTrigger is null) throw new InvalidOperationException("PreAsync: 需先调 OnKey.");
        if (asyncAction is null) throw new ArgumentNullException(nameof(asyncAction));
        _pendingPreActionAsync = asyncAction;
        return this;
    }

    /// <summary>
    /// Phase 16 C3: 在 OnKey 链中间插入 PostAction (clause Active 设置后调用), 同步形态. 与 Pre 对称.
    /// 例: <c>.OnKey(W).Post(() => _input.Press(A)).CastSkill(W).AfterEnterCD()</c> (假设 Active 后立即按 A).
    /// </summary>
    public HeroPlanBuilder Post(Action action)
    {
        if (_pendingTrigger is null) throw new InvalidOperationException("Post: 需先调 OnKey.");
        if (action is null) throw new ArgumentNullException(nameof(action));
        _pendingPostActionSync = action;
        return this;
    }

    /// <summary>
    /// Phase 16 C3: 在 OnKey 链中间插入 PostAction (clause Active 设置后 await), 异步形态. 与 PreAsync 对称.
    /// 例: <c>.OnKey(W).PostAsync(async () => await Task.Run(() => { Delay(330); _item.要求保持假腿(); })).CustomProbe(...)</c>
    /// 表示按 W 时, 先 Active=true 释放技能, 再 await 330ms Delay + 保持假腿 (火猫无影拳模板).
    /// </summary>
    public HeroPlanBuilder PostAsync(Func<Task> asyncAction)
    {
        if (_pendingTrigger is null) throw new InvalidOperationException("PostAsync: 需先调 OnKey.");
        if (asyncAction is null) throw new ArgumentNullException(nameof(asyncAction));
        _pendingPostActionAsync = asyncAction;
        return this;
    }

    /// <summary>
    /// Phase 26 B3 (2026-05-26): clause 强制 CommandAcked — chain 直接修饰刚终结的 clause (与 AlsoAdjustLegSwap 同形态).
    /// 业务消费者 (Engine wrap) 决定 Ack 语义; B2 已实现 ItemEngine 物品按键 fire-and-forget Ack, B3 chain 在 DSL 层声明意图供未来 Engine 路径扩展消费.
    /// 例: <c>.OnKey(R).CastSkill(R).AfterCast().RequireAck()</c>
    /// </summary>
    public HeroPlanBuilder RequireAck()
    {
        if (_clauses.Count == 0)
        {
            throw new InvalidOperationException("RequireAck: 需先终结一个 clause (.AfterCast/.AfterEnterCD/.WhenReady/.AfterEnterCDDo/.AfterCastDo/.WhenReadyDo/.AfterCastReplaceIcon).");
        }
        int lastIdx = _clauses.Count - 1;
        _clauses[lastIdx] = _clauses[lastIdx] with { RequireAck = true };
        return this;
    }

    /// <summary>
    /// Phase 26 D3 (2026-05-26): clause 延迟入队 — chain 直接修饰刚终结的 clause. 业务消费者 (Engine 路径 / Apply 路径扩展) 决定入队语义.
    /// 用于"吹风秒接跳刀"形态: <c>.OnKey(W).CastSkill(跳刀).AfterEnterCD().QueueWhen(ctx => !ctx.Aggregate.SomeBuff.IsControlled)</c>
    /// (当前 Apply 路径默认行为 = 直发; QueueWhen 字段写入 clause 后由后续 Engine 路径扩展消费).
    /// </summary>
    public HeroPlanBuilder QueueWhen(Func<HeroContext, bool> condition)
    {
        if (_clauses.Count == 0)
        {
            throw new InvalidOperationException("QueueWhen: 需先终结一个 clause.");
        }
        if (condition is null) throw new ArgumentNullException(nameof(condition));
        int lastIdx = _clauses.Count - 1;
        _clauses[lastIdx] = _clauses[lastIdx] with { QueueWhen = condition };
        return this;
    }

    /// <summary>
    /// 终结 OnKey 链为 ExecuteAction SetupAction — 任意 lambda 副作用 (不挂 ConditionSlot, 不挂 Probe).
    /// 例: <c>.OnKey(D2).Execute(() => { _aggregate.Skills.SetMode(W, 1); TTS.Speak("..."); })</c>
    /// 用于 D2 SetMode + TTS 形态 (幻刺) / W 直接 _item 副作用 (黑鸟).
    /// </summary>
    public HeroPlanBuilder Execute(Action action)
    {
        if (_pendingTrigger is null) throw new InvalidOperationException("Execute: 需先调 OnKey.");
        if (action is null) throw new ArgumentNullException(nameof(action));

        _setups.Add(new SetupAction(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            Guard: _pendingGuard,
            Kind: SetupActionKind.ExecuteAction,
            ParamKey: Keys.None,
            ParamBool: false,
            CustomAction: action,
            Modifiers: _pendingModifiers,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            GuardPredicate: _pendingGuardPredicate));
        ResetPending();
        return this;
    }

    /// <summary>
    /// Phase 22C: 终结 OnKey 链为 SetActive 形态 — 直接 set 指定 ConditionSlot.Active = true.
    /// 替代 8 处业务 <c>.OnKey(K, modifiers).Execute(() => _main._聚合.Conditions[X].Active = true)</c> 同构 Execute lambda.
    /// 业务侧形态: <c>.OnKey(Keys.W, KeyModifiers.Alt).SetActive(ConditionSlotKey.C2)</c>
    /// </summary>
    public HeroPlanBuilder SetActive(ConditionSlotKey slot)
    {
        if (_pendingTrigger is null) throw new InvalidOperationException("SetActive: 需先调 OnKey.");

        _setups.Add(new SetupAction(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            Guard: _pendingGuard,
            Kind: SetupActionKind.SetActive,
            ParamKey: Keys.None,
            ParamBool: false,
            Modifiers: _pendingModifiers,
            ParamConditionSlot: slot,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            GuardPredicate: _pendingGuardPredicate));
        ResetPending();
        return this;
    }

    /// <summary>
    /// 终结 OnKey/OnEveryKey 链为 SetupAction (不挂 Probe, 仅副作用) — 当前 Kind = AdjustLegSwap.
    /// 例: <c>.OnKey(F1).WhenHasAghanim().AdjustLegSwap(F, true)</c> 表示按 F1 + 持神杖时改 LegSwap.修改配置(F, true).
    /// </summary>
    public HeroPlanBuilder AdjustLegSwap(Keys paramKey, bool paramBool)
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("AdjustLegSwap: 需先调 OnKey 或 OnEveryKey.");
        }
        _setups.Add(new SetupAction(
            TriggerKey: _pendingIsOnEveryKey ? Domain.Actuation.VirtualKey.From(Keys.None) : Domain.Actuation.VirtualKey.From(_pendingTrigger!.Value),
            Guard: _pendingGuard,
            Kind: SetupActionKind.AdjustLegSwap,
            ParamKey: paramKey,
            ParamBool: paramBool,
            IsOnEveryKey: _pendingIsOnEveryKey,
            Modifiers: _pendingModifiers,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            GuardPredicate: _pendingGuardPredicate));
        ResetPending();
        return this;
    }

    /// <summary>
    /// Phase 17: 异型 ToggleSlot — trigger key (通常 D2/D3/D4/D5) toggle 指定 ConditionSlot.Active + TTS 播报.
    /// 与 Phase 13 <see cref="ToggleSlot(Keys, string, string)"/> 不同 — 那是占 clause 槽 + 注册 Probe 自循环;
    /// 这是 setup 形态, 不占 clause 槽, toggle 已存在的别的 ConditionSlot (e.g. D3 → C4 toggle, C4 由别的 CustomProbe 占).
    /// 例: <c>.OnKey(D3).ToggleConditionSlot(ConditionSlotKey.C4, "开启循环查克拉", "关闭循环查克拉")</c> (光法).
    /// 莱恩 D4/D5 单方向 toggle 形态也用此 (toggle 后 TTS 不同方向标签).
    /// </summary>
    public HeroPlanBuilder ToggleConditionSlot(ConditionSlotKey slot, string speakOn, string speakOff)
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("ToggleConditionSlot: 需先调 OnKey (不支持 OnEveryKey 形态).");
        }
        _setups.Add(new SetupAction(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            Guard: _pendingGuard,
            Kind: SetupActionKind.ToggleConditionSlot,
            ParamKey: Keys.None,
            ParamBool: false,
            ParamConditionSlot: slot,
            ParamStringOn: speakOn,
            ParamStringOff: speakOff,
            Modifiers: _pendingModifiers,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            GuardPredicate: _pendingGuardPredicate));
        ResetPending();
        return this;
    }

    /// <summary>
    /// Phase 16 C1b: 动态 ParamBool 版 AdjustLegSwap — 第二参为 ctx 谓词, 每次派发时调用求值.
    /// 用于火枪 OnEveryKey + 动态 HasShard 第二参形态: <c>.OnEveryKey().AdjustLegSwapDynamic(D, ctx => ctx.Aggregate.HasShard)</c>.
    /// </summary>
    public HeroPlanBuilder AdjustLegSwapDynamic(Keys paramKey, Func<HeroContext, bool> paramBoolProvider)
    {
        if (_pendingTrigger is null && !_pendingIsOnEveryKey)
        {
            throw new InvalidOperationException("AdjustLegSwapDynamic: 需先调 OnKey 或 OnEveryKey.");
        }
        if (paramBoolProvider is null) throw new ArgumentNullException(nameof(paramBoolProvider));

        _setups.Add(new SetupAction(
            TriggerKey: _pendingIsOnEveryKey ? Domain.Actuation.VirtualKey.From(Keys.None) : Domain.Actuation.VirtualKey.From(_pendingTrigger!.Value),
            Guard: _pendingGuard,
            Kind: SetupActionKind.AdjustLegSwap,
            ParamKey: paramKey,
            ParamBool: false,  // sentinel, ignored when ParamBoolProvider 非空.
            ParamBoolProvider: paramBoolProvider,
            IsOnEveryKey: _pendingIsOnEveryKey,
            Modifiers: _pendingModifiers,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            GuardPredicate: _pendingGuardPredicate));
        ResetPending();
        return this;
    }

    private void ResetPending()
    {
        _pendingTrigger = null;
        _pendingSkill = null;
        _pendingGuard = AggGuard.None;
        _pendingGuardPredicate = null;  // Phase 21A
        _pendingPreActionSync = null;
        _pendingPreActionAsync = null;
        _pendingIsOnEveryKey = false;
        _pendingModifiers = Domain.Actuation.KeyModifiers.None;
        _pendingPostActionSync = null;
        _pendingPostActionAsync = null;
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
    /// Phase 22A: 终结子句为 "主动技能进入 CD 后跑自定义动作" — 替原 CustomProbe(async () => await _skill.主动技能进入CD后续(skillKey, lambda).ConfigureAwait(true)) 3 层壳模板.
    /// 业务侧 lambda 只写"自定义动作 body"; DSL 内部 Plan.Apply 时 wrap 为 ConditionDelegateBitmap 调 SkillEngine.主动技能进入CD后续.
    /// </summary>
    public HeroPlanBuilder AfterEnterCDDo(Action customAction)
        => FinishClauseAfter(SkillAfterMode.EnterCD, customAction);

    /// <summary>
    /// Phase 22A: 终结子句为 "主动技能释放后跑自定义动作" — 替原 CustomProbe(async () => await _skill.主动技能释放后续(skillKey, lambda).ConfigureAwait(true)) 3 层壳模板.
    /// </summary>
    public HeroPlanBuilder AfterCastDo(Action customAction)
        => FinishClauseAfter(SkillAfterMode.Cast, customAction);

    /// <summary>
    /// Phase 22A: 终结子句为 "主动技能 CD 就绪后跑自定义动作" — 替原 CustomProbe(async () => await _skill.主动技能已就绪后续(skillKey, lambda).ConfigureAwait(true)) 3 层壳模板.
    /// </summary>
    public HeroPlanBuilder WhenReadyDo(Action customAction)
        => FinishClauseAfter(SkillAfterMode.WhenReady, customAction);

    /// <summary>
    /// Phase 26 F1 (2026-05-26): 终结子句为 "主动技能释放后替换图标技能后续" — 替原 CustomProbe(async () => await _skill.释放技能后替换图标技能后续(skillKey, ()=>Step(slot), v=>SetStep(slot,v)).ConfigureAwait(true)) 4 层壳模板.
    /// 业务侧 0 lambda; DSL 内部按 stepSlot 自动 wire Skills.Step/SetStep accessor. 用于大牛 W + 伐木机 R 两 hero ReplaceIcon 形态.
    /// </summary>
    public HeroPlanBuilder AfterCastReplaceIcon(Domain.Loop.SlotKey stepSlot)
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("AfterCastReplaceIcon: 需先调 OnKey.");
        }
        if (_pendingSkill is null)
        {
            throw new InvalidOperationException($"AfterCastReplaceIcon: 需先调 CastSkill (OnKey={_pendingTrigger}).");
        }

        _clauses.Add(new HeroPlanClause(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            SkillKey: _pendingSkill.Value,
            Mode: CastMode.AfterCast,  // 占位, Apply 走 AfterMode=CastReplaceIcon 路径不消费 Mode.
            ContinueAttack: false,
            ContinueKey: Keys.None,
            PostDelayMs: 0,
            Guard: _pendingGuard,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            Modifiers: _pendingModifiers,
            PostActionSync: _pendingPostActionSync,
            PostActionAsync: _pendingPostActionAsync,
            GuardPredicate: _pendingGuardPredicate,
            AfterMode: SkillAfterMode.CastReplaceIcon,
            AfterReplaceIconStepSlot: stepSlot));

        _lastClauseTrigger = _pendingTrigger;
        _lastClauseGuard = _pendingGuard;
        _lastClauseModifiers = _pendingModifiers;
        ResetPending();
        return this;
    }

    private HeroPlanBuilder FinishClauseAfter(SkillAfterMode mode, Action customAction)
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException($"FinishClauseAfter({mode}): 需先调 OnKey.");
        }
        if (_pendingSkill is null)
        {
            throw new InvalidOperationException($"FinishClauseAfter({mode}): 需先调 CastSkill (OnKey={_pendingTrigger}).");
        }
        if (customAction is null) throw new ArgumentNullException(nameof(customAction));

        _clauses.Add(new HeroPlanClause(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            SkillKey: _pendingSkill.Value,
            Mode: CastMode.AfterEnterCD,  // 占位, 走 AfterMode 路径不消费 Mode.
            ContinueAttack: false,
            ContinueKey: Keys.None,
            PostDelayMs: 0,
            Guard: _pendingGuard,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            Modifiers: _pendingModifiers,
            PostActionSync: _pendingPostActionSync,
            PostActionAsync: _pendingPostActionAsync,
            GuardPredicate: _pendingGuardPredicate,
            AfterMode: mode,
            AfterCustomAction: customAction));

        _lastClauseTrigger = _pendingTrigger;
        _lastClauseGuard = _pendingGuard;
        _lastClauseModifiers = _pendingModifiers;
        ResetPending();
        return this;
    }

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
            SpeakOff: speakOff,
            Modifiers: _pendingModifiers));
        ResetPending();
        return this;
    }

    /// <summary>
    /// 终结子句为 CustomProbe escape-hatch: 用户提供自定义 <see cref="ConditionDelegateBitmap"/> Probe,
    /// 完全跳过 _skill.技能通用判断 模板. 用于 ImageFinder 检测 / _skill.主动技能释放后续 lambda /
    /// 物品组合 + DOTA2释放CD就绪技能 等无法表达为简单 mode + skillKey 的形态.
    /// <para>CastSkill 步骤可跳过 (CustomProbe 内部决定释放什么技能).</para>
    /// <para>注意: lambda 内若引用 Strategy 的 instance 字段 (_skill / _item / _input),
    /// Strategy 内 <c>_plan</c> 必须改为 instance lazy-init 字段 (而非 static readonly),
    /// 在 OnActivate 内首次构造.</para>
    /// </summary>
    public HeroPlanBuilder CustomProbe(ConditionDelegateBitmap probe)
    {
        if (_pendingTrigger is null)
        {
            throw new InvalidOperationException("CustomProbe: 需先调 OnKey.");
        }
        if (probe is null) throw new ArgumentNullException(nameof(probe));

        _clauses.Add(new HeroPlanClause(
            TriggerKey: Domain.Actuation.VirtualKey.From(_pendingTrigger.Value),
            SkillKey: Keys.None,  // CustomProbe 无 mode 模板, SkillKey 由 lambda 内部决定.
            Mode: CastMode.AfterEnterCD,
            ContinueAttack: false,
            ContinueKey: Keys.None,
            PostDelayMs: 0,
            Guard: _pendingGuard,
            CustomProbeFn: probe,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            Modifiers: _pendingModifiers,
            PostActionSync: _pendingPostActionSync,
            PostActionAsync: _pendingPostActionAsync,
            GuardPredicate: _pendingGuardPredicate));

        _lastClauseTrigger = _pendingTrigger;
        _lastClauseGuard = _pendingGuard;
        _lastClauseModifiers = _pendingModifiers;
        ResetPending();
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
            Guard: _pendingGuard,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            Modifiers: _pendingModifiers,
            PostActionSync: _pendingPostActionSync,
            PostActionAsync: _pendingPostActionAsync));

        _lastClauseTrigger = _pendingTrigger;
        _lastClauseGuard = _pendingGuard;
        _lastClauseModifiers = _pendingModifiers;
        ResetPending();
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
            Guard: _pendingGuard,
            PreActionSync: _pendingPreActionSync,
            PreActionAsync: _pendingPreActionAsync,
            Modifiers: _pendingModifiers,
            PostActionSync: _pendingPostActionSync,
            PostActionAsync: _pendingPostActionAsync,
            GuardPredicate: _pendingGuardPredicate));

        _lastClauseTrigger = _pendingTrigger;
        _lastClauseGuard = _pendingGuard;
        _lastClauseModifiers = _pendingModifiers;
        ResetPending();
        return this;
    }

    /// <summary>
    /// 在刚终结的 clause 上追加一个共享 TriggerKey+Guard 的 AdjustLegSwap SetupAction —— 同一次按键触发两个效果:
    /// (1) clause 激活 ConditionSlot.Active (释放技能), (2) SetupAction 修改 LegSwap 配置.
    /// 用于树精 D 键+HasAghanim 同时 LegSwap(D,true) + Active 形态.
    /// </summary>
    public HeroPlanBuilder AlsoAdjustLegSwap(Keys paramKey, bool paramBool)
    {
        if (_lastClauseTrigger is null)
        {
            throw new InvalidOperationException("AlsoAdjustLegSwap: 需先终结一个 clause (AfterEnterCD / AfterCast / WhenReady / NoProbe).");
        }

        _setups.Add(new SetupAction(
            TriggerKey: Domain.Actuation.VirtualKey.From(_lastClauseTrigger.Value),
            Guard: _lastClauseGuard,
            Kind: SetupActionKind.AdjustLegSwap,
            ParamKey: paramKey,
            ParamBool: paramBool,
            Modifiers: _lastClauseModifiers));
        return this;
    }

    /// <summary>累加一个 LegSwap 条目 (按键 → alwaysSwap), 与子句独立.</summary>
    public HeroPlanBuilder LegSwap(Keys key, bool alwaysSwap)
    {
        _legSwap.Add(new LegSwapEntry(key, alwaysSwap));
        return this;
    }

    /// <summary>
    /// Phase 20C: LegSwap 三参重载 — 显式指定假腿类型 (LegSwapState.修改配置 默认 "智力", 三参重载支持 "力量" / "敏捷").
    /// 用于猴子 W 力量假腿等非默认形态.
    /// </summary>
    public HeroPlanBuilder LegSwap(Keys key, bool alwaysSwap, string attribute)
    {
        if (attribute is null) throw new ArgumentNullException(nameof(attribute));
        _legSwap.Add(new LegSwapEntry(key, alwaysSwap, attribute));
        return this;
    }

    /// <summary>
    /// Phase 19C: 注册一个 Probe 到指定 ConditionSlot, 不占 clause 顺序索引.
    /// 替代 Phase 17 引入的 <c>.OnKey(Keys.None).CustomProbe(probe)</c> placeholder hack.
    /// 使用场景: D3 ToggleConditionSlot 引用的 C{n} 槽需要挂 Probe (大圣 C4 无限跳跃 / 莱恩 C5/C6 / 飞机 C5 / 小精灵 C2/C3 / 等),
    /// 既往用 placeholder trigger=Keys.None 占 clause 槽, 现在直接 RegisterProbe(slot, probe) 显式注册.
    /// </summary>
    public HeroPlanBuilder RegisterProbe(ConditionSlotKey slot, ConditionDelegateBitmap probe)
    {
        if (probe is null) throw new ArgumentNullException(nameof(probe));
        _registeredProbes.Add((slot, probe));
        return this;
    }

    /// <summary>
    /// Phase 19G-4: 注册命石 Probe 到 <see cref="ConditionSlotSet.StoneProbe"/> 单字段.
    /// 与 ConditionSlot Probe 不同 — StoneProbe 是命石业务专用单字段 (海民 / 伐木机 / 骷髅王 命石选择检测),
    /// 业务侧 Probe 内通常设 StoneChoice + 自清 (StoneProbe = null) 后返回 false 一次性执行.
    /// </summary>
    public HeroPlanBuilder RegisterStoneProbe(ConditionDelegateBitmap probe)
    {
        if (probe is null) throw new ArgumentNullException(nameof(probe));
        _stoneProbe = probe;
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

    /// <summary>
    /// Phase 20C: OnActivate 一次性 Attack 计时配置 — 替代 4 hero (小骷髅 0.4/1.7 / 小鱼人 0.5/1.7 / 戴泽 0.3/1.7 / 龙骑 0.5/1.6 等) override OnActivate 内手动设 _main._聚合.Attack.基础攻击前摇/间隔 的形态.
    /// Plan.Apply 时一次性写入 ctx.Aggregate.Attack.基础攻击前摇/间隔.
    /// </summary>
    public HeroPlanBuilder AttackTiming(double preDelay, double interval)
    {
        _attackTiming = (preDelay, interval);
        return this;
    }

    /// <summary>
    /// Phase 20C: OnActivate 一次性 SkillSlot Step 初始化 — 替代 1 hero (军团 SlotKey.Global = -1) override OnActivate 内 Skills.SetStep 的形态.
    /// 多次调用累加 (不同 slot 不同初值); Plan.Apply 时按声明顺序一次性写入 ctx.Aggregate.Skills.SetStep.
    /// </summary>
    public HeroPlanBuilder InitSkillStep(Domain.Loop.SlotKey slot, int value)
    {
        _initSkillSteps.Add((slot, value));
        return this;
    }

    /// <summary>终结整个 Plan, 返回不可变 HeroPlan; 中间态 pending 未终止报错.</summary>
    public HeroPlan Done()
    {
        if (_pendingTrigger is not null || _pendingSkill is not null || _pendingIsOnEveryKey)
        {
            throw new InvalidOperationException(
                $"Done: pending 状态未终结 (OnKey={_pendingTrigger?.ToString() ?? "null"}, CastSkill={_pendingSkill?.ToString() ?? "null"}, OnEveryKey={_pendingIsOnEveryKey}).");
        }
        return new HeroPlan(_clauses.ToImmutable(), _legSwap.ToImmutable(), _setups.ToImmutable(), _registeredProbes.ToImmutable(), _stoneProbe, _repeatThreshold, _attackTiming, _initSkillSteps.ToImmutable());
    }
}

#endif
