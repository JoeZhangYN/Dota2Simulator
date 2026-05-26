// Phase 19G-4: 进化岛 Strategy 迁 HeroPlan — D/T/F3 三 Execute 全同 lambda (Delay 200 + PressViaEnigo A) - SimEnigo 后端调用模板.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("进化岛", HeroAttribute.Universal)]
public sealed partial class 进化岛Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.D).Execute(() => Task.Run(延迟后PressA))
        .OnKey(Keys.T).Execute(() => Task.Run(延迟后PressA))
        .OnKey(Keys.F3).Execute(() => Task.Run(延迟后PressA))
        .Done();

    private void 延迟后PressA()
    {
        Common.Delay(200);
        _input.PressViaEnigo(VirtualKey.From(Keys.A));
    }
}
#endif
