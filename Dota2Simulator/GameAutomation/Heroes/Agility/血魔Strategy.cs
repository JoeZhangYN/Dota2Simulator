#if DOTA2
using System.Drawing;
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

/// <summary>血魔（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "血魔"。</summary>
public sealed class 血魔Strategy : IHeroStrategy
{
    public HeroId Hero => new("血魔", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 血祭去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 割裂去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 血怒去后摇;
        Item._切假腿配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        KeyEventArgs e = new((Keys)key.ToNative());

        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

        if (e.KeyValue == (int)Keys.Q && (int)e.Modifiers == (int)Keys.Alt)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }

        if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private static async Task<bool> 血祭去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.W, () =>
        {
            SimKeyBoard.MouseRightClick();
            SimKeyBoard.KeyPress(Keys.A);

            Item.要求保持假腿();

            Common.Delay(2400);
            Point p = Control.MousePosition;
            SimKeyBoard.MouseMove(601, 988);
            if (Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
            {
                SimKeyBoard.MouseMove(p);
            }
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 割裂去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 血怒去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }
}
#endif
