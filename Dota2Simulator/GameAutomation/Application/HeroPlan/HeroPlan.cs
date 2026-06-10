// Phase 12 Chunk 3: 英雄技能行为表 — 不可变值对象, 由 HeroPlanBuilder 链式构造.
// 业务概念: 一组「按键 → 技能释放策略」子句 + 一组「按键 → 假腿配置」副作用; 取代 92 策略类
// 手写 OnActivate (Probe ??= helper + LegSwap.修改配置) + OnKeyAsync (if/else key 映射 ConditionSlot.Active = true) 同构样板.
#if DOTA2

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;  // ConditionDelegateBitmap
using Dota2Simulator.GameAutomation.Application.StepMachine;  // Phase 27A retry 2 S2: StepMachineRunner
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.StepMachine;  // Phase 27A retry 2 S2: StepMachineDefinition
using Dota2Simulator.Vision;  // Phase 27A retry 2 S2: ImageHandle

namespace Dota2Simulator.GameAutomation.Application.HeroPlans;

/// <summary>
/// Phase 22A: 标准化"主动技能 X 后续"调用形态 — 替代手写 CustomProbe + lambda 包装 SkillEngine 调用.
/// 业务侧 lambda 只写"释放后/CD 后/就绪后"自定义动作, DSL 自动包装为 ConditionDelegateBitmap.
/// </summary>
public enum SkillAfterMode
{
    /// <summary>无 — 走 enum Guard / CustomProbeFn / IsToggle 等其它路径.</summary>
    None = 0,
    /// <summary>主动技能进入 CD 后跑 customAction — 等价 <c>await _skill.主动技能进入CD后续(skillKey, customAction)</c>.</summary>
    EnterCD,
    /// <summary>主动技能释放后跑 customAction — 等价 <c>await _skill.主动技能释放后续(skillKey, customAction)</c>.</summary>
    Cast,
    /// <summary>主动技能 CD 就绪后跑 customAction — 等价 <c>await _skill.主动技能已就绪后续(skillKey, customAction)</c>.</summary>
    WhenReady,
    /// <summary>Phase 26 F1: 主动技能释放后替换图标 — 等价 <c>await _skill.释放技能后替换图标技能后续(skillKey, () =&gt; Skills.Step(slot), v =&gt; Skills.SetStep(slot, v))</c> (大牛 W / 伐木机 R 形态).</summary>
    CastReplaceIcon,
}

/// <summary>聚合状态守卫 (按键触发时检查): 决定该 clause/setup 是否触发.</summary>
public enum AggGuard
{
    /// <summary>无守卫, 无条件触发 (默认).</summary>
    None,
    /// <summary>仅当 HeroAggregate.HasAghanim == true 才触发.</summary>
    HasAghanim,
    /// <summary>仅当 HeroAggregate.HasShard == true 才触发.</summary>
    HasShard,
    /// <summary>Phase 19C: 反向 Guard — 仅当 HeroAggregate.HasAghanim == false 才触发.</summary>
    NotHasAghanim,
    /// <summary>Phase 19C: 反向 Guard — 仅当 HeroAggregate.HasShard == false 才触发.</summary>
    NotHasShard,
}

