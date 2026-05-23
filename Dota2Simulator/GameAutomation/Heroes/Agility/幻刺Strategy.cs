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

/// <summary>幻刺（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "幻刺"。</summary>
public sealed class 幻刺Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 幻刺Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("幻刺", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 窒息短匕敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 幻影突袭敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 魅影无形敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 刀阵旋风敏捷;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (Main._聚合.HasAghanim)
            {
                Main._聚合.LegSwap.配置.修改配置(Keys.D, true);
            }
        }
        else if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            // 触发激怒机制，2.3秒内不吸引仇恨
            _input.Press(VirtualKey.From(Keys.A));
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            if (Main._聚合.HasAghanim)
            {
                Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
    }

    private async Task<bool> 窒息短匕敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 幻影突袭敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private async Task<bool> 魅影无形敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0, false).ConfigureAwait(true);
    }

    private async Task<bool> 刀阵旋风敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
    }
}
#endif
