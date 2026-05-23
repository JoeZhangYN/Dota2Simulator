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

public sealed class 奶绿Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 奶绿Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("奶绿", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 弹无虚发去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 唤魂去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 越界去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 临别一枪去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 祭台去后摇;
        _main._聚合.LegSwap.配置.修改配置(Keys.E, false);
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
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private async Task<bool> 弹无虚发去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 唤魂去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private async Task<bool> 越界去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 0, 判断成功后延时: 360).ConfigureAwait(true);
    }

    private async Task<bool> 临别一枪去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
    }

    private async Task<bool> 祭台去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.F, 0).ConfigureAwait(true);
    }
}
#endif