/// <summary>
/// 单个技能行为子句 (按键 → 技能 + 释放模式 + 后续动作 + 可选 Guard).
/// <para>当 <see cref="IsToggle"/> = true 时:</para>
/// <list type="bullet">
/// <item>DispatchAsync 内 Active = !Active (而非 = true), 触发 TTS 播报 <see cref="SpeakOn"/>/<see cref="SpeakOff"/>;</item>
/// <item>Apply 内 Probe 返回 Active (自循环), 而非 _skill.技能通用判断 直返 — 用于 D3/D4 toggle 循环技能形态 (小仙女/小强/瘟疫法师).</item>
/// </list>
/// <para>当 <see cref="CustomProbeFn"/> 非空时: Apply 内用之注册 ConditionSlot.Probe, 完全跳过 _skill.技能通用判断 模板.
/// 用于 ImageFinder / _skill.主动技能释放后续 lambda / 物品 + DOTA2释放CD就绪技能 等自定义形态 (巫医/哈斯卡/拍拍).</para>
/// </summary>
public readonly record struct HeroPlanClause(
    VirtualKey TriggerKey,
    Keys SkillKey,
    CastMode Mode,
    bool ContinueAttack,
    Keys ContinueKey,
    int PostDelayMs,
    AggGuard Guard,
    bool IsToggle = false,
    string? SpeakOn = null,
    string? SpeakOff = null,
    ConditionDelegateBitmap? CustomProbeFn = null,
    Action? PreActionSync = null,
    Func<Task>? PreActionAsync = null,
    KeyModifiers Modifiers = KeyModifiers.None,
    Action? PostActionSync = null,
    Func<Task>? PostActionAsync = null,
    // Phase 21A: 通用谓词 Guard — 与 enum Guard 互斥 (非 null 时优先). 用于 SkillStep / StoneChoice 等多值状态的 specialized Guard.
    Func<HeroContext, bool>? GuardPredicate = null,
    // Phase 22A: 主动技能 X 后续 DSL — AfterMode != None 时优先级最高 (Apply 内自动包装 SkillEngine 调用为 Probe).
    SkillAfterMode AfterMode = SkillAfterMode.None,
    Action? AfterCustomAction = null,
    // Phase 26 F1: ReplaceIcon DSL — AfterMode = CastReplaceIcon 时存 step SlotKey, Apply 内 wrap 为 SkillEngine.释放技能后替换图标技能后续 调用 (大牛 W / 伐木机 R 形态).
    Domain.Loop.SlotKey? AfterReplaceIconStepSlot = null,
    // Phase 26 A3: 不应期 DSL — RefractoryName 非空时 DispatchAsync 路径: clause 触发前 check IsRefractory(name) → 真则短路; 命中后 SetRefractory(name, ms).
    string? RefractoryName = null,
    int RefractoryMs = 0,
    // Phase 27A retry 2 S2 (2026-05-26): StepMachine wiring 核心 — 非空时 DispatchAsync 命中后调 Runner.ExecuteAsync(StepMachineDefinitions[refId]), Task.Run wrap fire-and-forget (业务 Strategy 不感知).
    string? StepMachineRefId = null);

/// <summary>
/// 假腿配置条目 (按键 → alwaysSwap 标志, OnActivate 时一次性应用).
/// Phase 20C: 加 <see cref="Attribute"/> 可选第三参 — null 时走 LegSwapState.修改配置 默认 "智力"; 非 null 时显式覆盖 (猴子 "力量").
/// </summary>
public readonly record struct LegSwapEntry(Keys Key, bool AlwaysSwap, string? Attribute = null);

/// <summary>
/// 按键触发 + Guard 的副作用 (运行期, 非 OnActivate 一次性) —— 支持:
/// (1) <see cref="SetupActionKind.AdjustLegSwap"/>: 调 LegSwap.配置.修改配置(ParamKey, ParamBool).
/// (2) <see cref="SetupActionKind.ExecuteAction"/>: 调 <see cref="CustomAction"/> lambda 任意副作用 (不挂 ConditionSlot).
/// 例 (Execute): D2 键 → SetMode(W, 1) + TTS.Speak (幻刺); W 键 → _item.根据图片使用物品(纷争) (黑鸟).
/// <para>Phase 16 C1b 扩: <see cref="ParamBoolProvider"/> 非空时 AdjustLegSwap 第二参为 ctx 谓词 (动态 HasShard);
/// <see cref="IsOnEveryKey"/> = true 时 setup 在每键命中前都跑 (无 TriggerKey 匹配, 仅 Guard 过滤). 火枪 OnEveryKey + 动态 ParamBool 形态.</para>
/// </summary>
public readonly record struct SetupAction(
    VirtualKey TriggerKey,
    AggGuard Guard,
    SetupActionKind Kind,
    Keys ParamKey,
    bool ParamBool,
    Action? CustomAction = null,
    Func<HeroContext, bool>? ParamBoolProvider = null,
    bool IsOnEveryKey = false,
    KeyModifiers Modifiers = KeyModifiers.None,
    ConditionSlotKey ParamConditionSlot = ConditionSlotKey.C1,
    string? ParamStringOn = null,
    string? ParamStringOff = null,
    // Phase 25A C1: SetupAction PreAction 注入 (海民 F: WhenStoneChoiceEq(1).Pre(释放 E).SetActive(C3) 形态; async 优先).
    Action? PreActionSync = null,
    Func<Task>? PreActionAsync = null,
    // Phase 25A C1: SetupAction 通用谓词 Guard — 与 enum Guard 互斥 (predicate 非空优先). 修旧 bug: 终结方法之前丢弃 _pendingGuardPredicate (海民 E WhenStoneChoiceEq.SetActive 形态需要).
    Func<HeroContext, bool>? GuardPredicate = null,
    // Phase 26 A3: 不应期 DSL (与 HeroPlanClause 对称) — RefractoryName 非空时 DispatchAsync 路径: setup 触发前 check IsRefractory(name) 短路; 命中后 SetRefractory(name, ms).
    string? RefractoryName = null,
    int RefractoryMs = 0,
    // Phase 28 C5: ToggleSkillModeTts 目标技能槽 (Skills.Mode toggle 目标; 文案复用 ParamStringOn/Off).
    Domain.Loop.SlotKey? ParamSkillSlot = null);

