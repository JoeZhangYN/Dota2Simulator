#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 混沌Strategy : IHeroStrategy
{
    public HeroId Hero => new("混沌", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 混乱之箭去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 实相裂隙去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 混沌之军去后摇;
        Item._切假腿配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Item.根据图片使用物品(Dota2_Pictrue.物品.紫苑);
            Item.根据图片使用物品(Dota2_Pictrue.物品.血棘);
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.Q);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "混乱之箭接拉" : "混乱之箭接A");
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            await 切臂章().ConfigureAwait(true);
        }
    }

    private static async Task<bool> 混乱之箭去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1, 要接的按键: Main._聚合.Skills.Mode(SlotKey.Q) == 1 ? Keys.W : Keys.A).ConfigureAwait(true);
    }

    private static async Task<bool> 实相裂隙去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 11).ConfigureAwait(true);
    }

    private static async Task<bool> 混沌之军去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    // 沿用 Main.切臂章
    private static async Task 切臂章()
    {
        Keys key = Item.根据图片获取物品按键(Dota2_Pictrue.物品.臂章_开启);
        if (key != Keys.Escape)
        {
            SimKeyBoard.KeyPress(key);
            Common.Delay(15);
            _ = Item.根据图片使用物品(Dota2_Pictrue.物品.魔棒);
            _ = Item.根据图片自我使用物品(Dota2_Pictrue.物品.吊坠);
            _ = Item.根据图片使用物品(Dota2_Pictrue.物品.仙草);
            _ = Item.根据图片使用物品(Dota2_Pictrue.物品.假腿_力量腿);
            Common.Delay(15);
            SimKeyBoard.KeyPress(key);
            Item._条件假腿敏捷 = false;
            Item.要求保持假腿();

            _ = await Task.FromResult(false).ConfigureAwait(true);
        }
    }
}
#endif
