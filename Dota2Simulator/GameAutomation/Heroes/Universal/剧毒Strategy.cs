// Phase 19D: 剧毒 Strategy 迁 HeroPlan — Q/R AfterCast + E CustomProbe (MouseClick Right + return false) + D3 ToggleSlot(E 循环蛇棒 mode 2) + S Execute (Session.IsPaused + 重置 5 Active).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("剧毒", HeroAttribute.Universal)]
public sealed partial class 剧毒Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.W, alwaysSwap: false)
        .RepeatThreshold(100)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.E).CustomProbe(蛇棒去后摇)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.D3).ToggleSlot(Keys.E, "循环蛇棒", "终止循环")
        .OnKey(Keys.S).Execute(() =>
        {
            _main.Session!.IsPaused = true;
            for (int i = 0; i < 5; i++)
                _main._聚合.Conditions[(ConditionSlotKey)i].Active = false;
        })
        .Done();

    private async Task<bool> 蛇棒去后摇()
    {
        _input.MouseClick(MouseButton.Right);
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
