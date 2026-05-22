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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 术士Strategy : IHeroStrategy
{
    public HeroId Hero => new("术士", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 致命链接去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 暗言术去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 混乱之祭去后摇;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
            Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒());
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Skills.SetTime(SlotKey.R, Common.获取当前时间毫秒());
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }

        return Task.CompletedTask;
    }

    private static async Task<bool> 致命链接去后摇(ImageHandle 句柄)
    {
        static void 致命链接后()
        {
            Skill.通用技能后续动作(false);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        致命链接后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 暗言术去后摇(ImageHandle 句柄)
    {
        static void 暗言术后()
        {
            Skill.通用技能后续动作(false);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        暗言术后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 混乱之祭去后摇(ImageHandle 句柄)
    {
        static void 混乱之祭后()
        {
            Skill.通用技能后续动作(false);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        混乱之祭后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