/// <summary>SetupAction 副作用种类.</summary>
public enum SetupActionKind
{
    /// <summary>调 LegSwap.配置.修改配置(ParamKey, ParamBool).</summary>
    AdjustLegSwap,
    /// <summary>调 <see cref="SetupAction.CustomAction"/> lambda — 任意副作用 (不挂 ConditionSlot).</summary>
    ExecuteAction,
    /// <summary>Phase 17: trigger key (通常 D2/D3/D4/D5 数字键) toggle 指定 ConditionSlot.Active + TTS 播报. 与 clause IsToggle 不同 — 这是 setup 形态, 不占 clause 槽, toggle 别的 ConditionSlot (e.g. D3 → C4 Active toggle).</summary>
    ToggleConditionSlot,
    /// <summary>Phase 22C: trigger key 直接 set 指定 ConditionSlot.Active = true (单方向, 非 toggle). 替代 8 处业务 .Execute(() => Conditions[X].Active = true) 同构 Execute lambda. ParamConditionSlot 指定目标槽.</summary>
    SetActive,
    /// <summary>Phase 28 C5: trigger key toggle 指定 Skills.Mode(SlotKey) + TTS 播报 (Mode==1?On:Off). 收口 17 处 D2/D3 .Execute(ToggleMode+TTS) 同构. ParamSkillSlot 目标技能槽, ParamStringOn/Off 文案.</summary>
    ToggleSkillModeTts,
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
    private readonly ImmutableArray<(ConditionSlotKey Slot, ConditionDelegateBitmap Probe)> _registeredProbes;
    private readonly ConditionDelegateBitmap? _stoneProbe;
    private readonly int? _repeatThreshold;
    // Phase 20C: OnActivate 一次性聚合配置 — 替代 4 hero override OnActivate 设 Attack.基础攻击前摇/间隔; 1 hero (军团) InitSkillStep(Global, -1).
    private readonly (double PreDelay, double Interval)? _attackTiming;
    private readonly ImmutableArray<(Domain.Loop.SlotKey Slot, int Value)> _initSkillSteps;
    // Phase 27A retry 2 S2: StepMachine 子定义注册表 (name → definition). Apply 时一次性注册 InitialStep, DispatchAsync 命中含 StepMachineRefId 的 clause 时触发 Runner.
    private readonly ImmutableDictionary<string, StepMachineDefinition> _stepMachineDefinitions;

    internal HeroPlan(
        ImmutableArray<HeroPlanClause> clauses,
        ImmutableArray<LegSwapEntry> legSwap,
        ImmutableArray<SetupAction> setups,
        ImmutableArray<(ConditionSlotKey, ConditionDelegateBitmap)> registeredProbes,
        ConditionDelegateBitmap? stoneProbe,
        int? repeatThreshold,
        (double PreDelay, double Interval)? attackTiming,
        ImmutableArray<(Domain.Loop.SlotKey, int)> initSkillSteps,
        ImmutableDictionary<string, StepMachineDefinition>? stepMachineDefinitions = null)
    {
        if (clauses.Length > 9)
        {
            // C1..C9 是数字槽; 超过 9 个 clause 需扩到 Z/X/C/V/B/Space 字母槽, 当前 builder 仅占数字段.
            throw new InvalidOperationException($"HeroPlan: 子句数 {clauses.Length} 超出 C1..C9 数字槽容量; 字母槽 Z/X/C/V/B/Space 需后续扩展.");
        }
        _clauses = clauses;
        _legSwap = legSwap;
        _setups = setups;
        _registeredProbes = registeredProbes;
        _stoneProbe = stoneProbe;
        _repeatThreshold = repeatThreshold;
        _attackTiming = attackTiming;
        _initSkillSteps = initSkillSteps;
        _stepMachineDefinitions = stepMachineDefinitions ?? ImmutableDictionary<string, StepMachineDefinition>.Empty;
    }

