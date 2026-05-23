#if DOTA2
using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("冰女", HeroAttribute.Intelligence)]
public sealed partial class 冰女Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        return Task.CompletedTask;
    }
}
#endif
