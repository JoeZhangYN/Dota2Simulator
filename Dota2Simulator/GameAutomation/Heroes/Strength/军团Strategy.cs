// Phase 19G-3: 军团 Strategy 迁 HeroPlan — Q/W/R 3 simple AfterX + F CustomProbe (决斗 Step 状态机 多步骤宏 inline) + D2 Execute (ToggleMode Global + TTS) + LegSwap E false.
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

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("军团", HeroAttribute.Strength)]
public sealed partial class 军团Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()  // C1: 压倒性优势
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()  // C2: 强攻
        .OnKey(Keys.F).Pre(() =>
        {
            if (_main._聚合.Skills.Step(SlotKey.Global) == -1)
                _main._聚合.Skills.SetStep(SlotKey.Global, 0);
        }).CustomProbe(决斗)  // C3: 决斗 Step 状态机
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()  // C4: 决斗去后摇 (mode 1)
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Global);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Global) == 1 ? "跳刀决斗" : "直接决斗");
        })
        .Done();

    public override void OnActivate(HeroContext ctx)
    {
        base.OnActivate(ctx);
        _main._聚合.Skills.SetStep(SlotKey.Global, -1);
    }

    private async Task<bool> 决斗()
    {
        return await Task.Run(async () =>
        {
            int 步骤 = _main._聚合.Skills.Step(SlotKey.Global);
            switch (步骤)
            {
                case < 1:
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.臂章_Tpl));
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.魂戒_Tpl));
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.相位鞋_Tpl));
                    if (_skill.DOTA2判断技能是否CD(Keys.W))
                    {
                        _input.ComboAlt(VirtualKey.From(Keys.W));
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.刃甲_Tpl));
                    _main._聚合.Skills.SetStep(SlotKey.Global, 1);
                    return await Task.FromResult(true).ConfigureAwait(true);
                case < 2 when 步骤 == 1:
                    Common.Delay(33 * (
                        _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_Tpl)
                        + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀_Tpl)
                        + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀_Tpl)
                        + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀_Tpl)));
                    _main._聚合.Skills.SetStep(SlotKey.Global, 2);
                    return await Task.FromResult(true).ConfigureAwait(true);
                case < 3:
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.紫苑_Tpl));
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.血棘_Tpl));
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.否决_Tpl));
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.散失_Tpl));
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.散魂_Tpl));
                    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.深渊之刃_Tpl));
                    _main._聚合.Skills.SetStep(SlotKey.Global, 3);
                    return await Task.FromResult(true).ConfigureAwait(true);
                case < 4:
                    _input.Press(VirtualKey.From(Keys.A));
                    if (_skill.DOTA2释放CD就绪技能(Keys.R))
                    {
                        Common.Delay(60);
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }
                    _main._聚合.Skills.SetStep(SlotKey.Global, -1);
                    return await Task.FromResult(false).ConfigureAwait(true);
            }
            return await Task.FromResult(false).ConfigureAwait(true);
        }).ConfigureAwait(true);
    }
}
#endif
