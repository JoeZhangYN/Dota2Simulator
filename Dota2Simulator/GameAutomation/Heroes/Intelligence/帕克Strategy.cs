#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 帕克Strategy : IHeroStrategy
{
    public HeroId Hero => new("帕克", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 幻象法球去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 新月之痕去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 梦境缠绕去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 灵动之翼定位;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        KeyEventArgs e = new((Keys)key.ToNative() | ConvertModifiers(trigger.Modifiers));

        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

        if (e.KeyValue == (int)Keys.W && (int)e.Modifiers == (int)Keys.Control)
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
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.D);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.D) == 1 ? "传" : "不传");
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

    private static async Task<bool> 幻象法球去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.Q, () =>
        {
            Main._聚合.Skills.SetStep(SlotKey.Q, 1);
            Common.Delay(3400);
            if (Main._聚合.Skills.Mode(SlotKey.D) == 1)
            {
                SimKeyBoard.KeyPress(Keys.D);
            }

            Main._聚合.Skills.SetStep(SlotKey.Q, 0);
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 新月之痕去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 梦境缠绕去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 灵动之翼定位(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.D, () =>
        {
            SimKeyBoard.KeyPress(Keys.F1);
            SimKeyBoard.KeyPress(Keys.F1);
        }).ConfigureAwait(true);
    }
}
#endif
