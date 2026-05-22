#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 夜魔Strategy : IHeroStrategy
{
    public HeroId Hero => new("夜魔", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 虚空去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 伤残恐惧去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 黑暗飞升去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 暗夜猎影去后摇;
        Item._切假腿配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (Item._是否魔晶)
            {
                Item._切假腿配置.修改配置(Keys.E, true);
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
            if (Item._是否魔晶)
            {
                Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private static async Task<bool> 虚空去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 伤残恐惧去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 暗夜猎影去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 黑暗飞升去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }
}
#endif
