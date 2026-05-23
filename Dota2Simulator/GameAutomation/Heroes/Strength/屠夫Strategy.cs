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

public sealed class 屠夫Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 屠夫Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("屠夫", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 钩子去僵直;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 肢解检测状态;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 快速接肢解;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.Q);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "勾接咬" : "勾接平A");
        }
    }

    // 钩子出手后，就可以用W，但其他技能无法释放且物品会被锁闭，可以通过判断锁闭的状态
    private async Task<bool> 钩子去僵直(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.Q, () =>
        {
            if (!Skill.DOTA2判断状态技能是否启动(Keys.W, in 句柄))
            {
                _input.Press(VirtualKey.From(Keys.W));
            }

            _input.Press(VirtualKey.From(Keys.A));
            if (Main._聚合.Skills.Mode(SlotKey.Q) == 1)
            {
                Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 肢解检测状态(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.R, () =>
        {
            if (!Skill.DOTA2判断状态技能是否启动(Keys.W, in 句柄))
            {
                _input.Press(VirtualKey.From(Keys.W));
            }

            _ = Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
            _ = Item.根据图片使用物品(Dota2_Pictrue.物品.希瓦);
        }).ConfigureAwait(true);
    }

    // 技能颜色虽然变了，但是CD状态的颜色没变，
    // 钩可以直接接咬，但期间物品还是锁闭的
    // 解决。
    private async Task<bool> 快速接肢解(ImageHandle 句柄)
    {
        return await Item.所有物品可用后续(句柄, () =>
        {
            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.纷争));
            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.希瓦));
            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃));
            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.否决));

            Common.Delay(33 *
                  (Item.根据图片使用物品(Dota2_Pictrue.物品.红杖) +
                   Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2) +
                   Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3) +
                   Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4) +
                   Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)));

            _input.Press(VirtualKey.From(Keys.R));
        }).ConfigureAwait(true);
    }
}
#endif
