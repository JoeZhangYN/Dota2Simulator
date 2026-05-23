#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

using Dota2Simulator.GameAutomation.Domain.Perception;
namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>血魔（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "血魔"。</summary>
public sealed class 血魔Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 血魔Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("血魔", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 血祭去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 割裂去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 血怒去后摇;
        Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        KeyEventArgs e = new((Keys)key.ToNative() | ConvertModifiers(trigger.Modifiers));

        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

        if (e.KeyValue == (int)Keys.Q && (int)e.Modifiers == (int)Keys.Alt)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }

        if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
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

    private async Task<bool> 血祭去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.W, () =>
        {
            _input.MouseClick(MouseButton.Right);
            _input.Press(VirtualKey.From(Keys.A));

            Item.要求保持假腿();

            Common.Delay(2400);
            Point p = Control.MousePosition;
            _input.MouseMoveTo(new ScreenPoint(601, 988));
            if (Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
            {
                _input.MouseMoveTo(new ScreenPoint(p.X, p.Y));
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 割裂去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private async Task<bool> 血怒去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }
}
#endif
