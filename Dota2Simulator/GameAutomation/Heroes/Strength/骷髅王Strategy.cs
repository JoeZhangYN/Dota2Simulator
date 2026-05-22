#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 骷髅王Strategy : IHeroStrategy
{
    /// <summary>命石范围（沿用 Main.命石区域）。</summary>
    private static readonly Rectangle 命石区域 = new(738, 945, 70, 26);

    public HeroId Hero => new("骷髅王", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions.StoneProbe ??= 骷髅王获取命石;
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 冥火爆击去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 白骨守卫去后摇;
        Item._切假腿配置.修改配置(Keys.W, false);
        Item._切假腿配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            if (Main._聚合.Conditions.StoneChoice == 1)
            {
                Item._切假腿配置.修改配置(Keys.W, true);
                Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
            }
        }
    }

    private static async Task<bool> 骷髅王获取命石(ImageHandle 句柄)
    {
        if (Main._聚合.Conditions.StoneChoice == 0)
        {
            Main._聚合.Conditions.StoneChoice = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.骷髅王_白骨守卫, GlobalScreenCapture.GetCurrentHandle(), 命石区域) ? 1 : 2;
        }

        Main._聚合.Conditions.StoneProbe = null;
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 冥火爆击去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 白骨守卫去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }
}
#endif
