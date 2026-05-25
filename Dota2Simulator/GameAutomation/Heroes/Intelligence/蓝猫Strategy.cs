#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("蓝猫", HeroAttribute.Intelligence)]
public sealed partial class 蓝猫Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;



    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 拉接平A;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 滚接平A;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
            await Task.Run(残影接平A).ConfigureAwait(true);
        }
        else if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D4))
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private async Task<bool> 拉接平A()
    {
        return await Task.FromResult(true).ConfigureAwait(true);
    }

    private void 残影接平A()
    {
        Common.Delay(等待延迟);
        _input.Press(VirtualKey.From(Keys.A));
    }

    private async Task<bool> 滚接平A()
    {
        return await Task.FromResult(true).ConfigureAwait(true);
    }
}
#endif
