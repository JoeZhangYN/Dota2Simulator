// Phase 12 Chunk 3b: 土猫 Strategy 迁 HeroPlan DSL — 3 helper 全简单一行式, 100% fit Plan; 无 LegSwap.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("土猫", HeroAttribute.Strength)]
public sealed partial class 土猫Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast(continueAttack: true)
        .Done();
}
#endif
