#if DOTA2
using System.Drawing;
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

using Dota2Simulator.GameAutomation.Domain.Perception;
namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>猛犸（全才）策略——迁移自 _main.根据当前英雄增强 的 case "猛犸"。</summary>
public sealed class 猛犸Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 猛犸Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("猛犸", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe = 震荡波去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe = 授予力量去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe = 巨角冲撞去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe = 两级反转去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe = 长角抛物去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
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

    private async Task<bool> 震荡波去后摇(ImageHandle 句柄)
    {
        void 震荡波后()
        {
            _skill.通用技能后续动作();
        }

        震荡波后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 授予力量去后摇(ImageHandle 句柄)
    {
        void 授予力量后()
        {
            _skill.通用技能后续动作();
        }

        授予力量后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 巨角冲撞去后摇(ImageHandle 句柄)
    {
        void 巨角冲撞后()
        {
            _skill.通用技能后续动作();
        }

        巨角冲撞后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 长角抛物去后摇(ImageHandle 句柄)
    {
        void 长角抛物后()
        {
            _skill.通用技能后续动作();
        }

        长角抛物后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 两级反转去后摇(ImageHandle 句柄)
    {
        void 两级反转后()
        {
            _skill.通用技能后续动作();
        }

        两级反转后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    // todo 逻辑优化 有鱼叉
    private void 跳拱指定地点()
    {
        _input.Press(VirtualKey.From(Keys.Space));
        Common.Delay(等待延迟);
        _input.Press(VirtualKey.From(Keys.D9));
        Point target = _main._聚合.Skills.Target(SlotKey.Global);
        _input.MouseMoveTo(new ScreenPoint(target.X, target.Y));
        Common.Delay(等待延迟);
        _input.Press(VirtualKey.From(Keys.E));
        Common.Delay(等待延迟);
        _input.Press(VirtualKey.From(Keys.D9));
    }

    private void 指定地点()
    {
        _main._聚合.Skills.SetTarget(SlotKey.Global, Control.MousePosition);

        Common.Delay(等待延迟);
        _input.KeyDown(VirtualKey.From(Keys.Control));
        Common.Delay(等待延迟);
        _input.Press(VirtualKey.From(Keys.D9));
        Common.Delay(等待延迟);
        _input.KeyUp(VirtualKey.From(Keys.Control));
    }
}
#endif
