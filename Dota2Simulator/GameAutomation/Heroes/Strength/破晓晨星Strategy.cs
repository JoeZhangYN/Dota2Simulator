// Phase 19G-3: 破晓晨星 Strategy 迁 HeroPlan — W AfterCast (上界重锤 mode 1). C1 业务侧无触发, 用 NoProbe 占位映射 W→C2.
#if DOTA2
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("破晓晨星", HeroAttribute.Strength)]
public sealed partial class 破晓晨星Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.None).NoProbe()  // 占 C1 (原 OnActivate 未注册 C1, OnKeyAsync 也不触发, 业务侧 W→C2 跳过 C1)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()
        .Done();

    protected override HeroPlan BuildPlan() => _plan;
}
#endif
