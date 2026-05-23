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

/// <summary>电棍（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "电棍"。</summary>
public sealed class 电棍Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 电棍Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("电棍", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 等离子场去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 静电连接去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 风暴之眼去后摇;
        Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
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
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private static async Task<bool> 等离子场去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 静电连接去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 风暴之眼去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }
}
#endif
