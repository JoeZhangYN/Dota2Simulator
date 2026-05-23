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

/// <summary>露娜（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "露娜"。</summary>
public sealed class 露娜Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 露娜Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("露娜", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 月光后敏捷平a;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 月刃后敏捷平a;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 月蚀后敏捷平a;
        _main._聚合.LegSwap.配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private async Task<bool> 月光后敏捷平a(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 月刃后敏捷平a(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private async Task<bool> 月蚀后敏捷平a(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }
}
#endif
