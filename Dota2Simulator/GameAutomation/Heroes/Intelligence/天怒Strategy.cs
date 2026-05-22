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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 天怒Strategy : IHeroStrategy
{
    public HeroId Hero => new("天怒", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 循环奥数鹰隼;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 天怒秒人连招;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 奥数鹰隼去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 上古封印去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 神秘之耀去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C6].Probe ??= 震荡光弹去后摇;
        Skill.重复按键执行间隔阈值 = 100;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C6].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = !Main._聚合.Conditions[ConditionSlotKey.C1].Active;
            TTS.TTS.Speak(Main._聚合.Conditions[ConditionSlotKey.C1].Active ? "循环鹰隼" : "不循环鹰隼");
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            Main._聚合.Skills.SetStep(SlotKey.Global, 1);
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
    }

    private static async Task<bool> 循环奥数鹰隼(ImageHandle 句柄)
    {
        await Skill.技能通用判断(Keys.Q, 2).ConfigureAwait(true);
        return await Task.FromResult(Main._聚合.Conditions[ConditionSlotKey.C1].Active).ConfigureAwait(true);
    }

    private static async Task<bool> 天怒秒人连招(ImageHandle 句柄)
    {
        int 步骤 = Main._聚合.Skills.Step(SlotKey.Global);

        switch (步骤)
        {
            case < 2:
                if (Skill.DOTA2释放CD就绪技能(Keys.W, in 句柄))
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Skill.DOTA2释放CD就绪技能(Keys.E, in 句柄))
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                Common.Delay(0 * Item.根据图片使用物品(Dota2_Pictrue.物品.阿托斯之棍));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.缚灵锁));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃));

                Common.Delay(33 * (
                    Item.根据图片使用物品(Dota2_Pictrue.物品.红杖)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)));

                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.羊刀));

                Main._聚合.Skills.SetStep(SlotKey.Global, 2);

                return await Task.FromResult(true).ConfigureAwait(true);
            case < 3:
                {
                    if (Skill.DOTA2释放CD就绪技能(Keys.R, in 句柄))
                    {
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }

                    Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
                    Main._聚合.Skills.SetStep(SlotKey.Global, 3);

                    return await Task.FromResult(false).ConfigureAwait(true);
                }
        }

        return await Task.FromResult(true).ConfigureAwait(true);
    }

    private static async Task<bool> 奥数鹰隼去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 上古封印去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 神秘之耀去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 震荡光弹去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }
}
#endif
