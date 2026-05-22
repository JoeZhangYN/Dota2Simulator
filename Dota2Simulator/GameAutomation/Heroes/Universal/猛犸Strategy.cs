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

/// <summary>猛犸（全才）策略——迁移自 Main.根据当前英雄增强 的 case "猛犸"。</summary>
public sealed class 猛犸Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;

    public HeroId Hero => new("猛犸", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe = 震荡波去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe = 授予力量去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe = 巨角冲撞去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe = 两级反转去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe = 长角抛物去后摇;
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
        else if (key == VirtualKey.F)
        {
            await Task.Run(跳拱指定地点).ConfigureAwait(false);
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            await Task.Run(指定地点).ConfigureAwait(false);
        }
    }

    private static async Task<bool> 震荡波去后摇(ImageHandle 句柄)
    {
        static void 震荡波后()
        {
            Skill.通用技能后续动作();
        }

        震荡波后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 授予力量去后摇(ImageHandle 句柄)
    {
        static void 授予力量后()
        {
            Skill.通用技能后续动作();
        }

        授予力量后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 巨角冲撞去后摇(ImageHandle 句柄)
    {
        static void 巨角冲撞后()
        {
            Skill.通用技能后续动作();
        }

        巨角冲撞后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 长角抛物去后摇(ImageHandle 句柄)
    {
        static void 长角抛物后()
        {
            Skill.通用技能后续动作();
        }

        长角抛物后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 两级反转去后摇(ImageHandle 句柄)
    {
        static void 两级反转后()
        {
            Skill.通用技能后续动作();
        }

        两级反转后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    // todo 逻辑优化 有鱼叉
    private static void 跳拱指定地点()
    {
        SimKeyBoard.KeyPress(Keys.Space);
        Common.Delay(等待延迟);
        SimKeyBoard.KeyPress(Keys.D9);
        SimKeyBoard.MouseMove(Main._聚合.Skills.Target(SlotKey.Global));
        Common.Delay(等待延迟);
        SimKeyBoard.KeyPress(Keys.E);
        Common.Delay(等待延迟);
        SimKeyBoard.KeyPress(Keys.D9);
    }

    private static void 指定地点()
    {
        Main._聚合.Skills.SetTarget(SlotKey.Global, Control.MousePosition);

        Common.Delay(等待延迟);
        SimKeyBoard.KeyDown(Keys.Control);
        Common.Delay(等待延迟);
        SimKeyBoard.KeyPress(Keys.D9);
        Common.Delay(等待延迟);
        SimKeyBoard.KeyUp(Keys.Control);
    }
}
#endif
