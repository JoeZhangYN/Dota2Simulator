#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("巫医", HeroAttribute.Intelligence)]
public sealed partial class 巫医Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 麻痹药剂去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 巫蛊咒术去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 死亡守卫隐身;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private async Task<bool> 麻痹药剂去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1, 要接的按键: Keys.E).ConfigureAwait(true);
    }

    private async Task<bool> 巫蛊咒术去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.E, () =>
        {
            _input.Press(VirtualKey.From(Keys.A));
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.魂之灵龛);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.影之灵龛);
        }).ConfigureAwait(true);
    }

    private async Task<bool> 死亡守卫隐身(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.R, () =>
        {
            _ = _item.根据图片自我使用物品(Dota2_Pictrue.物品.微光披风);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.隐刀);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.大隐刀);
        }).ConfigureAwait(true);
    }
}
#endif
