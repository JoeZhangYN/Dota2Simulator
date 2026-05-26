// Phase 12 Chunk 3c: 小狗 Strategy 迁 HeroPlan DSL — 2 helper 全简单一行式, 100% fit.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("小狗", HeroAttribute.Strength)]
public sealed partial class 小狗Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast(continueAttack: true)
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();

    public override void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
