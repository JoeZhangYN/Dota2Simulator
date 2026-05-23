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

/// <summary>美杜莎（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "美杜莎"。</summary>
public sealed class 美杜莎Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    public 美杜莎Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
    }
    public HeroId Hero => new("美杜莎", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 秘术异蛇去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 罗网剑阵去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 石化凝视去后摇;
        Main._聚合.LegSwap.配置.修改配置(Keys.Q, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

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

    private async Task<bool> 秘术异蛇去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private async Task<bool> 罗网剑阵去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
    }

    private async Task<bool> 石化凝视去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }
}
#endif
