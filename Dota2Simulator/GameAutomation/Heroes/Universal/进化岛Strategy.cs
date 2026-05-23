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
public sealed class 进化岛Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 进化岛Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
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
