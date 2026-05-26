// Phase 13 C1: 斯温 Strategy 迁 HeroPlan — 3 helper 全 _skill.技能通用判断, 100% fit Plan.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("斯温", HeroAttribute.Strength)]
public sealed partial class 斯温Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .LegSwap(Keys.W, alwaysSwap: false)
        .Done();
}
#endif
