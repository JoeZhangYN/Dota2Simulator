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

[HeroStrategy("干扰者", HeroAttribute.Intelligence)]
public sealed partial class 干扰者Strategy : IHeroStrategy
{


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

    private async Task<bool> 风雷之击去后摇()
    {
        return await _skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private async Task<bool> 静态风暴去后摇()
    {
        return await _skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }

    private async Task<bool> 恶念瞥视去后摇()
    {
        return await _skill.技能通用判断(Keys.W, 0, false).ConfigureAwait(true);
    }

    private async Task<bool> 动能力场去后摇()
    {
        return await _skill.技能通用判断(Keys.E, 0, false).ConfigureAwait(true);
    }

    private async Task<bool> 静态风暴动能立场风雷之击()
    {
        return _skill.DOTA2释放CD就绪技能(Keys.R)
            ? await Task.FromResult(true).ConfigureAwait(true)
            : _skill.DOTA2释放CD就绪技能(Keys.E)
                ? await Task.FromResult(true).ConfigureAwait(true)
                : _skill.DOTA2释放CD就绪技能(Keys.Q)
                    ? await Task.FromResult(true).ConfigureAwait(true)
                    : await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
