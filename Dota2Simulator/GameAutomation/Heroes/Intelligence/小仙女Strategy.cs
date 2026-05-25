// Phase 13 C2: 小仙女 Strategy 迁 HeroPlan + ToggleSlot DSL — F+HasShard→NoProbe / D3 toggle 循环续暗影 (mode 2 Probe 自检 Active).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("小仙女", HeroAttribute.Intelligence)]
public sealed partial class 小仙女Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.F).WhenHasShard().NoProbe()
        .OnKey(Keys.D3).ToggleSlot(skillKey: Keys.W, speakOn: "续暗影", speakOff: "不续暗影")
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
