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

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>狼人（全才）策略——迁移自 Main.根据当前英雄增强 的 case "狼人"。</summary>
public sealed class 狼人Strategy : IHeroStrategy
{
    public HeroId Hero => new("狼人", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 招狼去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 嚎叫去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 撕咬去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 变狼去后摇;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
    }

    private static async Task<bool> 招狼去后摇(ImageHandle 句柄)
    {
        static void 招狼后()
        {
            Main._聚合.Skills.SetTime(SlotKey.Q, -1);
            SimKeyBoard.KeyPress(Keys.A);
        }

        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.Q) > 400 && Main._聚合.Skills.Time(SlotKey.Q) != -1 && Item._条件开启切假腿)
        {
            招狼后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        招狼后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 嚎叫去后摇(ImageHandle 句柄)
    {
        static void 嚎叫后()
        {
            Main._聚合.Skills.SetTime(SlotKey.W, -1);
            SimKeyBoard.KeyPress(Keys.A);
        }

        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.W) > 400 && Main._聚合.Skills.Time(SlotKey.W) != -1 && Item._条件开启切假腿)
        {
            嚎叫后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        嚎叫后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 撕咬去后摇(ImageHandle 句柄)
    {
        static void 撕咬后()
        {
            Main._聚合.Skills.SetTime(SlotKey.D, -1);
            SimKeyBoard.KeyPress(Keys.A);
        }

        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.D) > 400 && Main._聚合.Skills.Time(SlotKey.D) != -1 && Item._条件开启切假腿)
        {
            撕咬后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.D, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        撕咬后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 变狼去后摇(ImageHandle 句柄)
    {
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.R) > 1200 && Main._聚合.Skills.Time(SlotKey.R) != -1 && Item._条件开启切假腿)
        {
            Main._聚合.Skills.SetTime(SlotKey.R, -1);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        return await Task.FromResult(true).ConfigureAwait(true);
    }
}
#endif
