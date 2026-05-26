// Phase 19D: 赏金 Strategy 迁 HeroPlan — Q AfterCast + R AfterCast + D2 ToggleSlot (循环标记 Probe 自循环 mode 2).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("赏金", HeroAttribute.Agility)]
public sealed partial class 赏金Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.W, alwaysSwap: false)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.D2).ToggleSlot(Keys.R, "循环标记", "不循环标记")
        .Done();
}
#endif
