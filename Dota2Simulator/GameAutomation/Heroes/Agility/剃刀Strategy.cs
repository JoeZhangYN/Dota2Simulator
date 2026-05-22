#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>剃刀（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "剃刀"。</summary>
public sealed class 剃刀Strategy : IHeroStrategy
{
    public HeroId Hero => new("剃刀", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        //_聚合.Conditions[ConditionSlotKey.C1].Probe ??= 棒击大地去后摇;
        //_聚合.Conditions[ConditionSlotKey.C2].Probe ??= 乾坤之跃敏捷;
        //_聚合.Conditions[ConditionSlotKey.C3].Probe ??= 猴子猴孙敏捷;
        //_聚合.Conditions[ConditionSlotKey.C4].Probe ??= 大圣无限跳跃;
        //Item._切假腿配置.修改配置(Keys.Q, false);
        //Item._切假腿配置.修改配置(Keys.W, false);
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }
}
#endif
