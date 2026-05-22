#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.KeyboardMouse;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>进化岛策略——迁移自 Main.根据当前英雄增强 的 case "进化岛"。</summary>
public sealed class 进化岛Strategy : IHeroStrategy
{
    public HeroId Hero => new("进化岛", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.D)
        {
            _ = Task.Run(() =>
            {
                Common.Delay(200);
                SimEnigo.KeyPress(Keys.A);
            }).ConfigureAwait(false);
        }
        else if (key == VirtualKey.From(Keys.T))
        {
            _ = Task.Run(() =>
            {
                Common.Delay(200);
                SimEnigo.KeyPress(Keys.A);
            }).ConfigureAwait(false);
        }
        else if (key == VirtualKey.From(Keys.F3))
        {
            _ = Task.Run(() =>
            {
                Common.Delay(200);
                SimEnigo.KeyPress(Keys.A);
            }).ConfigureAwait(false);
        }

        return Task.CompletedTask;
    }
}
#endif
