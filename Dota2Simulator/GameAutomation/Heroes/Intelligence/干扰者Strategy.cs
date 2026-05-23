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

public sealed class 干扰者Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 干扰者Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("干扰者", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 风雷之击去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 恶念瞥视去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 动能力场去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 静态风暴去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 静态风暴动能立场风雷之击;
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
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.SetStep(SlotKey.W, 0);
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
    }

    private async Task<bool> 风雷之击去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private async Task<bool> 静态风暴去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }

    private async Task<bool> 恶念瞥视去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 0, false).ConfigureAwait(true);
    }

    private async Task<bool> 动能力场去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 0, false).ConfigureAwait(true);
    }

    private async Task<bool> 静态风暴动能立场风雷之击(ImageHandle 句柄)
    {
        return _skill.DOTA2释放CD就绪技能(Keys.R, in 句柄)
            ? await Task.FromResult(true).ConfigureAwait(true)
            : _skill.DOTA2释放CD就绪技能(Keys.E, in 句柄)
                ? await Task.FromResult(true).ConfigureAwait(true)
                : _skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄)
                    ? await Task.FromResult(true).ConfigureAwait(true)
                    : await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
