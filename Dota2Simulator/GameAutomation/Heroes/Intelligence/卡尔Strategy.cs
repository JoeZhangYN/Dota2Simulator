// Phase 19D: 卡尔 Strategy 业务死代码清理 — 4 Probe 全 return Task.FromResult(false/true) 无副作用 (与 Phase 19B 死代码模板等价) ⇒ 迁 HeroPlan NoProbe 占槽形态.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("卡尔", HeroAttribute.Intelligence)]
public sealed partial class 卡尔Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.D1).NoProbe()
        .OnKey(Keys.D2).NoProbe()
        .OnKey(Keys.D3).NoProbe()
        .OnKey(Keys.D4).NoProbe()
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
