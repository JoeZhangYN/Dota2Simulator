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

[HeroStrategy("破晓晨星", HeroAttribute.Strength)]
public sealed partial class 破晓晨星Strategy : IHeroStrategy
{


    public override void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 上界重锤去后摇;
    }

    public override async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
    }

    private async Task<bool> 上界重锤去后摇()
    {
        return await _skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }
}
#endif
