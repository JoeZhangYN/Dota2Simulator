#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 谜团Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 谜团Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("谜团", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.F)
        {
            await Task.Run(刷新接凋零黑洞).ConfigureAwait(true);
        }
    }

    private void 刷新接凋零黑洞()
    {
        _input.Press(VirtualKey.From(Keys.X));

        for (int i = 0; i < 2; i++)
        {
            Common.Delay(等待延迟);
            _input.Press(VirtualKey.From(Keys.Z));
            _input.Press(VirtualKey.From(Keys.V));
            _input.Press(VirtualKey.From(Keys.R));
        }
    }
}
#endif
