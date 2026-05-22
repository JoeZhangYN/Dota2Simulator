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
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>火猫（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "火猫"。</summary>
public sealed class 火猫Strategy : IHeroStrategy
{
    /// <summary>1080p 增益状态栏区域——内联自 Main.buff状态技能栏。</summary>
    private static readonly Rectangle buff状态技能栏 = new(962, 826, 526, 80);

    public HeroId Hero => new("火猫", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 无影拳后续处理;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 炎阳索去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 烈火罩去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 激活残焰去后摇;
        Item._切假腿配置.修改配置(Keys.D, true);
        Item._切假腿配置.修改配置(Keys.R, false);
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
            await Task.Run(() =>
            {
                Common.Delay(330);
                Item.要求保持假腿();
            }).ConfigureAwait(false);
        }
        else if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.W);
            Dota2Simulator.TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.W) == 0 ? "不接捆" : "接捆");
        }
    }

    private static async Task<bool> 无影拳后续处理(ImageHandle 句柄)
    {
        bool b = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.火猫_无影拳, in 句柄, buff状态技能栏);

        if (b)
        {
            if (Main._聚合.Skills.Mode(SlotKey.W) == 1)
            {
                SimKeyBoard.KeyPress(Keys.Q);
            }

            Item.要求保持假腿();

            SimKeyBoard.KeyPress(Keys.A);
        }

        return await Task.FromResult(!b).ConfigureAwait(true);
    }

    private static async Task<bool> 炎阳索去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 烈火罩去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 激活残焰去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
    }
}
#endif
