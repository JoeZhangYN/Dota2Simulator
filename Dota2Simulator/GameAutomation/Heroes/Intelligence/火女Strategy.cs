// Phase 19D: 火女 Strategy 迁 HeroPlan — Q/W/R 3 CustomProbe (CD 检查 + Press A 同质模板). 3 helper 合并 1 共享 Probe.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("火女", HeroAttribute.Intelligence)]
public sealed partial class 火女Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(() => CD检查或PressA(Keys.Q))
        .OnKey(Keys.W).CustomProbe(() => CD检查或PressA(Keys.W))
        .OnKey(Keys.R).CustomProbe(() => CD检查或PressA(Keys.R))
        .Done();

    /// <summary>3 原 helper (龙破斩/光击阵/神灭斩去后摇) 同质模板复用.</summary>
    private async Task<bool> CD检查或PressA(Keys skillKey)
    {
        if (_skill.DOTA2判断技能是否CD(skillKey))
            return await Task.FromResult(true).ConfigureAwait(true);
        _input.Press(VirtualKey.From(Keys.A));
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
