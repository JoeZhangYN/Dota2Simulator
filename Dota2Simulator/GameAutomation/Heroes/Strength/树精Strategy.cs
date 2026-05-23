#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 树精Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 树精Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("树精", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 自然卷握去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 寄生种子去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 活体护甲去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 丛林之眼去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 疯狂生长去后摇;
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
        else if (key == VirtualKey.E)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            if (_main._聚合.HasAghanim)
            {
                _main._聚合.LegSwap.配置.修改配置(Keys.D, true);
                _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
    }

    private async Task<bool> 自然卷握去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 寄生种子去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private async Task<bool> 活体护甲去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
    }

    private async Task<bool> 丛林之眼去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
    }

    private async Task<bool> 疯狂生长去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }
}
#endif
