// Phase 19B: 修补匠 业务死代码清理 — OnActivate 空 + 仅 Q/E/R Active=true (无 Probe 跑) ⇒ 迁 HeroPlan NoProbe 占槽形态.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("修补匠", HeroAttribute.Intelligence)]
public sealed partial class 修补匠Strategy : IHeroStrategy
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
