// Phase 16 C1a: 小骷髅 Strategy 迁 HeroPlan — F1+HasShard Execute (LegSwap(D,true,"敏捷") 第三参 string, 避污染 DSL), F1+HasAghanim AdjustLegSwap(F,true), Q/W/E/R/D/F CustomProbe (Mode 条件物品组合释放), D2 Execute, D3+HasShard Execute.
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

[HeroStrategy("小骷髅", HeroAttribute.Agility)]
public sealed partial class 小骷髅Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .AttackTiming(preDelay: 0.4, interval: 1.7)
        .OnKey(Keys.F1).WhenHasShard().Execute(() => _main._聚合.LegSwap.配置.修改配置(Keys.D, true, "敏捷"))
        .OnKey(Keys.F1).WhenHasAghanim().AdjustLegSwap(Keys.F, paramBool: true)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCDDo(() =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.Q) == 1)
            {
                _item.批量使用物品(
                    Dota2_Pictrue.物品.散失_Tpl,
                    Dota2_Pictrue.物品.散魂_Tpl,
                    Dota2_Pictrue.物品.否决_Tpl,
                    Dota2_Pictrue.物品.紫苑_Tpl,
                    Dota2_Pictrue.物品.血棘_Tpl,
                    Dota2_Pictrue.物品.羊刀_Tpl,
                    Dota2_Pictrue.物品.阿托斯之棍_Tpl,
                    Dota2_Pictrue.物品.缚灵锁_Tpl);
            }
            _skill.通用技能后续动作();
        })
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCDDo(() =>
        {
            _ = _skill.DOTA2释放CD就绪技能(Keys.Q);
            _skill.通用技能后续动作();
        })
        .OnKey(Keys.E).CastSkill(Keys.E).AfterCastDo(() => _input.MouseClick(MouseButton.Right))
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCDDo(() => _input.MouseClick(MouseButton.Right))
        .OnKey(Keys.D).WhenHasShard().CastSkill(Keys.F).AfterCastDo(() =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                Common.Delay(0);
                Press(Keys.R);
            }
        })
        .OnKey(Keys.F).WhenHasAghanim().CastSkill(Keys.F).AfterEnterCD()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Q);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "无脑接道具" : "手动道具");
        })
        .OnKey(Keys.D3).WhenHasShard().Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.F);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.F) == 1 ? "炽烈火雨隐身" : "炽烈火雨不隐身");
        })
        .Done();
}
#endif
