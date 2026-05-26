// Phase 15 C1: 大牛 Strategy 迁 HeroPlan + Pre DSL — Q AfterCast(postDelayMs:1300), W Pre(_input.Press(A)) + AfterCastReplaceIcon (Phase 26 G2: DSL chain 替原 CustomProbe lambda), R AfterCast.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("大牛", HeroAttribute.Strength)]
public sealed partial class 大牛Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(postDelayMs: 1300)
        .OnKey(Keys.W).Pre(() => 走A()).CastSkill(Keys.W).AfterCastReplaceIcon(SlotKey.W)  // Phase 26 G2: AfterCastReplaceIcon DSL 替原 CustomProbe lambda
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();
}
#endif
