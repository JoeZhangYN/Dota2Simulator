#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 女王Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 女王Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("女王", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 暗影突袭去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 闪烁去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 痛苦尖叫去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 冲击波去后摇;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
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
        else if (key == VirtualKey.From(Keys.S))
        {
            Main._中断条件 = true;
        }

        return Task.CompletedTask;
    }

    private async Task<bool> 暗影突袭去后摇(ImageHandle 句柄)
    {
        void 暗影突袭后()
        {
            Main._聚合.Skills.SetTime(SlotKey.Q, -1);
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        暗影突袭后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 闪烁去后摇(ImageHandle 句柄)
    {
        void 闪烁后()
        {
            Main._聚合.Skills.SetTime(SlotKey.W, -1);
            _input.MouseClick(MouseButton.Right);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        闪烁后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 痛苦尖叫去后摇(ImageHandle 句柄)
    {
        void 痛苦尖叫后()
        {
            Main._聚合.Skills.SetTime(SlotKey.E, -1);
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        痛苦尖叫后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 冲击波去后摇(ImageHandle 句柄)
    {
        void 冲击波后()
        {
            Main._聚合.Skills.SetTime(SlotKey.R, -1);
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        冲击波后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
