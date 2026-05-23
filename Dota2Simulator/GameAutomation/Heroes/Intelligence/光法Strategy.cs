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

public sealed class 光法Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 光法Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("光法", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 减少300毫秒蓄力;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 炎阳之缚去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 查克拉魔法去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 循环查克拉;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 致盲之光去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C6].Probe ??= 灵光去后摇接炎阳;
        Skill.重复按键执行间隔阈值 = 100;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        KeyEventArgs e = new((Keys)key.ToNative() | ConvertModifiers(trigger.Modifiers));

        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

        if (e.KeyValue == (int)Keys.E && (int)e.Modifiers == (int)Keys.Alt)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
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
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.F)
        {
            Main._聚合.Conditions[ConditionSlotKey.C6].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = !Main._聚合.Conditions[ConditionSlotKey.C4].Active;
            TTS.TTS.Speak(Main._聚合.Conditions[ConditionSlotKey.C4].Active ? "开启循环查克拉" : "关闭循环查克拉");
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

    private async Task<bool> 减少300毫秒蓄力(ImageHandle 句柄)
    {
        int 全局步骤 = Main._聚合.Skills.Step(SlotKey.Q);

        switch (全局步骤)
        {
            case 1:
                return await Skill.主动技能进入CD后续(Keys.Q, () =>
                {
                    Main._聚合.Skills.SetStep(SlotKey.Q, 0);
                    Main._聚合.LegSwap.配置.修改配置(Keys.Q, true);
                }).ConfigureAwait(true);
            default:
                Main._聚合.Skills.SetStep(SlotKey.Q, 1);
                if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.光法_大招, GlobalScreenCapture.GetCurrentHandle(), new System.Drawing.Rectangle(962, 826, 526, 80)))
                {
                    Main._聚合.LegSwap.配置.修改配置(Keys.Q, false);
                    _input.MouseClick(MouseButton.Right);
                }

                _ = Task.Run(() =>
                {
                    Common.Delay(2700);
                    _input.Press(VirtualKey.From(Keys.Q));
                }).ConfigureAwait(false);

                return await Task.FromResult(true).ConfigureAwait(true);
        }
    }

    private async Task<bool> 炎阳之缚去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
    }

    private async Task<bool> 查克拉魔法去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
    }

    private async Task<bool> 致盲之光去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private async Task<bool> 灵光去后摇接炎阳(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.F, 1).ConfigureAwait(true);
    }

    private async Task<bool> 循环查克拉(ImageHandle 句柄)
    {
        await Skill.技能通用判断(Keys.E, 2).ConfigureAwait(true);
        return await Task.FromResult(Main._聚合.Conditions[ConditionSlotKey.C4].Active).ConfigureAwait(true);
    }
}
#endif
