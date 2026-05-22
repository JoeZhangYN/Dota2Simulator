#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.KeyboardMouse;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 谜团Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;

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

    private static void 刷新接凋零黑洞()
    {
        SimKeyBoard.KeyPress(Keys.X);

        for (int i = 0; i < 2; i++)
        {
            Common.Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.Z);
            SimKeyBoard.KeyPress(Keys.V);
            SimKeyBoard.KeyPress(Keys.R);
        }
    }
}
#endif
