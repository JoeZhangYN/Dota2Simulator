// Phase 19B: 剃刀 业务死代码清理 — Probe 全注释 + 仅 Active=true 设置 (无 Probe 跑) ⇒ 迁 HeroPlan NoProbe 占槽形态.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>剃刀（敏捷）策略——Phase 12+ 横向扫荡未迁的形态不 fit 英雄, Phase 19B 用 NoProbe 占槽统一形态.</summary>
[HeroStrategy("剃刀", HeroAttribute.Agility)]
public sealed partial class 剃刀Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).NoProbe()
        .OnKey(Keys.E).NoProbe()
        .OnKey(Keys.R).NoProbe()
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
