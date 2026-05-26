// Phase 16 C1a: 钢背 Strategy 迁 HeroPlan — F1+HasShard/HasAghanim 双 AdjustLegSwap, D+HasShard / E+HasAghanim Guard NoProbe, Q/W NoProbe (原 4 Probe ??= 注册全注释为死代码, 此处用 NoProbe 等价占槽).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("钢背", HeroAttribute.Strength)]
public sealed partial class 钢背Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.D, paramBool: true)
        .OnKey(Keys.F1).WhenHasAghanim().AdjustLegSwap(Keys.E, paramBool: true)
        .OnKey(Keys.Q).NoProbe()
        .OnKey(Keys.D).WhenHasShard().NoProbe()
        .OnKey(Keys.E).WhenHasAghanim().NoProbe()
        .OnKey(Keys.W).NoProbe()
        .Done();
}
#endif
