#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>幽鬼（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "幽鬼"。</summary>
public sealed class 幽鬼Strategy : IHeroStrategy
{
    public HeroId Hero => new("幽鬼", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 幽鬼之刃去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 如影随形去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 空降去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 折射去后摇;
        Item._切假腿配置.修改配置(Keys.W, false);
        Item._切假腿配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (Item._是否魔晶)
            {
                Item._切假腿配置.修改配置(Keys.E, true);
            }
        }
        else if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.F);
            Dota2Simulator.TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.F) == 1 ? "如影随形分身" : "关闭随形分身");
        }
    }

    private static async Task<bool> 幽鬼之刃去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1, false).ConfigureAwait(true);
    }

    private static async Task<bool> 如影随形去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.R, () =>
        {
            if (Main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                SimKeyBoard.KeyPress(Keys.D);
            }
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 空降去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.D, () =>
        {
            if (Main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                if (Item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧) == 1)
                {
                    分身一齐攻击();
                }

                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.否决));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.紫苑));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.血棘));
            }

            Item.要求保持假腿();

            SimKeyBoard.KeyPress(Keys.A);
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 折射去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    /// <summary>因为有0.1秒的分裂时间，所以必须等待——复制自 Main.分身一齐攻击。</summary>
    private static void 分身一齐攻击()
    {
        Common.Delay(140);
        SimKeyBoard.KeyDown(Keys.Control);
        SimKeyBoard.KeyPress(Keys.A);
        SimKeyBoard.KeyUp(Keys.Control);
    }
}
#endif
