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

public sealed class 莱恩Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;

    public HeroId Hero => new("莱恩", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 莱恩羊接技能;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 死亡一指去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 推推破林肯秒羊;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 羊刺刷新秒人;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            await 大招前纷争(GlobalScreenCapture.GetCurrentHandle()).ConfigureAwait(true);
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.S))
        {
            Main._中断条件 = true;
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D4) && !Main._聚合.Conditions[ConditionSlotKey.C5].Active)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
            TTS.TTS.Speak("开启刷新秒人");
        }
        else if (key == VirtualKey.From(Keys.D4))
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = false;
            TTS.TTS.Speak("关闭刷新秒人");
        }
        else if (key == VirtualKey.From(Keys.D5) && !Main._聚合.Conditions[ConditionSlotKey.C6].Active)
        {
            Main._聚合.Conditions[ConditionSlotKey.C6].Active = true;
            TTS.TTS.Speak("开启羊接吸");
        }
        else if (key == VirtualKey.From(Keys.D5))
        {
            Main._聚合.Conditions[ConditionSlotKey.C6].Active = false;
            TTS.TTS.Speak("开启羊接A");
        }
    }

    private static async Task<bool> 莱恩羊接技能(ImageHandle 句柄)
    {
        static void 莱恩羊后()
        {
            if (Main._聚合.Conditions[ConditionSlotKey.C6].Active)
            {
                SimKeyBoard.KeyPress(Keys.E);
            }
            else
            {
                SimKeyBoard.KeyPress(Keys.A);
            }
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        莱恩羊后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 死亡一指去后摇(ImageHandle 句柄)
    {
        static void 死亡一指后()
        {
            //RightClick();
            SimKeyBoard.KeyPress(Keys.A);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        死亡一指后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 大招前纷争(ImageHandle 句柄)
    {
        Common.Delay(33 * (
            Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.纷争)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)
        ));
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 推推破林肯秒羊(ImageHandle 句柄)
    {
        if (Item.根据图片使用物品(Dota2_Pictrue.物品.推推棒) == 1)
        {
            Common.Delay(等待延迟);
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        SimKeyBoard.KeyPress(Keys.W);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 羊刺刷新秒人(ImageHandle 句柄)
    {
        int 步骤 = Main._聚合.Skills.Step(SlotKey.Global);

        if (步骤 == 1)
        {
            if (Item.根据图片使用物品(Dota2_Pictrue.物品.奥术鞋) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                SimKeyBoard.KeyPress(Keys.Q);
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                SimKeyBoard.KeyPress(Keys.R);
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
        }
        else if (步骤 == 0)
        {
            if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                SimKeyBoard.KeyPress(Keys.W);
                Common.Delay(等待延迟);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                SimKeyBoard.KeyPress(Keys.Q);
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Item.根据图片使用物品(Dota2_Pictrue.物品.奥术鞋) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                SimKeyBoard.KeyPress(Keys.R);
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (Main._聚合.Conditions[ConditionSlotKey.C5].Active && Item.根据图片使用物品(Dota2_Pictrue.物品.刷新球) == 1)
            {
                Main._聚合.Skills.SetStep(SlotKey.Global, 1);
                Common.Delay(120);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
        }

        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
