// Phase 13 C1: 宙斯 Strategy 迁 HeroPlan — 5 helper 全 _skill.技能通用判断, 100% fit Plan.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("宙斯", HeroAttribute.Intelligence)]
public sealed partial class 宙斯Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.D).CastSkill(Keys.D).AfterEnterCD()
        .Done();
}
#endif
