#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>巨魔（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "巨魔"。</summary>
public sealed class 巨魔Strategy : IHeroStrategy
{
    public HeroId Hero => new("巨魔", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 旋风飞斧远去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 旋风飞斧近去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 战斗专注去后摇;
        Item._切假腿配置.修改配置(Keys.Q, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private static async Task<bool> 旋风飞斧远去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 旋风飞斧近去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 战斗专注去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }
}
#endif
