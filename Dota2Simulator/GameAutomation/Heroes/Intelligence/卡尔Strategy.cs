#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 卡尔Strategy : IHeroStrategy
{
    public HeroId Hero => new("卡尔", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 三冰对线;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 三雷对线;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 三雷幽灵;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 极冷吹风陨星锤雷暴;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.D1))
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D4))
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
    }

    private static async Task<bool> 三冰对线(ImageHandle 句柄)
    {
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 三雷对线(ImageHandle 句柄)
    {
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 三雷幽灵(ImageHandle 句柄)
    {
        return await Task.FromResult(true).ConfigureAwait(true);
    }

    private static async Task<bool> 极冷吹风陨星锤雷暴(ImageHandle 句柄)
    {
        return await Task.FromResult(true).ConfigureAwait(true);
    }
}
#endif
