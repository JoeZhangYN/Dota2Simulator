#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>马西（全才）策略——迁移自 Main.根据当前英雄增强 的 case "马西"。</summary>
public sealed class 马西Strategy : IHeroStrategy
{
    private static readonly Rectangle buff状态技能栏 = new(962, 826, 526, 80);

    public HeroId Hero => new("马西", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 幽魂检测;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.W)
        {
            if (Item._是否神杖)
            {
                return;
            }

            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
    }

    private static async Task<bool> 幽魂检测(ImageHandle 句柄)
    {
        return ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.小精灵_幽魂, in 句柄, buff状态技能栏)
            ? await Task.FromResult(true).ConfigureAwait(true)
            : await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
