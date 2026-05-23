#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 大鱼人Strategy : IHeroStrategy
{
    /// <summary>基准帧延迟（沿用 Main.等待延迟）。</summary>
    private const int 等待延迟 = 33;


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 大鱼人Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("大鱼人", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 踩去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 踩去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 雾霭去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 跳刀接踩;
        // Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
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
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
    }

    private static async Task<bool> 跳刀接踩(ImageHandle 句柄)
    {
        if (Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒) == 1)
        {
            Common.Delay(等待延迟);
        }

        if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀) == 1
           )
        {
            Common.Delay(等待延迟);
        }

        _ = Skill.DOTA2释放CD就绪技能(Keys.W, in 句柄);

        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 守卫冲刺去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 踩去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1, 要接的按键: Main._聚合.HasShard ? Keys.A : Keys.R).ConfigureAwait(true);
    }

    private static async Task<bool> 雾霭去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }
}
#endif
