// Phase 16 C1a: 幽鬼 Strategy 迁 HeroPlan — F1+HasShard AdjustLegSwap(E,true), Q AfterCast(false), R/D CustomProbe (Mode 检查物品组合), E AfterEnterCD, D2 Execute (ToggleMode F + TTS).
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

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("幽鬼", HeroAttribute.Agility)]
public sealed partial class 幽鬼Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.W, alwaysSwap: false)
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.E, paramBool: true)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueAttack: false)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCastDo(() =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                Press(Keys.D);
            }
        })
        .OnKey(Keys.D).CastSkill(Keys.D).AfterEnterCDDo(() =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                if (_item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧_Tpl) == 1)
                {
                    分身一齐攻击();
                }
                _item.批量使用物品(
                    Dota2_Pictrue.物品.否决_Tpl,
                    Dota2_Pictrue.物品.紫苑_Tpl,
                    Dota2_Pictrue.物品.血棘_Tpl);
            }
            KeepLeg();
            走A();
        })
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()
        .OnKey(Keys.D2).ToggleModeTts(SlotKey.F, "如影随形分身", "关闭随形分身")
        .Done();
}
#endif
