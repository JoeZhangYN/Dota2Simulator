// Phase 19B: 冰女 业务死代码清理 — OnActivate / OnKeyAsync 完全空 ⇒ 迁 HeroPlan 空 plan 形态 (0 clause / 0 setup).
#if DOTA2
using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("冰女", HeroAttribute.Intelligence)]
public sealed partial class 冰女Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New().Done();
}
#endif
