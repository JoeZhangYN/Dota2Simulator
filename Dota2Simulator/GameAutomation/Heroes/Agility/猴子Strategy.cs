// Phase 25A-C2 (2026-05-26): 猴子 Strategy 迁 DSL — 删 override OnKeyAsync.
// Q/W/R 前置 Press(E) if !E启动 用 HeroStrategyBase.PressIfStateOff(C1 helper) + .Pre() DSL; R 键无 Active 走 Execute setup.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("猴子", HeroAttribute.Agility)]
public sealed partial class 猴子Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.E, alwaysSwap: false)
        .LegSwap(Keys.R, alwaysSwap: false)
        .LegSwap(Keys.W, alwaysSwap: true, "力量")
        .OnKey(Keys.Q).Pre(PressIfStateOff(Keys.E, Keys.E)).CastSkill(Keys.Q).AfterCast()  // C1 灵魂之矛敏捷; Q 前置 Press(E) if !E启动
        .OnKey(Keys.W).Pre(PressIfStateOff(Keys.E, Keys.E)).CustomProbe(神行百变选择幻象)  // C2 神行百变; W 同前置
        .OnKey(Keys.R).Execute(PressIfStateOff(Keys.E, Keys.E))  // R 仅前置 Press(E) if !E启动, 无 ConditionSlot
        .Done();

    private async Task<bool> 神行百变选择幻象()
    {
        return await _skill.主动技能释放后续(Keys.W, () =>
        {
            Common.Delay(1000);
            Press(Keys.D1);
            Common.Delay(33);
            _input.MouseClick(MouseButton.Right);
            Press(Keys.F1);
            KeepLeg();
        }).ConfigureAwait(true);
    }
}
#endif
