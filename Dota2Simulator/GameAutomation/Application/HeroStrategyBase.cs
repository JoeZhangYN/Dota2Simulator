// Phase 19G-2 (2026-05-26): HeroStrategyBase 终极业务抽象 — Phase 12-19 持续推迟项严格收尾.
// 75 已迁 HeroPlan Strategy 1 行 OnActivate + 1 行 OnKeyAsync + _plan 字段全部样板 → base 默认实现 + override BuildPlan() 一行表达式.
// 17 未迁 Strategy 保留 override OnActivate/OnKeyAsync 自定义业务 body.
// SG 配合: 不再 emit ports field, ctor 改 `: base(...)` 调用, Hero 改 `public override` 形式; ports 字段 base 持 (protected readonly).
#if DOTA2

using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// Hero strategy 模板基类 (Phase 19G-2 引入).
/// 标准模式 (已迁 HeroPlan): 业务侧仅 override <see cref="BuildPlan"/> 返回声明式 HeroPlan, base 默认实现 OnActivate/OnKeyAsync 调用 Apply/DispatchAsync.
/// 自定义模式 (未迁 hero): 业务侧 override OnActivate/OnKeyAsync 提供自定义 body, BuildPlan 留 default 空 plan.
/// </summary>
public abstract class HeroStrategyBase : IHeroStrategy
{
    protected readonly IInputExecutor _input;
    protected readonly IScreenVision _vision;
    protected readonly SkillEngine _skill;
    protected readonly ItemEngine _item;
    protected readonly IUiInvoker? _ui;
    protected readonly HeroLoopHost _main;

    protected HeroStrategyBase(
        IInputExecutor input,
        IScreenVision vision,
        SkillEngine skill,
        ItemEngine item,
        HeroLoopHost main,
        IUiInvoker? ui = null)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
        _ui = ui;
    }

    /// <summary>该策略对应的英雄标识 — SG emit override (从 [HeroStrategy] attribute SSOT 派生).</summary>
    public abstract HeroId Hero { get; }

    /// <summary>Plan lazy cache — 首次 OnActivate/OnKeyAsync 调用时触发 BuildPlan() 并缓存; HeroPlan 是不可变值对象, 单实例 strategy 全生命周期复用. WinForms message pump 单线程调用, 无需 Lazy&lt;T&gt; 锁开销.</summary>
    private HeroPlan? _cachedPlan;

    /// <summary>缓存的 plan 访问器 — 业务侧若需访问可在 override OnActivate/OnKeyAsync 内直接调用 <c>Plan</c>.</summary>
    protected HeroPlan Plan => _cachedPlan ??= BuildPlan();

    /// <summary>
    /// 声明式 HeroPlan 构造 — 业务侧 override 返回 HeroPlanBuilder.New()...Done().
    /// 默认空 plan 用于未迁 hero (业务侧 override OnActivate/OnKeyAsync 自定义 body).
    /// </summary>
    protected virtual HeroPlan BuildPlan() => HeroPlanBuilder.New().Done();

    /// <summary>HeroPlan 主路径默认实现 — 业务覆盖 BuildPlan 即可消费; 未迁 hero 可 override.</summary>
    public virtual void OnActivate(HeroContext ctx) => Plan.Apply(ctx, _skill);

    /// <summary>HeroPlan 主路径默认实现 — 业务覆盖 BuildPlan 即可消费; 未迁 hero 可 override.</summary>
    public virtual Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => Plan.DispatchAsync(trigger, ctx, _item);

    /// <summary>
    /// Phase 22D (2026-05-26): WinForms Keys → VirtualKey 自动 wrap, 替原业务 <c>_input.Press(VirtualKey.From(Keys.X))</c> 冗长形态.
    /// 不改 IInputExecutor 端口契约 (保持 VirtualKey 强类型 + Adapter 实现纯洁), 仅在 base 内 ergonomic helper.
    /// </summary>
    protected void Press(System.Windows.Forms.Keys k) => _input.Press(Domain.Actuation.VirtualKey.From(k));

    /// <summary>Phase 22D: 走 A 专用简写 — 替原 <c>_input.Press(VirtualKey.From(Keys.A))</c> 40 处同构.</summary>
    protected void 走A() => _input.Press(Domain.Actuation.VirtualKey.From(System.Windows.Forms.Keys.A));

    /// <summary>
    /// Phase 26 F1 (2026-05-26): 切假腿保持快捷 helper — 替业务 lambda 内 <c>_item.要求保持假腿()</c> 9 处同构.
    /// 用法: <c>.OnKey(Keys.Q).CastSkill(Keys.Q).AfterCastDo(() => KeepLeg())</c>.
    /// </summary>
    protected void KeepLeg() => _item.要求保持假腿();

    /// <summary>
    /// Phase 25A C1: 条件 Press 前置 helper — 返回一个 Action，当 stateSkill 状态技能未启动时按 keyToPress.
    /// 用法: <c>.OnKey(Keys.Q).Pre(PressIfStateOff(Keys.E, Keys.E)).CastSkill(Keys.Q).AfterCast()</c>
    /// 替代猴子 Q/W/R 共用 <c>if (!_skill.DOTA2判断状态技能是否启动(E)) Press(E)</c> 3 行同构 Pre lambda.
    /// </summary>
    protected System.Action PressIfStateOff(System.Windows.Forms.Keys stateSkill, System.Windows.Forms.Keys keyToPress)
        => () =>
        {
            if (!_skill.DOTA2判断状态技能是否启动(stateSkill))
                Press(keyToPress);
        };
}

#endif
