#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>影魔（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "影魔"。</summary>
public sealed class 影魔Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 影魔Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("影魔", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= z炮去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= x炮去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= c炮去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 灵魂盛宴去后摇;
        //_聚合.Conditions[ConditionSlotKey.C5].Probe ??= 如影随形去后摇;
        Item._切假腿配置.修改配置(Keys.F, false);
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
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
    }

    private static async Task<bool> z炮去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1, false).ConfigureAwait(true);
    }

    private static async Task<bool> x炮去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1, false).ConfigureAwait(true);
    }

    private static async Task<bool> c炮去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 1, false).ConfigureAwait(true);
    }

    private static async Task<bool> 灵魂盛宴去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 1, false).ConfigureAwait(true);
    }
}
#endif
