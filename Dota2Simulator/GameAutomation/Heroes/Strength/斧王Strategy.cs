#if DOTA2
using System.Drawing;
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

using Dota2Simulator.GameAutomation.Domain.Perception;
namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 斧王Strategy : IHeroStrategy
{
    /// <summary>基准帧延迟（沿用 Main.等待延迟）。</summary>
    private const int 等待延迟 = 33;


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 斧王Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("斧王", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 吼去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 战斗饥渴去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 淘汰之刃去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 跳吼;
        Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒);
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒);
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒);
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D4))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.Q);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "吼接刃甲" : "吼不接刃甲");
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            快速触发激怒();
        }
    }

    private async Task<bool> 吼去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.Q, () =>
        {
            if (Main._聚合.Skills.Mode(SlotKey.Q) == 1)
            {
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.刃甲);
            }
            // 触发激怒
            _input.Press(VirtualKey.From(Keys.A));
            _input.Press(VirtualKey.From(Keys.W));
        }).ConfigureAwait(true);
    }

    private async Task<bool> 战斗饥渴去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private async Task<bool> 淘汰之刃去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private async Task<bool> 跳吼(ImageHandle 句柄)
    {
        if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀) == 1)
        {
            Common.Delay(等待延迟);
        }

        _ = Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);

        return await Task.FromResult(false).ConfigureAwait(true);
    }

    // 沿用 Main.快速触发激怒
    private void 快速触发激怒()
    {
        var 原始位置 = Control.MousePosition;

        for (int i = 0; i < 10; i++)
        {
            _input.MouseMoveTo(new ScreenPoint(575 + 515 + 61 * i, 20));
            _input.Press(VirtualKey.From(Keys.A));
            Common.Delay(2);
        }

        _input.MouseMoveTo(new ScreenPoint(原始位置.X, 原始位置.Y));
    }
}
#endif
