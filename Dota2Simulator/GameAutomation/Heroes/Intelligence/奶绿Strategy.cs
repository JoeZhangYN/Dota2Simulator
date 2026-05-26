// Phase 19D: 奶绿 Strategy 迁 HeroPlan — Q AfterCast + W/R AfterEnterCD (R postDelayMs=360) + LegSwap E false. 原 D/F 键无 OnKeyAsync 触发, C4/C5 Probe 注册 dead, 迁后不声明.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("奶绿", HeroAttribute.Intelligence)]
public sealed partial class 奶绿Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD(postDelayMs: 360)
        .Done();

    public override void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
