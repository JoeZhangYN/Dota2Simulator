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

    /// <summary>
    /// 声明式 HeroPlan 构造 — 业务侧 override 返回 HeroPlanBuilder.New()...Done().
    /// 默认空 plan 用于未迁 hero (业务侧 override OnActivate/OnKeyAsync 自定义 body).
    /// </summary>
    protected virtual HeroPlan BuildPlan() => HeroPlanBuilder.New().Done();

    /// <summary>HeroPlan 主路径默认实现 — 业务覆盖 BuildPlan 即可消费; 未迁 hero 可 override.</summary>
    public virtual void OnActivate(HeroContext ctx) => BuildPlan().Apply(ctx, _skill);

    /// <summary>HeroPlan 主路径默认实现 — 业务覆盖 BuildPlan 即可消费; 未迁 hero 可 override.</summary>
    public virtual Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => BuildPlan().DispatchAsync(trigger, ctx, _item);
}

#endif
