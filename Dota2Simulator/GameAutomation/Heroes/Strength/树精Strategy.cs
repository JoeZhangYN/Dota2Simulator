// Phase 13 C3: 树精 Strategy 迁 HeroPlan + AlsoAdjustLegSwap DSL — D 键+HasAghanim 同时 AdjustLegSwap(D,true) + Active (clause + setup 共享 TriggerKey/Guard).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("树精", HeroAttribute.Strength)]
public sealed partial class 树精Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterCast()
        .OnKey(Keys.D).WhenHasAghanim().CastSkill(Keys.D).AfterCast().AlsoAdjustLegSwap(Keys.D, true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .Done();
}
#endif
