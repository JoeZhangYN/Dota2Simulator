#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("屠夫", HeroAttribute.Strength)]
public sealed partial class 屠夫Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 钩子去僵直;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 肢解检测状态;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 快速接肢解;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Q);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "勾接咬" : "勾接平A");
        }
    }

    // 钩子出手后，就可以用W，但其他技能无法释放且物品会被锁闭，可以通过判断锁闭的状态
    private async Task<bool> 钩子去僵直()
    {
        return await _skill.主动技能释放后续(Keys.Q, () =>
        {
            if (!_skill.DOTA2判断状态技能是否启动(Keys.W))
            {
                _input.Press(VirtualKey.From(Keys.W));
            }

            _input.Press(VirtualKey.From(Keys.A));
            if (_main._聚合.Skills.Mode(SlotKey.Q) == 1)
            {
                _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 肢解检测状态()
    {
        return await _skill.主动技能释放后续(Keys.R, () =>
        {
            if (!_skill.DOTA2判断状态技能是否启动(Keys.W))
            {
                _input.Press(VirtualKey.From(Keys.W));
            }

            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.希瓦_Tpl);
        }).ConfigureAwait(true);
    }

    // 技能颜色虽然变了，但是CD状态的颜色没变，
    // 钩可以直接接咬，但期间物品还是锁闭的
    // 解决。
    private async Task<bool> 快速接肢解()
    {
        return await _item.所有物品可用后续(() =>
        {
            Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl));
            Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.希瓦_Tpl));
            Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃_Tpl));
            Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.否决_Tpl));

            Common.Delay(33 *
                  (_item.根据图片使用物品(Dota2_Pictrue.物品.红杖_Tpl) +
                   _item.根据图片使用物品(Dota2_Pictrue.物品.红杖2_Tpl) +
                   _item.根据图片使用物品(Dota2_Pictrue.物品.红杖3_Tpl) +
                   _item.根据图片使用物品(Dota2_Pictrue.物品.红杖4_Tpl) +
                   _item.根据图片使用物品(Dota2_Pictrue.物品.红杖5_Tpl)));

            _input.Press(VirtualKey.From(Keys.R));
        }).ConfigureAwait(true);
    }
}
#endif
