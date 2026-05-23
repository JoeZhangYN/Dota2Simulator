#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 发条Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 发条Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("发条", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        //Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 回音践踏去后摇;
        //Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 灵体游魂去后摇;
        //Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 裂地沟壑去后摇;
        //Item._切假腿配置.修改配置(Keys.E, false);
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
            // 用于回收时按W
            _input.Press(VirtualKey.From(Keys.A));
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }
}
#endif
