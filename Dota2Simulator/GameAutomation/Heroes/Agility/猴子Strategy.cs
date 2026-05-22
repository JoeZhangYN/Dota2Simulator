#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>猴子（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "猴子"。</summary>
public sealed class 猴子Strategy : IHeroStrategy
{
    public HeroId Hero => new("猴子", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 灵魂之矛敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 神行百变选择幻象;
        Item._切假腿配置.修改配置(Keys.W, true, "力量");
        Item._切假腿配置.修改配置(Keys.E, false);
        Item._切假腿配置.修改配置(Keys.R, false);
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            if (!Skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
            {
                SimKeyBoard.KeyPress(Keys.E);
            }

            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            if (!Skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
            {
                SimKeyBoard.KeyPress(Keys.E);
            }

            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            if (!Skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
            {
                SimKeyBoard.KeyPress(Keys.E);
            }
        }
    }

    private static async Task<bool> 灵魂之矛敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 神行百变选择幻象(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.W, () =>
        {
            Common.Delay(1000);
            SimKeyBoard.KeyPress(Keys.D1);
            Common.Delay(33);
            SimKeyBoard.MouseRightClick();
            SimKeyBoard.KeyPress(Keys.F1);
            Item.要求保持假腿();
        }).ConfigureAwait(true);
    }
}
#endif
