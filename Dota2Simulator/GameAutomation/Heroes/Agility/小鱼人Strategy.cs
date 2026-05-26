// Phase 16 C1a: 小鱼人 Strategy 迁 HeroPlan — F1+HasShard AdjustLegSwap(D,true), Q AfterEnterCD, W CustomProbe (跳水 Task.Run lambda 假腿切换), D+HasShard AfterEnterCD, R AfterEnterCD, D2 Execute (5 步键序列 L+右键+L+W).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("小鱼人", HeroAttribute.Agility)]
public sealed partial class 小鱼人Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .AttackTiming(preDelay: 0.5, interval: 1.7)
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.D, paramBool: true)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()
        .OnKey(Keys.W).CustomProbe(async () =>
        {
            _ = Task.Run(() =>
            {
                _skill.通用技能后续动作(是否保持假腿: false);
                _main._聚合.LegSwap.需要切假腿 = false;
                Common.Delay(200);
                _item.要求保持假腿();
            });
            return await Task.FromResult(false).ConfigureAwait(true);
        })
        .OnKey(Keys.D).WhenHasShard().CastSkill(Keys.D).AfterEnterCD()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .OnKey(Keys.D2).Execute(() =>
        {
            _input.KeyDown(VirtualKey.From(Keys.L));
            Common.Delay(33);
            _input.MouseClick(MouseButton.Right);
            Common.Delay(33);
            _input.KeyUp(VirtualKey.From(Keys.L));
            Common.Delay(110);
            Press(Keys.W);
        })
        .Done();
}
#endif
