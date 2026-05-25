#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>赏金（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "赏金"。</summary>
[HeroStrategy("赏金", HeroAttribute.Agility)]
public sealed partial class 赏金Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 飞镖接平a;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 标记去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 循环标记;
        _main._聚合.LegSwap.配置.修改配置(Keys.W, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = !_main._聚合.Conditions[ConditionSlotKey.C3].Active;
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Conditions[ConditionSlotKey.C3].Active ? "循环标记" : "不循环标记");
        }
    }

    private async Task<bool> 飞镖接平a()
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 标记去后摇()
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private async Task<bool> 循环标记()
    {
        await _skill.技能通用判断(Keys.R, 2).ConfigureAwait(true);
        return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C3].Active).ConfigureAwait(true);
    }
}
#endif
