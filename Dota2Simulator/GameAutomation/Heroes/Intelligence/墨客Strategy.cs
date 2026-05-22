#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 墨客Strategy : IHeroStrategy
{
    public HeroId Hero => new("墨客", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 命运之笔去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 幻影之拥去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 墨泳去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 缚魂去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 暗绘去后摇;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        KeyEventArgs e = new((Keys)key.ToNative());

        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

        if (e.KeyValue == (int)Keys.E && (int)e.Modifiers == (int)Keys.Alt)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }

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
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
    }

    private static async Task<bool> 命运之笔去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 幻影之拥去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 墨泳去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 缚魂去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 暗绘去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
    }
}
#endif
