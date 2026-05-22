#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>沙王（全才）策略——迁移自 Main.根据当前英雄增强 的 case "沙王"。</summary>
public sealed class 沙王Strategy : IHeroStrategy
{
    public HeroId Hero => new("沙王", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
        Skill.重复按键执行间隔阈值 = 150;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        //if (key == VirtualKey.Q)
        //{
        //    Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        //}
        //else if (key == VirtualKey.W)
        //{
        //    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃));
        //    Common.Delay(33 * (Item.根据图片使用物品(Dota2_Pictrue.物品.红杖) +
        //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2) +
        //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3) +
        //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4) +
        //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)));
        //    Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        //}
        //else if (key == VirtualKey.E)
        //{
        //    Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        //}
    }
}
#endif
