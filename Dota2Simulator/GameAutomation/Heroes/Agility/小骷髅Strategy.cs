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

/// <summary>小骷髅（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "小骷髅"。</summary>
public sealed class 小骷髅Strategy : IHeroStrategy
{
    public HeroId Hero => new("小骷髅", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 扫射去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 焦油去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 死亡契约去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 骨隐步去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 炽烈火雨去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C6].Probe ??= 骷髅之军去后摇;
        Main._聚合.Attack.基础攻击前摇 = 0.4;
        Main._聚合.Attack.基础攻击间隔 = 1.7;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (Item._是否魔晶)
            {
                Item._切假腿配置.修改配置(Keys.D, true, "敏捷");
            }

            if (Item._是否神杖)
            {
                Item._切假腿配置.修改配置(Keys.F, true);
            }
        }
        else if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            if (Item._是否魔晶)
            {
                Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
            }
        }
        else if (key == VirtualKey.F)
        {
            if (Item._是否神杖)
            {
                Main._聚合.Conditions[ConditionSlotKey.C6].Active = true;
            }
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.Q);
            Dota2Simulator.TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "无脑接道具" : "手动道具");
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            if (Item._是否魔晶)
            {
                Main._聚合.Skills.ToggleMode(SlotKey.F);
                Dota2Simulator.TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.F) == 1 ? "炽烈火雨隐身" : "炽烈火雨不隐身");
            }
        }
    }

    private static async Task<bool> 扫射去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.Q, () =>
        {
            if (Main._聚合.Skills.Mode(SlotKey.Q) == 1)
            {
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.散失));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.散魂));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.否决));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.紫苑));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.血棘));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.羊刀));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.阿托斯之棍));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.缚灵锁));
            }

            Skill.通用技能后续动作();
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 焦油去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.W, () =>
        {
            _ = Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);
            Skill.通用技能后续动作();
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 死亡契约去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.E, SimKeyBoard.MouseRightClick).ConfigureAwait(true);
    }

    private static async Task<bool> 骨隐步去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.R, SimKeyBoard.MouseRightClick).ConfigureAwait(true);
    }

    private static async Task<bool> 炽烈火雨去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.F, () =>
        {
            // 持续时间施法，其实啥也不用管？
            if (Main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                Common.Delay(0);
                SimKeyBoard.KeyPress(Keys.R);
            }
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 骷髅之军去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.F, 0).ConfigureAwait(true);
    }
}
#endif
