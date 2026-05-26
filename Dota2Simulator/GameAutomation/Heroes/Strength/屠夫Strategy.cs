// Phase 19G-3: 屠夫 Strategy 迁 HeroPlan — Q/R CustomProbe (钩子去僵直 / 肢解检测状态 同质 主动技能释放后续 lambda) + D2 Execute (ToggleMode Q + TTS) + RegisterProbe(C3, 快速接肢解 由 Q Probe 跨 clause 设 Active).
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

[HeroStrategy("屠夫", HeroAttribute.Strength)]
public sealed partial class 屠夫Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(钩子去僵直)  // C1: 钩子 (Mode==1 时跨 clause 设 C3.Active=true)
        .OnKey(Keys.R).CustomProbe(肢解检测状态)  // C2: 肢解
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Q);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "勾接咬" : "勾接平A");
        })
        .RegisterProbe(ConditionSlotKey.C3, 快速接肢解)  // C3: 快速接肢解 (由 C1 Probe 跨 clause 驱动)
        .Done();

    private async Task<bool> 钩子去僵直()
    {
        return await _skill.主动技能释放后续(Keys.Q, () =>
        {
            if (!_skill.DOTA2判断状态技能是否启动(Keys.W))
                _input.Press(VirtualKey.From(Keys.W));
            _input.Press(VirtualKey.From(Keys.A));
            if (_main._聚合.Skills.Mode(SlotKey.Q) == 1)
                _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }).ConfigureAwait(true);
    }

    private async Task<bool> 肢解检测状态()
    {
        return await _skill.主动技能释放后续(Keys.R, () =>
        {
            if (!_skill.DOTA2判断状态技能是否启动(Keys.W))
                _input.Press(VirtualKey.From(Keys.W));
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.希瓦_Tpl);
        }).ConfigureAwait(true);
    }

    private async Task<bool> 快速接肢解()
    {
        return await _item.所有物品可用后续(() =>
        {
            Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl));
            Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.希瓦_Tpl));
            Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃_Tpl));
            Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.否决_Tpl));
            Common.Delay(33 * (
                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖_Tpl) +
                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖2_Tpl) +
                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖3_Tpl) +
                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖4_Tpl) +
                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖5_Tpl)));
            _input.Press(VirtualKey.From(Keys.R));
        }).ConfigureAwait(true);
    }
}
#endif
