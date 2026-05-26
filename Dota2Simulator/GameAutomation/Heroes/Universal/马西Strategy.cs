// Phase 19G-4: 马西 Strategy 迁 HeroPlan — W WhenNotHasAghanim Execute (C2 Active=true) + RegisterProbe(C2, 幽魂检测 ImageFinder). Phase 19C 反向 Guard DSL 已支持.
#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("马西", HeroAttribute.Universal)]
public sealed partial class 马西Strategy : IHeroStrategy
{
    private static readonly Rectangle buff状态技能栏 = new(962, 826, 526, 80);

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.W).WhenNotHasAghanim().Execute(() => _main._聚合.Conditions[ConditionSlotKey.C2].Active = true)
        .RegisterProbe(ConditionSlotKey.C2, 幽魂检测)
        .Done();

    private async Task<bool> 幽魂检测()
    {
        return _vision.Find(Dota2_Pictrue.Buff.小精灵_幽魂_Tpl, buff状态技能栏, new MatchRate(0.9), Tolerance.Exact).Found
            ? await Task.FromResult(true).ConfigureAwait(true)
            : await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
