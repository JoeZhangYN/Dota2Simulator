// Phase 19G-3: 谜团 Strategy 迁 HeroPlan — F Execute (Task.Run 刷新接凋零黑洞 多步骤宏 Press X/Z/V/R 2x).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("谜团", HeroAttribute.Intelligence)]
public sealed partial class 谜团Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.F).Execute(() => Task.Run(刷新接凋零黑洞))
        .Done();

    private void 刷新接凋零黑洞()
    {
        Press(Keys.X);
        for (int i = 0; i < 2; i++)
        {
            Common.Delay(等待延迟);
            Press(Keys.Z);
            Press(Keys.V);
            Press(Keys.R);
        }
    }
}
#endif
