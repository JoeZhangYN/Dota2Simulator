// Phase 19D: 蓝猫 Strategy 迁 HeroPlan — Q Execute (残影接平A 多步骤宏) + W/R/D4 NoProbe (原 拉接平A/滚接平A return true dead, D4 无 Probe dead).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("蓝猫", HeroAttribute.Intelligence)]
public sealed partial class 蓝猫Strategy : IHeroStrategy
{

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).Execute(() => Task.Run(残影接平A))
        .OnKey(Keys.W).NoProbe()  // 占 C1 (原 拉接平A return true dead Probe)
        .OnKey(Keys.R).NoProbe()  // 占 C2 (原 滚接平A return true dead Probe)
        .OnKey(Keys.D4).NoProbe()  // 占 C3 (原 OnActivate 未注册 C3 Probe, 完全 dead)
        .Done();

    private void 残影接平A()
    {
        Common.Delay(等待延迟);
        走A();
    }
}
#endif
