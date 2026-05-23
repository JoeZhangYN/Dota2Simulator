#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>飞机（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "飞机"。</summary>
[HeroStrategy("飞机", HeroAttribute.Agility)]
public sealed partial class 飞机Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        //_聚合.Conditions[ConditionSlotKey.C1].Probe ??= 火箭弹幕敏捷;
        //_聚合.Conditions[ConditionSlotKey.C2].Probe ??= 追踪导弹敏捷;
        //_聚合.Conditions[ConditionSlotKey.C3].Probe ??= 高射火炮敏捷;
        //_聚合.Conditions[ConditionSlotKey.C4].Probe ??= 召唤飞弹敏捷;
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 循环火箭弹幕;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        //if (key == VirtualKey.Q)
        //{
        //    _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        //}
        //else if (key == VirtualKey.W)
        //{
        //    _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        //}
        //else if (key == VirtualKey.E)
        //{
        //    _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        //}
        //else if (key == VirtualKey.R)
        //{
        //    _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        //}
        if (key == VirtualKey.From(Keys.D3))
        {
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = !_main._聚合.Conditions[ConditionSlotKey.C5].Active;
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Conditions[ConditionSlotKey.C5].Active ? "循环弹幕" : "关闭弹幕");
        }
    }

    private async Task<bool> 火箭弹幕敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private async Task<bool> 追踪导弹敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private async Task<bool> 高射火炮敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private async Task<bool> 召唤飞弹敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private async Task<bool> 循环火箭弹幕(ImageHandle 句柄)
    {
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Q) > 400)
            await _skill.主动技能已就绪后续(Keys.Q, () =>
            {
                _input.Press(VirtualKey.From(Keys.Q));
                _main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());
            }).ConfigureAwait(true);

        return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C5].Active);
    }
}
#endif
