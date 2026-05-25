// Phase 12 Chunk 3b: 全能 Strategy 迁 HeroPlan DSL — 3 helper 全 AfterCast(true), 100% fit Plan.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("全能", HeroAttribute.Strength)]
public sealed partial class 全能Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueAttack: true)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast(continueAttack: true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast(continueAttack: true)
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
