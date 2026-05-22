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

public sealed class 炸弹人Strategy : IHeroStrategy
{
    public HeroId Hero => new("炸弹人", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 粘性炸弹去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 活性电击去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 爆破起飞去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 爆破后接3雷粘性炸弹;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
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
            Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.E);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.E) == 0 ? "起飞后不接3连炸弹" : "起飞后接3连炸弹");
        }

        return Task.CompletedTask;
    }

    private static async Task<bool> 粘性炸弹去后摇(ImageHandle 句柄)
    {
        static void 粘性炸弹后()
        {
            //RightClick();
            SimKeyBoard.KeyPress(Keys.A);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        粘性炸弹后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 活性电击去后摇(ImageHandle 句柄)
    {
        static void 活性电击后()
        {
            //RightClick();
            SimKeyBoard.KeyPress(Keys.A);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        活性电击后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 爆破起飞去后摇(ImageHandle 句柄)
    {
        static void 爆破起飞后()
        {
            //RightClick();
            SimKeyBoard.KeyPress(Keys.A);
            Common.Delay(750);

            switch (Main._聚合.Skills.Mode(SlotKey.E))
            {
                case 1:
                    Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
                    Main._聚合.Skills.SetTarget(SlotKey.R, Control.MousePosition);
                    Main._聚合.Skills.SetTime(SlotKey.R, Common.获取当前时间毫秒());
                    break;
                case 0:
                    break;
            }
        }

        if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        爆破起飞后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    // todo 逻辑修改
    private static async Task<bool> 爆破后接3雷粘性炸弹(ImageHandle 句柄)
    {
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.R) >= 3000)
        {
            Main._聚合.Skills.SetTime(SlotKey.R, -1);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
