#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 哈斯卡Strategy : IHeroStrategy
{
    public HeroId Hero => new("哈斯卡", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 心炎去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 牺牲去后摇;
        Item._切假腿配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
    }

    private static async Task<bool> 心炎去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 牺牲去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.R, () =>
        {
            SimKeyBoard.MouseRightClick();

            if (Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
            {
                return;
            }

            SimKeyBoard.KeyPress(Keys.A);
        }).ConfigureAwait(true);
    }
}
#endif
