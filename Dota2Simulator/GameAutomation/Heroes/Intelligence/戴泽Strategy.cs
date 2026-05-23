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

public sealed class 戴泽Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 戴泽Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("戴泽", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 剧毒之触去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 薄葬去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 暗影波去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 邪能去后摇;
        Main._聚合.Attack.基础攻击前摇 = 0.3;
        Main._聚合.Attack.基础攻击间隔 = 1.7;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
            Main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Skills.SetTime(SlotKey.W, Common.获取当前时间毫秒());
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒());
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Skills.SetTime(SlotKey.R, Common.获取当前时间毫秒());
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }

        return Task.CompletedTask;
    }

    private static async Task<bool> 剧毒之触去后摇(ImageHandle 句柄)
    {
        static void 剧毒之触后()
        {
            Main._聚合.Skills.SetTime(SlotKey.Q, -1);
            Skill.通用技能后续动作();
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.Q) > 1200 && Main._聚合.Skills.Time(SlotKey.Q) != -1)
        {
            剧毒之触后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        剧毒之触后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 薄葬去后摇(ImageHandle 句柄)
    {
        static void 薄葬后()
        {
            Main._聚合.Skills.SetTime(SlotKey.W, -1);
            Skill.通用技能后续动作();
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.W) > 1200 && Main._聚合.Skills.Time(SlotKey.W) != -1)
        {
            薄葬后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        薄葬后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 暗影波去后摇(ImageHandle 句柄)
    {
        static void 暗影波后()
        {
            Main._聚合.Skills.SetTime(SlotKey.E, -1);
            Skill.通用技能后续动作();
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.E) > 1200 && Main._聚合.Skills.Time(SlotKey.E) != -1)
        {
            暗影波后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        暗影波后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 邪能去后摇(ImageHandle 句柄)
    {
        static void 邪能后()
        {
            Main._聚合.Skills.SetTime(SlotKey.R, -1);
            Skill.通用技能后续动作();
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.R) > 1200 && Main._聚合.Skills.Time(SlotKey.R) != -1)
        {
            邪能后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        邪能后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