    /// <summary>Phase 27A retry 2 S2: StepMachine 子定义快照 (诊断 / sanity test 用; 不可变).</summary>
    public ImmutableDictionary<string, StepMachineDefinition> StepMachineDefinitions => _stepMachineDefinitions;

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

            // Phase 22A: AfterXDo DSL (.AfterEnterCDDo / .AfterCastDo / .WhenReadyDo) — 业务侧 lambda 只写自定义动作, DSL 包装 SkillEngine 调用为 Probe.
            if (clause.AfterMode != SkillAfterMode.None && clause.AfterCustomAction is not null)
            {
                Action body = clause.AfterCustomAction;
                Keys sk = clause.SkillKey;
                ConditionDelegateBitmap probe = clause.AfterMode switch
                {
                    SkillAfterMode.EnterCD => async () => await skill.主动技能进入CD后续(sk, body).ConfigureAwait(true),
                    SkillAfterMode.Cast => async () => await skill.主动技能释放后续(sk, body).ConfigureAwait(true),
                    SkillAfterMode.WhenReady => async () => await skill.主动技能已就绪后续(sk, body).ConfigureAwait(true),
                    _ => throw new InvalidOperationException($"未处理的 SkillAfterMode: {clause.AfterMode}"),
                };
                ctx.Aggregate.Conditions[slotKey].Probe ??= probe;
                continue;
            }

            // Phase 26 F1: AfterCastReplaceIcon DSL — 业务侧 0 lambda, DSL 包装 SkillEngine.释放技能后替换图标技能后续 调用为 Probe.
            if (clause.AfterMode == SkillAfterMode.CastReplaceIcon && clause.AfterReplaceIconStepSlot.HasValue)
            {
                Keys sk = clause.SkillKey;
                Domain.Loop.SlotKey stepSlot = clause.AfterReplaceIconStepSlot.Value;
                ConditionDelegateBitmap replaceIconProbe = async () =>
                    await skill.释放技能后替换图标技能后续(
                        sk,
                        () => ctx.Aggregate.Skills.Step(stepSlot),
                        v => ctx.Aggregate.Skills.SetStep(stepSlot, v)).ConfigureAwait(true);
                ctx.Aggregate.Conditions[slotKey].Probe ??= replaceIconProbe;
                continue;
            }

            // CustomProbe escape-hatch: 用户自定义 Probe (ImageFinder / 主动技能释放后续 lambda / 物品组合等).
            if (clause.CustomProbeFn is not null)
            {
                ctx.Aggregate.Conditions[slotKey].Probe ??= clause.CustomProbeFn;
                continue;
            }

