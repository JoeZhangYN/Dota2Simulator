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

public sealed class 火女Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 火女Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("火女", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 龙破斩去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 光击阵去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 神灭斩去后摇;
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
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }

        return Task.CompletedTask;
    }

    private async Task<bool> 龙破斩去后摇(ImageHandle 句柄)
    {
        void 龙破斩后()
        {
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        龙破斩后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 光击阵去后摇(ImageHandle 句柄)
    {
        void 光击阵后()
        {
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        光击阵后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 神灭斩去后摇(ImageHandle 句柄)
    {
        void 神灭斩后()
        {
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        神灭斩后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
