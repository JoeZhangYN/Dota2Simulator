#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 钢背Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 钢背Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("钢背", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        //Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 鼻涕去后摇;
        //// Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 针刺循环; 已优化不需要
        //Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 毛团去后摇;
        //Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 钢毛后背去后摇;
        //Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 扫射切回假腿;
        Item._切假腿配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (Item._是否魔晶)
            {
                Item._切假腿配置.修改配置(Keys.D, true);
            }

            if (Item._是否神杖)
            {
                Item._切假腿配置.修改配置(Keys.E, true);
            }
        }
        else if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            if (Item._是否魔晶)
            {
                Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }
        else if (key == VirtualKey.E)
        {
            if (Item._是否神杖)
            {
                Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
    }

    private static async Task<bool> 鼻涕去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 扫射切回假腿(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 10).ConfigureAwait(true);
    }

    private static async Task<bool> 毛团去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 钢毛后背去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0, false).ConfigureAwait(true);
    }
}
#endif
