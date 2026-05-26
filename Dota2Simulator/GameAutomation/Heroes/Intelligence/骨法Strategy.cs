// Phase 19G-4: 骨法 Strategy 迁 HeroPlan — 4 CustomProbe (Q/E 动态 mode CustomProbe escape-hatch + W 物品组合 + R Step 状态机) + R Pre(物品组合 inline) + D2 Execute (ToggleMode R + TTS).
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

[HeroStrategy("骨法", HeroAttribute.Intelligence)]
public sealed partial class 骨法Strategy : IHeroStrategy
{
    private HeroPlan? _plan;
    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(幽冥轰爆去后摇)  // C1: 动态 mode (Step(R)>0 ? 10 : 0)
        .OnKey(Keys.W).CustomProbe(衰老去后摇)  // C2: 主动技能进入CD后续 + 5 红杖
        .OnKey(Keys.E).CustomProbe(幽冥守卫去后摇)  // C3: 动态 mode
        .OnKey(Keys.R).Pre(() =>
        {
            _item.根据图片使用物品(Dota2_Pictrue.物品.希瓦_Tpl);
            _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl);
        }).CustomProbe(生命吸取去后摇)  // C4: Step 状态机
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.R);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.R) == 1 ? "吸取接衰老" : "吸取不接衰老");
        })
        .Done();

    protected override HeroPlan BuildPlan() => GetPlan();

    private async Task<bool> 幽冥轰爆去后摇()
        => await _skill.技能通用判断(Keys.Q, _main._聚合.Skills.Step(SlotKey.R) > 0 ? 10 : 0).ConfigureAwait(true);

    private async Task<bool> 衰老去后摇()
    {
        return await _skill.主动技能进入CD后续(Keys.W, () =>
        {
            Common.Delay(33 * (
                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖_Tpl)
                + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖2_Tpl)
                + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖3_Tpl)
                + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖4_Tpl)
                + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖5_Tpl)
            ));
        }).ConfigureAwait(true);
    }

    private async Task<bool> 幽冥守卫去后摇()
        => await _skill.技能通用判断(Keys.E, _main._聚合.Skills.Step(SlotKey.R) > 0 ? 10 : 0).ConfigureAwait(true);

    private async Task<bool> 生命吸取去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.R))
        {
            _main._聚合.Skills.SetStep(SlotKey.R, 0);
            return await Task.FromResult(true).ConfigureAwait(true);
        }
        return await Task.Run(async () =>
        {
            if (_main._聚合.Skills.Step(SlotKey.R) == 0)
            {
                if (_main._聚合.Skills.Mode(SlotKey.R) == 1)
                    _input.Press(VirtualKey.From(Keys.W));
                _main._聚合.Skills.SetStep(SlotKey.R, 1);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            else if (_main._聚合.Skills.Step(SlotKey.R) == 1)
            {
                _ = Task.Run(() =>
                {
                    Common.Delay(200);
                    _main._聚合.Skills.SetStep(SlotKey.R, 2);
                });
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            else
            {
                if (!_skill.DOTA2判断是否持续施法())
                {
                    _main._聚合.Skills.SetStep(SlotKey.R, 0);
                    _input.Press(VirtualKey.From(Keys.A));
                    return await Task.FromResult(false).ConfigureAwait(true);
                }
                return await Task.FromResult(true).ConfigureAwait(true);
            }
        }).ConfigureAwait(false);
    }
}
#endif
