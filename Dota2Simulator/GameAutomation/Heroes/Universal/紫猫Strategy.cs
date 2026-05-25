// Phase 19B: 紫猫 业务死代码清理 — OnActivate Probe 全注释 + 仅 Active=true (无 Probe 跑) ⇒ 迁 HeroPlan NoProbe 占槽形态.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>紫猫（全才）策略——Phase 19B 业务死代码迁 HeroPlan NoProbe 形态.</summary>
[HeroStrategy("紫猫", HeroAttribute.Universal)]
public sealed partial class 紫猫Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).NoProbe()
        .OnKey(Keys.W).NoProbe()
        .OnKey(Keys.D).NoProbe()
        .OnKey(Keys.R).NoProbe()
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
