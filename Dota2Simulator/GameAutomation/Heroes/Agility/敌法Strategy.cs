// Phase 16 C1a: 敌法 Strategy 迁 HeroPlan — F1+HasShard AdjustLegSwap(D,true), W CustomProbe (主动技能释放后续 lambda + 分身物品组合 + 分身一齐攻击 helper), E AfterEnterCDLegOnly, R AfterCast, D+HasShard AfterEnterCDLegOnly, D2 Execute (SetMode+TTS 一次性).
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

[HeroStrategy("敌法", HeroAttribute.Agility)]
public sealed partial class 敌法Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.Q, alwaysSwap: false)
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.D, paramBool: true)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCastDo(() =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.W) == 1)
            {
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧_Tpl);
                分身一齐攻击();
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.深渊之刃_Tpl);
                _main._聚合.Skills.SetMode(SlotKey.W, 0);
            }
            _skill.通用技能后续动作();
        })
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCDLegOnly()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.D).WhenHasShard().CastSkill(Keys.D).AfterEnterCDLegOnly()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.SetMode(SlotKey.W, 1);
            Dota2Simulator.TTS.TTS.Speak("闪烁分身晕锤一次");
        })
        .Done();
}
#endif
