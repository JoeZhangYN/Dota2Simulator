#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>拍拍（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "拍拍"。</summary>
public sealed class 拍拍Strategy : IHeroStrategy
{
    public HeroId Hero => new("拍拍", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 震撼大地去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 超强力量去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 跳拍;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 狂怒去后摇;
        Item._切假腿配置.修改配置(Keys.E, false);
        Item._切假腿配置.修改配置(Keys.R, false);
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
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
    }

    private static async Task<bool> 超强力量去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 震撼大地去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 狂怒去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 跳拍(ImageHandle 句柄)
    {
        _ = Task.Run(() =>
        {
            if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀) == 1)
            {
                SimKeyBoard.KeyPress(Keys.A);

                _ = Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);
            }
        });

        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
