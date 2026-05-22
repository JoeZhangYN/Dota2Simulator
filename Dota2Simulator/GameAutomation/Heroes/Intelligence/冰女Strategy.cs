#if DOTA2
using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 冰女Strategy : IHeroStrategy
{
    public HeroId Hero => new("冰女", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
    }

    public Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        return Task.CompletedTask;
    }
}
#endif
