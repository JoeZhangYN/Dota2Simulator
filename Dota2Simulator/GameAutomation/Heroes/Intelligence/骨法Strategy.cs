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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 骨法Strategy : IHeroStrategy
{
    public HeroId Hero => new("骨法", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 幽冥轰爆去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 衰老去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 幽冥守卫去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 生命吸取去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
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
            Item.根据图片使用物品(Dota2_Pictrue.物品.希瓦);
            Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.R);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.R) == 1 ? "吸取接衰老" : "吸取不接衰老");
        }
    }

    private static async Task<bool> 幽冥轰爆去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, Main._聚合.Skills.Step(SlotKey.R) > 0 ? 10 : 0).ConfigureAwait(true);
    }

    private static async Task<bool> 衰老去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.W, () =>
        {
            Common.Delay(33 * (
                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)
            ));
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 幽冥守卫去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, Main._聚合.Skills.Step(SlotKey.R) > 0 ? 10 : 0).ConfigureAwait(true);
    }

    private static async Task<bool> 生命吸取去后摇(ImageHandle 句柄)
    {
        if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            Main._聚合.Skills.SetStep(SlotKey.R, 0);
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        return await Task.Run(async () =>
        {
            if (Main._聚合.Skills.Step(SlotKey.R) == 0)
            {
                if (Main._聚合.Skills.Mode(SlotKey.R) == 1)
                {
                    SimKeyBoard.KeyPress(Keys.W);
                }

                Main._聚合.Skills.SetStep(SlotKey.R, 1);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            else if (Main._聚合.Skills.Step(SlotKey.R) == 1)
            {
                _ = Task.Run(() =>
                {
                    Common.Delay(200);
                    Main._聚合.Skills.SetStep(SlotKey.R, 2);
                });
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            else
            {
                if (!Skill.DOTA2判断是否持续施法(in 句柄))
                {
                    Main._聚合.Skills.SetStep(SlotKey.R, 0);
                    SimKeyBoard.KeyPress(Keys.A);
                    return await Task.FromResult(false).ConfigureAwait(true);
                }
                else
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
            }
        }).ConfigureAwait(false);
    }
}
#endif
