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
    public override void OnActivate(HeroContext ctx)
    {
        base.OnActivate(ctx);
        _main._聚合.Attack.基础攻击前摇 = 0.4;
        _main._聚合.Attack.基础攻击间隔 = 1.7;
    }

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.F1).WhenHasShard().Execute(() => _main._聚合.LegSwap.配置.修改配置(Keys.D, true, "敏捷"))
        .OnKey(Keys.F1).WhenHasAghanim().AdjustLegSwap(Keys.F, paramBool: true)
        .OnKey(Keys.Q).CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.Q, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.Q) == 1)
            {
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.散失_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.散魂_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.否决_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.紫苑_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.血棘_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.羊刀_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.阿托斯之棍_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.缚灵锁_Tpl));
            }
            _skill.通用技能后续动作();
        }).ConfigureAwait(true))
        .OnKey(Keys.W).CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.W, () =>
        {
            _ = _skill.DOTA2释放CD就绪技能(Keys.Q);
            _skill.通用技能后续动作();
        }).ConfigureAwait(true))
        .OnKey(Keys.E).CustomProbe(async () => await _skill.主动技能释放后续(Keys.E, () => _input.MouseClick(MouseButton.Right)).ConfigureAwait(true))
        .OnKey(Keys.R).CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.R, () => _input.MouseClick(MouseButton.Right)).ConfigureAwait(true))
        .OnKey(Keys.D).WhenHasShard().CustomProbe(async () => await _skill.主动技能释放后续(Keys.F, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                Common.Delay(0);
                _input.Press(VirtualKey.From(Keys.R));
            }
        }).ConfigureAwait(true))
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
