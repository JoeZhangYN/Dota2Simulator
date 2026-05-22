#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>小强（全才）策略——迁移自 Main.根据当前英雄增强 的 case "小强"。</summary>
public sealed class 小强Strategy : IHeroStrategy
{
    public HeroId Hero => new("小强", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
        //Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 穿刺去后摇;
        //Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 神智爆裂去后摇;
        //Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 尖刺外壳去后摇;
        //Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 复仇接穿刺;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 循环接爆裂;
        Skill.重复按键执行间隔阈值 = 150;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.D3))
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = !Main._聚合.Conditions[ConditionSlotKey.C5].Active;
            Dota2Simulator.TTS.TTS.Speak(Main._聚合.Conditions[ConditionSlotKey.C5].Active ? "循环爆裂" : "终止循环");
        }
    }

    private static async Task<bool> 循环接爆裂(ImageHandle 句柄)
    {
        await Skill.技能通用判断(Keys.W, 2).ConfigureAwait(true);
        return await Task.FromResult(Main._聚合.Conditions[ConditionSlotKey.C5].Active).ConfigureAwait(true);
    }
}
#endif
