#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 巫妖Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    public 巫妖Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
    }
    public HeroId Hero => new("巫妖", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 寒霜爆发去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 冰霜魔盾去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 阴邪凝视去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 连环霜冻去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 寒冰尖柱去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        KeyEventArgs e = new((Keys)key.ToNative() | ConvertModifiers(trigger.Modifiers));

        await _item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

        if (e.KeyValue == (int)Keys.W && (int)e.Modifiers == (int)Keys.Alt)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }

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
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
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

    private async Task<bool> 寒霜爆发去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, Main._聚合.Skills.Step(SlotKey.E) > 0 ? 11 : 1).ConfigureAwait(true);
    }

    private async Task<bool> 冰霜魔盾去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, Main._聚合.Skills.Step(SlotKey.E) > 0 ? 11 : 1, false).ConfigureAwait(true);
    }

    private async Task<bool> 阴邪凝视去后摇(ImageHandle 句柄)
    {
        if (_skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
        {
            Main._聚合.Skills.SetStep(SlotKey.E, 0);
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        return await Task.Run(async () =>
        {
            if (Main._聚合.Skills.Step(SlotKey.E) == 0)
            {
                Main._聚合.Skills.SetStep(SlotKey.E, 1);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            else if (Main._聚合.Skills.Step(SlotKey.E) == 1)
            {
                _ = Task.Run(() =>
                {
                    Common.Delay(200);
                    Main._聚合.Skills.SetStep(SlotKey.E, 2);
                });
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            else
            {
                if (!SkillEngine.DOTA2判断是否持续施法(in 句柄))
                {
                    Main._聚合.Skills.SetStep(SlotKey.E, 0);
                    _input.Press(VirtualKey.From(Keys.A));
                    _ = _item.根据图片使用物品(Dota2_Pictrue.物品.羊刀);
                    return await Task.FromResult(false).ConfigureAwait(true);
                }
                else
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
            }
        }).ConfigureAwait(false);
    }

    private async Task<bool> 连环霜冻去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, Main._聚合.Skills.Step(SlotKey.E) > 0 ? 11 : 1).ConfigureAwait(true);
    }

    private async Task<bool> 寒冰尖柱去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.D, Main._聚合.Skills.Step(SlotKey.E) > 0 ? 10 : 0).ConfigureAwait(true);
    }
}
#endif
