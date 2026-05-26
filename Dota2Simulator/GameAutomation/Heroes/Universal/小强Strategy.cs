// Phase 13 C2: 小强 Strategy 迁 HeroPlan + ToggleSlot DSL — D3 toggle 循环爆裂 (mode 2) + RepeatThreshold(150).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("小强", HeroAttribute.Universal)]
public sealed partial class 小强Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.D3).ToggleSlot(skillKey: Keys.W, speakOn: "循环爆裂", speakOff: "终止循环")
        .RepeatThreshold(150)
        .Done();
}
#endif
