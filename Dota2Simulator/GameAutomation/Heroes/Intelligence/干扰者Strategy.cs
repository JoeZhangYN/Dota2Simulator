// Phase 19D: 干扰者 Strategy 迁 HeroPlan — Q/W/E/R AfterEnterCD 简单 fit + D2 Execute (SetStep + Active C5) + RegisterProbe(C5, 静态风暴动能立场风雷之击 dynamic Probe).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("干扰者", HeroAttribute.Intelligence)]
public sealed partial class 干扰者Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD(continueAttack: false)
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD(continueAttack: false)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.SetStep(SlotKey.W, 0);
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        })
        .RegisterProbe(ConditionSlotKey.C5, 静态风暴动能立场风雷之击)
        .Done();

    private async Task<bool> 静态风暴动能立场风雷之击()
    {
        return _skill.DOTA2释放CD就绪技能(Keys.R)
            ? await Task.FromResult(true).ConfigureAwait(true)
            : _skill.DOTA2释放CD就绪技能(Keys.E)
                ? await Task.FromResult(true).ConfigureAwait(true)
                : _skill.DOTA2释放CD就绪技能(Keys.Q)
                    ? await Task.FromResult(true).ConfigureAwait(true)
                    : await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
