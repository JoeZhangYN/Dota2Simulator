#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 蓝胖Strategy : IHeroStrategy
{
    public HeroId Hero => new("蓝胖", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 火焰轰爆去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 引燃去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 嗜血术去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 烈火护盾去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 未精通火焰轰爆去后摇;
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
        else if (key == VirtualKey.F)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.W);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.W) == 0 ? "引燃接轰爆" : "引燃不接轰爆");
        }

        return Task.CompletedTask;
    }

    private static async Task<bool> 火焰轰爆去后摇(ImageHandle 句柄)
    {
        static void 火焰轰爆后()
        {
            SimKeyBoard.MouseRightClick();
        }

        if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        火焰轰爆后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 引燃去后摇(ImageHandle 句柄)
    {
        static void 引燃后()
        {
            switch (Main._聚合.Skills.Mode(SlotKey.W))
            {
                case 1:
                    SimKeyBoard.KeyPress(Keys.Q);
                    break;
                default:
                    SimKeyBoard.MouseRightClick();
                    break;
            }
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        引燃后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 嗜血术去后摇(ImageHandle 句柄)
    {
        static void 嗜血术后()
        {
            SimKeyBoard.MouseRightClick();
        }

        if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        嗜血术后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 未精通火焰轰爆去后摇(ImageHandle 句柄)
    {
        static void 未精通火焰轰爆后()
        {
            SimKeyBoard.MouseRightClick();
        }

        if (Skill.DOTA2判断技能是否CD(Keys.D, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        未精通火焰轰爆后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 烈火护盾去后摇(ImageHandle 句柄)
    {
        static void 烈火护盾后()
        {
            SimKeyBoard.MouseRightClick();
        }

        if (Skill.DOTA2判断技能是否CD(Keys.F, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        烈火护盾后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
