#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>TB（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "TB"。</summary>
public sealed class TBStrategy : IHeroStrategy
{
    public HeroId Hero => new("TB", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 倒影敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 幻惑敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 魔化敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 恶魔狂热去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 恐怖心潮敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C6].Probe ??= 断魂敏捷;
        Item._切假腿配置.修改配置(Keys.W, false);
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (Item._是否神杖)
            {
                Item._切假腿配置.修改配置(Keys.F, true);
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
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            if (Item._是否魔晶)
            {
                Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
        else if (key == VirtualKey.F)
        {
            if (Item._是否神杖)
            {
                Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
            }
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C6].Active = true;
        }
    }

    private static async Task<bool> 倒影敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 幻惑敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 魔化敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 恶魔狂热去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 恐怖心潮敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.F, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 断魂敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }
}
#endif
