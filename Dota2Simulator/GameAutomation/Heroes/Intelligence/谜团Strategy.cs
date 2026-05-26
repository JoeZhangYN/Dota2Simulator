#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("谜团", HeroAttribute.Intelligence)]
public sealed partial class 谜团Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;



    public override void OnActivate(HeroContext ctx)
    {
    }

    public override async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
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
