#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 蓝猫Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;

    public HeroId Hero => new("蓝猫", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 拉接平A;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 滚接平A;
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
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D4))
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private static async Task<bool> 拉接平A(ImageHandle 句柄)
    {
        return await Task.FromResult(true).ConfigureAwait(true);
    }

    private static void 残影接平A()
    {
        Common.Delay(等待延迟);
        SimKeyBoard.KeyPress(Keys.A);
    }

    private static async Task<bool> 滚接平A(ImageHandle 句柄)
    {
        return await Task.FromResult(true).ConfigureAwait(true);
    }
}
#endif
