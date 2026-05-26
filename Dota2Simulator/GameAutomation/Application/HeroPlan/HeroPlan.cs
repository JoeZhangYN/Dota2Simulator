// Phase 12 Chunk 3: 英雄技能行为表 — 不可变值对象, 由 HeroPlanBuilder 链式构造.
// 业务概念: 一组「按键 → 技能释放策略」子句 + 一组「按键 → 假腿配置」副作用; 取代 92 策略类
// 手写 OnActivate (Probe ??= helper + LegSwap.修改配置) + OnKeyAsync (if/else key 映射 ConditionSlot.Active = true) 同构样板.
#if DOTA2

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;  // ConditionDelegateBitmap
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
    Func<Task>? PostActionAsync = null);

/// <summary>假腿配置条目 (按键 → alwaysSwap 标志, OnActivate 时一次性应用).</summary>
public readonly record struct LegSwapEntry(Keys Key, bool AlwaysSwap);

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
    string? ParamStringOff = null);

/// <summary>SetupAction 副作用种类.</summary>
public enum SetupActionKind
{
    /// <summary>调 LegSwap.配置.修改配置(ParamKey, ParamBool).</summary>
    AdjustLegSwap,
    /// <summary>调 <see cref="SetupAction.CustomAction"/> lambda — 任意副作用 (不挂 ConditionSlot).</summary>
    ExecuteAction,
    /// <summary>Phase 17: trigger key (通常 D2/D3/D4/D5 数字键) toggle 指定 ConditionSlot.Active + TTS 播报. 与 clause IsToggle 不同 — 这是 setup 形态, 不占 clause 槽, toggle 别的 ConditionSlot (e.g. D3 → C4 Active toggle).</summary>
    ToggleConditionSlot,
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

    internal HeroPlan(
        ImmutableArray<HeroPlanClause> clauses,
        ImmutableArray<LegSwapEntry> legSwap,
        ImmutableArray<SetupAction> setups,
        ImmutableArray<(ConditionSlotKey, ConditionDelegateBitmap)> registeredProbes,
        ConditionDelegateBitmap? stoneProbe,
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
        _registeredProbes = registeredProbes;
        _stoneProbe = stoneProbe;
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
            ctx.Aggregate.LegSwap.配置.修改配置(leg.Key, leg.AlwaysSwap);
        }

        // Phase 19C: RegisterProbe DSL — 注册到指定 ConditionSlot, 不占 clause 顺序索引. 替代 OnKey(Keys.None).CustomProbe(...) placeholder hack.
        foreach (var entry in _registeredProbes)
        {
            ctx.Aggregate.Conditions[entry.Slot].Probe ??= entry.Probe;
        }

        // Phase 19G-4: StoneProbe DSL — 命石 Probe 单字段注册 (与 ConditionSlot Probe 双轨, 海民/伐木机/骷髅王 命石业务).
        if (_stoneProbe is not null)
        {
            ctx.Aggregate.Conditions.StoneProbe ??= _stoneProbe;
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

        // 1. 先跑 SetupAction (按键 + Guard → 副作用, 如 F1+HasAghanim → LegSwap.修改配置 / D2 → SetMode + TTS).
        // Phase 16 C1b: IsOnEveryKey = true 时无 TriggerKey 匹配 (每键命中前都跑); ParamBoolProvider 非空时 AdjustLegSwap 第二参动态求值 (火枪 HasShard 形态).
        // Phase 16 C2: setup.Modifiers 严格等值比对 (default None == 裸键, OnEveryKey 形态 Modifiers 必须 None 也匹配 default trigger.Modifiers).
        foreach (SetupAction setup in _setups)
        {
            bool matchKey = setup.IsOnEveryKey || setup.TriggerKey == key;
            // OnEveryKey 形态忽略 Modifiers (每键都跑, 含修饰键场景); 否则 setup.Modifiers 与 trigger.Modifiers 严格等值.
            bool matchMod = setup.IsOnEveryKey || setup.Modifiers == trigger.Modifiers;
            if (matchKey && matchMod && CheckGuard(setup.Guard, ctx))
            {
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
                }
            }
        }

        // 2. 再跑 Clause 激活 (按键 → ConditionSlot.Active, 受 Guard 控制).
        // Phase 16 C2: clause.Modifiers 与 trigger.Modifiers 严格等值比对; default None == 裸键, 兼容现有 33 文件零 Modifiers 形态.
        for (int i = 0; i < _clauses.Length; i++)
        {
            if (_clauses[i].TriggerKey == key && _clauses[i].Modifiers == trigger.Modifiers && CheckGuard(_clauses[i].Guard, ctx))
            {
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
}

#endif