            // Toggle 形态 Probe 自循环: 释放技能后返 Active (再次按下 toggle 关闭循环);
            // 非 toggle 形态 Probe 直返 _skill.技能通用判断 结果 (true 继续判断 / false 释放完成).
            if (clause.IsToggle)
            {
                int clauseIndex = i;  // 捕获给闭包, 避免循环变量陷阱.
                HeroPlanClause capturedClause = clause;
                ctx.Aggregate.Conditions[slotKey].Probe ??= async () =>
                {
                    await skill.技能通用判断(
                        capturedClause.SkillKey,
                        (int)capturedClause.Mode,
                        capturedClause.ContinueAttack,
                        capturedClause.ContinueKey,
                        capturedClause.PostDelayMs).ConfigureAwait(true);
                    return ctx.Aggregate.Conditions[(ConditionSlotKey)clauseIndex].Active;
                };
            }
            else
            {
                ctx.Aggregate.Conditions[slotKey].Probe ??= async () =>
                    await skill.技能通用判断(
                        clause.SkillKey,
                        (int)clause.Mode,
                        clause.ContinueAttack,
                        clause.ContinueKey,
                        clause.PostDelayMs).ConfigureAwait(true);
            }
        }

        foreach (LegSwapEntry leg in _legSwap)
        {
            if (leg.Attribute is null)
                ctx.Aggregate.LegSwap.配置.修改配置(leg.Key, leg.AlwaysSwap);
            else
                ctx.Aggregate.LegSwap.配置.修改配置(leg.Key, leg.AlwaysSwap, leg.Attribute);
        }

        // Phase 19C: RegisterProbe DSL — 注册到指定 ConditionSlot, 不占 clause 顺序索引. 替代 OnKey(Keys.None).CustomProbe(...) placeholder hack.
        foreach (var entry in _registeredProbes)
        {
            ctx.Aggregate.Conditions[entry.Slot].Probe ??= entry.Probe;
        }

        // Phase 19G-4: StoneProbe DSL — 命石 Probe 单字段注册 (与 ConditionSlot Probe 双轨, 海民/伐木机/骷髅王 命石业务).
        // Phase 20D: 迁 ctx.Aggregate.Conditions.StoneProbe → ctx.Aggregate.Stone.Probe (子聚合自治).
        if (_stoneProbe is not null)
        {
            ctx.Aggregate.Stone.Probe ??= _stoneProbe;
        }

        // OnActivate 一次性 SkillEngine 配置: 沙王/天怒 等设按键重复执行间隔阈值的形态.
        if (_repeatThreshold.HasValue)
        {
            skill.重复按键执行间隔阈值 = _repeatThreshold.Value;
        }

        // Phase 20C: OnActivate 一次性 Attack 计时配置 (4 hero: 小骷髅 / 小鱼人 / 戴泽 / 龙骑 等手动设基础攻击前摇/间隔的形态).
        if (_attackTiming.HasValue)
        {
            ctx.Aggregate.Attack.基础攻击前摇 = _attackTiming.Value.PreDelay;
            ctx.Aggregate.Attack.基础攻击间隔 = _attackTiming.Value.Interval;
        }

        // Phase 20C: OnActivate 一次性 SkillSlot Step 初始化 (军团 SlotKey.Global = -1; 业务需 Step 状态机起始值的形态).
        foreach (var (slot, value) in _initSkillSteps)
        {
            ctx.Aggregate.Skills.SetStep(slot, value);
        }

        // Phase 27A retry 2 S2: 一次性注册每 StepMachine 的 InitialStep 到 StepMachineState 子聚合.
        // 跨 hero 切换 HeroLoopHost.Reset 已 Clear; 本次 Apply 重置回各 machine 的初态.
        foreach (var kvp in _stepMachineDefinitions)
        {
            ctx.Aggregate.StepMachines.SetStep(kvp.Key, kvp.Value.InitialStep);
        }
    }

    /// <summary>
    /// 按键派发 — 先调 ItemEngine 通用逻辑 (假腿切换 / 技能数量同步等), 再按 TriggerKey 命中 clause 激活对应 ConditionSlot.
    /// 取代手写 OnKeyAsync 中 1 处 _item.根据按键判断技能释放前通用逻辑 + N 处 if/else (VirtualKey → ConditionSlotKey 映射).
    /// Phase 27A retry 2 S2 (2026-05-26): +skill + handle 参数 — 命中含 StepMachineRefId 的 clause 时构造 StepMachineRunner 并 Task.Run wrap fire-and-forget.
    /// </summary>
    public async Task DispatchAsync(KeyTrigger trigger, HeroContext ctx, ItemEngine item, SkillEngine? skill = null, ImageHandle handle = default)
    {
        if (ctx is null) throw new ArgumentNullException(nameof(ctx));
        if (item is null) throw new ArgumentNullException(nameof(item));

        VirtualKey key = trigger.Key;
        await item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        // 1. 先跑 SetupAction (按键 + Guard → 副作用, 如 F1+HasAghanim → LegSwap.修改配置 / D2 → SetMode + TTS).
        // Phase 16 C1b: IsOnEveryKey = true 时无 TriggerKey 匹配 (每键命中前都跑); ParamBoolProvider 非空时 AdjustLegSwap 第二参动态求值 (火枪 HasShard 形态).
        // Phase 16 C2: setup.Modifiers 严格等值比对 (default None == 裸键, OnEveryKey 形态 Modifiers 必须 None 也匹配 default trigger.Modifiers).
        foreach (SetupAction setup in _setups)
        {
            bool matchKey = setup.IsOnEveryKey || setup.TriggerKey == key;
            // OnEveryKey 形态忽略 Modifiers (每键都跑, 含修饰键场景); 否则 setup.Modifiers 与 trigger.Modifiers 严格等值.
            bool matchMod = setup.IsOnEveryKey || setup.Modifiers == trigger.Modifiers;
            // Phase 25A C1: 用 CheckGuardCombined 让 SetupAction 也支持 specialized GuardPredicate (海民 E WhenStoneChoiceEq.SetActive 形态生效).
            if (matchKey && matchMod && CheckGuardCombined(setup.Guard, setup.GuardPredicate, ctx))
            {
                // Phase 26 A3: setup 不应期短路 — RefractoryName 非空时 check IsRefractory 真则 skip 整 setup.
                if (setup.RefractoryName is not null && ctx.Aggregate.Refractory.IsRefractory(setup.RefractoryName))
                {
                    continue;
                }

                // Phase 25A C1: SetupAction PreAction (海民 F: WhenStoneChoiceEq(1).Pre(释放 E).SetActive(C3) 形态). async 优先.
                if (setup.PreActionAsync is not null)
                {
                    await setup.PreActionAsync!().ConfigureAwait(true);
                }
                else
                {
                    setup.PreActionSync?.Invoke();
                }

                switch (setup.Kind)
                {
                    case SetupActionKind.AdjustLegSwap:
                        bool boolVal = setup.ParamBoolProvider is not null ? setup.ParamBoolProvider(ctx) : setup.ParamBool;
                        ctx.Aggregate.LegSwap.配置.修改配置(setup.ParamKey, boolVal);
                        break;
                    case SetupActionKind.ExecuteAction:
                        setup.CustomAction?.Invoke();
                        break;
                    case SetupActionKind.ToggleConditionSlot:
                        bool newActive = !ctx.Aggregate.Conditions[setup.ParamConditionSlot].Active;
                        ctx.Aggregate.Conditions[setup.ParamConditionSlot].Active = newActive;
                        if (setup.ParamStringOn is not null && setup.ParamStringOff is not null)
                        {
                            Dota2Simulator.TTS.TTS.Speak(newActive ? setup.ParamStringOn : setup.ParamStringOff);
                        }
                        break;
                    case SetupActionKind.SetActive:
                        // Phase 22C: 直接 set Active = true (单方向, 不 toggle), 替原 Execute(() => Conditions[X].Active = true) 同构.
                        ctx.Aggregate.Conditions[setup.ParamConditionSlot].Active = true;
                        break;
                    case SetupActionKind.ToggleSkillModeTts:
                        // Phase 28 C5: toggle Skills.Mode + TTS 播报 (Mode==1 ? On : Off). 收口 17 处 D2/D3 ToggleMode+TTS 同构.
                        if (setup.ParamSkillSlot.HasValue)
                        {
                            ctx.Aggregate.Skills.ToggleMode(setup.ParamSkillSlot.Value);
                            if (setup.ParamStringOn is not null && setup.ParamStringOff is not null)
                            {
                                Dota2Simulator.TTS.TTS.Speak(
                                    ctx.Aggregate.Skills.Mode(setup.ParamSkillSlot.Value) == 1
                                        ? setup.ParamStringOn
                                        : setup.ParamStringOff);
                            }
                        }
                        break;
                }

                // Phase 26 A3: setup 命中后 SetRefractory — 防短期内同 setup 重入.
                if (setup.RefractoryName is not null && setup.RefractoryMs > 0)
                {
                    ctx.Aggregate.Refractory.SetRefractory(setup.RefractoryName, setup.RefractoryMs);
                }
            }
        }

        // 2. 再跑 Clause 激活 (按键 → ConditionSlot.Active, 受 Guard 控制).
        // Phase 16 C2: clause.Modifiers 与 trigger.Modifiers 严格等值比对; default None == 裸键, 兼容现有 33 文件零 Modifiers 形态.
        for (int i = 0; i < _clauses.Length; i++)
        {
            if (_clauses[i].TriggerKey == key && _clauses[i].Modifiers == trigger.Modifiers && CheckGuardCombined(_clauses[i].Guard, _clauses[i].GuardPredicate, ctx))
            {
                // Phase 26 A3: clause 不应期短路 — RefractoryName 非空时 check IsRefractory 真则 skip 整 clause.
                if (_clauses[i].RefractoryName is not null && ctx.Aggregate.Refractory.IsRefractory(_clauses[i].RefractoryName!))
                {
                    return;
                }

                // PreAction (Active 设置前的副作用 — 例: _input.Press(A) 后再 Active 释放技能).
                if (_clauses[i].PreActionAsync is not null)
                {
                    await _clauses[i].PreActionAsync!().ConfigureAwait(true);
                }
                else
                {
                    _clauses[i].PreActionSync?.Invoke();
                }

                ConditionSlotKey slot = (ConditionSlotKey)i;
                if (_clauses[i].IsToggle)
                {
                    bool newActive = !ctx.Aggregate.Conditions[slot].Active;
                    ctx.Aggregate.Conditions[slot].Active = newActive;
                    if (_clauses[i].SpeakOn is not null && _clauses[i].SpeakOff is not null)
                    {
                        Dota2Simulator.TTS.TTS.Speak(newActive ? _clauses[i].SpeakOn : _clauses[i].SpeakOff);
                    }
                }
                else
                {
                    ctx.Aggregate.Conditions[slot].Active = true;
                }

                // Phase 16 C3: PostAction (Active 设置后副作用 — 火猫 W: Task.Run(Delay+保持假腿)). async 优先.
                if (_clauses[i].PostActionAsync is not null)
                {
                    await _clauses[i].PostActionAsync!().ConfigureAwait(true);
                }
                else
                {
                    _clauses[i].PostActionSync?.Invoke();
                }

                // Phase 26 A3: clause 命中后 SetRefractory — 防短期内同 clause 重入.
                if (_clauses[i].RefractoryName is not null && _clauses[i].RefractoryMs > 0)
                {
                    ctx.Aggregate.Refractory.SetRefractory(_clauses[i].RefractoryName!, _clauses[i].RefractoryMs);
                }

                // Phase 27A retry 2 S2: StepMachine wiring hook — clause.StepMachineRefId 命中时调 Runner.ExecuteAsync, fire-and-forget wrap (业务 Strategy 不感知).
                // skill 为 null (业务未传) → 跳过 (向后兼容旧业务路径); definition 未找到 → 跳过 (defensive).
                if (_clauses[i].StepMachineRefId is { } refId
                    && skill is not null
                    && _stepMachineDefinitions.TryGetValue(refId, out StepMachineDefinition def))
                {
                    StepMachineRunner runner = new(
                        ctx.Aggregate.StepMachines,
                        ctx.Aggregate,
                        item,
                        skill,
                        handle);
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await runner.ExecuteAsync(def).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            // fire-and-forget 兜底; 主循环不应被 step machine 异常打断.
                            Dota2Simulator.Games.Common.Main_Logger.Error($"StepMachineRunner '{refId}' 异常: {ex.Message}");
                        }
                    });
                }
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
        AggGuard.NotHasAghanim => !ctx.Aggregate.HasAghanim,
        AggGuard.NotHasShard => !ctx.Aggregate.HasShard,
        _ => true,
    };

    /// <summary>
    /// Phase 21A: 综合 Guard 检查 — predicate 非空时优先调用 (specialized Guard 如 WhenStepEq/WhenStoneChoiceEq);
    /// predicate 为 null 时回退 enum Guard. 二者互斥, builder 终结 clause 时保证只一个非默认值.
    /// </summary>
    private static bool CheckGuardCombined(AggGuard guard, Func<HeroContext, bool>? predicate, HeroContext ctx)
        => predicate is not null ? predicate(ctx) : CheckGuard(guard, ctx);
}

#endif
