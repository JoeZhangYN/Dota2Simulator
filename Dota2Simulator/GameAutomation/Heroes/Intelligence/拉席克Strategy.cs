#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 拉席克Strategy : IHeroStrategy
{
    public HeroId Hero => new("拉席克", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 撕裂大地去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 恶魔敕令去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 闪电风暴去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 脉冲新星去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 虚无主义去后摇;
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
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
    }

    private static async Task<bool> 撕裂大地去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 恶魔敕令去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 闪电风暴去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 脉冲新星去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 虚无主义去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
    }
}
#endif
