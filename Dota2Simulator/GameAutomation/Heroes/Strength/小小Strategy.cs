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

public sealed class 小小Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 小小Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("小小", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 山崩去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 投掷去后摇;
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
    }

    private static async Task<bool> 山崩去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 投掷去后摇(ImageHandle 句柄)
    {
        return await Task.Run(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                Common.Delay(20);
                Skill.通用技能后续动作();
            }

            return false;
        }).ConfigureAwait(true);
    }
}
#endif
