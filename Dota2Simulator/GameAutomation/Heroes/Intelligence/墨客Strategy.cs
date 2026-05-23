#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("墨客", HeroAttribute.Intelligence)]
public sealed class 墨客Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 墨客Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("墨客", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 命运之笔去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 幻影之拥去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 墨泳去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 缚魂去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 暗绘去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        KeyEventArgs e = new((Keys)key.ToNative() | ConvertModifiers(trigger.Modifiers));

        await _item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

        if (e.KeyValue == (int)Keys.E && (int)e.Modifiers == (int)Keys.Alt)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }

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
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
    }

    /// <summary>把领域中性的 <see cref="KeyModifiers"/> 转回 WinForms <see cref="Keys"/> 修饰键标志。</summary>
    private static Keys ConvertModifiers(KeyModifiers modifiers)
    {
        Keys result = Keys.None;
        if ((modifiers & KeyModifiers.Alt) != 0) result |= Keys.Alt;
        if ((modifiers & KeyModifiers.Control) != 0) result |= Keys.Control;
        if ((modifiers & KeyModifiers.Shift) != 0) result |= Keys.Shift;
        return result;
    }

    private async Task<bool> 命运之笔去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 幻影之拥去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private async Task<bool> 墨泳去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
    }

    private async Task<bool> 缚魂去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private async Task<bool> 暗绘去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
    }
}
#endif
