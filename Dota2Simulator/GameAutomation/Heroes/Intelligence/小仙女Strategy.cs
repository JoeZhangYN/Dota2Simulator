#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 小仙女Strategy : IHeroStrategy
{
    public HeroId Hero => new("小仙女", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 无限暗影之境;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.F)
        {
            if (Item._是否魔晶)
            {
                Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = !Main._聚合.Conditions[ConditionSlotKey.C5].Active;
            TTS.TTS.Speak(Main._聚合.Conditions[ConditionSlotKey.C5].Active ? "续暗影" : "不续暗影");
        }
    }

    private static async Task<bool> 无限暗影之境(ImageHandle 句柄)
    {
        await Skill.技能通用判断(Keys.W, 2).ConfigureAwait(true);
        return await Task.FromResult(Main._聚合.Conditions[ConditionSlotKey.C5].Active).ConfigureAwait(true);
    }
}
#endif
