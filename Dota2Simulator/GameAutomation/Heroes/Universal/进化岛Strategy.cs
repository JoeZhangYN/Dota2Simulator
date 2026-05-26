#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>进化岛策略——迁移自 _main.根据当前英雄增强 的 case "进化岛"。</summary>
[HeroStrategy("进化岛", HeroAttribute.Universal)]
public sealed partial class 进化岛Strategy : IHeroStrategy
{


    public override void OnActivate(HeroContext ctx)
    {
    }

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.D)
        {
            _ = Task.Run(() =>
            {
                Common.Delay(200);
                _input.PressViaEnigo(VirtualKey.From(Keys.A));
            }).ConfigureAwait(false);
        }
        else if (key == VirtualKey.From(Keys.T))
        {
            _ = Task.Run(() =>
            {
                Common.Delay(200);
                _input.PressViaEnigo(VirtualKey.From(Keys.A));
            }).ConfigureAwait(false);
        }
        else if (key == VirtualKey.From(Keys.F3))
        {
            _ = Task.Run(() =>
            {
                Common.Delay(200);
                _input.PressViaEnigo(VirtualKey.From(Keys.A));
            }).ConfigureAwait(false);
        }

        return Task.CompletedTask;
    }
}
#endif
