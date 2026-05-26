// Phase 16 C2: 巫妖 Strategy 迁 HeroPlan — 5 全 CustomProbe (Step(E)>0 ? 11/10 : 1/0 动态 mode, Q/W/R Step 状态机 11/1 切换), E CustomProbe (阴邪凝视 Step 0/1/2 状态机), W+Alt Execute 共享 C2.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("巫妖", HeroAttribute.Intelligence)]
public sealed partial class 巫妖Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(async () => await _skill.技能通用判断(Keys.Q, _main._聚合.Skills.Step(SlotKey.E) > 0 ? 11 : 1).ConfigureAwait(true))
        .OnKey(Keys.W).CustomProbe(async () => await _skill.技能通用判断(Keys.W, _main._聚合.Skills.Step(SlotKey.E) > 0 ? 11 : 1, false).ConfigureAwait(true))
        .OnKey(Keys.W, KeyModifiers.Alt).SetActive(ConditionSlotKey.C2)
        .OnKey(Keys.E).CustomProbe(async () =>
        {
            if (_skill.DOTA2判断技能是否CD(Keys.E))
            {
                _main._聚合.Skills.SetStep(SlotKey.E, 0);
                return true;
            }
            int step = _main._聚合.Skills.Step(SlotKey.E);
            if (step == 0)
            {
                _main._聚合.Skills.SetStep(SlotKey.E, 1);
                return true;
            }
            else if (step == 1)
            {
                _ = Task.Run(() =>
                {
                    Common.Delay(200);
                    _main._聚合.Skills.SetStep(SlotKey.E, 2);
                });
                return true;
            }
            else
            {
                if (!_skill.DOTA2判断是否持续施法())
                {
                    _main._聚合.Skills.SetStep(SlotKey.E, 0);
                    走A();
                    _ = _item.根据图片使用物品(Dota2_Pictrue.物品.羊刀_Tpl);
                    return false;
                }
                return true;
            }
        })
        .OnKey(Keys.R).CustomProbe(async () => await _skill.技能通用判断(Keys.R, _main._聚合.Skills.Step(SlotKey.E) > 0 ? 11 : 1).ConfigureAwait(true))
        .OnKey(Keys.D).CustomProbe(async () => await _skill.技能通用判断(Keys.D, _main._聚合.Skills.Step(SlotKey.E) > 0 ? 10 : 0).ConfigureAwait(true))
        .Done();
}
#endif
