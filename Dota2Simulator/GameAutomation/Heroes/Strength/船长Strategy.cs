#if DOTA2
using System;
using System.Threading;
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

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("船长", HeroAttribute.Strength)]
public sealed partial class 船长Strategy : IHeroStrategy
{
    /// <summary>E 技能并发锁（沿用 _main._全局模式e_lock）。</summary>
    private static readonly Lock _全局模式e_lock = new();



    public override void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 洪流接x回;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= x释放后相关逻辑;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= x2次释放后;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 立即释放洪流;
    }

    public override async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            if (_main._聚合.Skills.Step(SlotKey.E) == 1)
            {
                return;
            }
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.SetStep(SlotKey.R, 1);
            _input.Press(VirtualKey.From(Keys.E));
        }
    }

    private async Task<bool> 洪流接x回()
    {
        return await _skill.主动技能释放后续(Keys.Q, () =>
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
            _main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());

            // 如果E已经释放
            if (!_main.Session!.IsPaused && _main._聚合.Skills.Step(SlotKey.E) == 1)
            {
                // 1600 延迟 返回200施法时间
                Common.Delay(1350, _main._聚合.Skills.Time(SlotKey.Q));
                _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
                _input.Press(VirtualKey.From(Keys.E));
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> x释放后相关逻辑()
    {
        // 释放x后放船，x的时间3秒，船0.3秒，3.1秒延迟，控制还是得靠水起来
        return await _skill.主动技能释放后续(Keys.E, () =>
        {
            int 步骤e = _main._聚合.Skills.Step(SlotKey.E);

            if (步骤e == 1) return;

            _main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒());

            if (_main._聚合.Skills.Step(SlotKey.R) == 1)
            {
                _input.Press(VirtualKey.From(Keys.R));
                _main._聚合.Skills.SetStep(SlotKey.R, 0);
            }

            lock (_全局模式e_lock)
            {
                _main._聚合.Skills.SetStep(SlotKey.E, 1);
                _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }

            int 等待时间 = (int)Math.Floor(3000 * _main._聚合.Attack.状态抗性倍数) - 1670;
            Common.Delay(等待时间, _main._聚合.Skills.Time(SlotKey.E));
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }).ConfigureAwait(true);
    }

    private async Task<bool> x2次释放后()
    {
        return await _skill.主动技能进入CD后续(Keys.E, () =>
        {
            lock (_全局模式e_lock)
            {
                // 玲珑心，释放完后至少再等6秒，等2秒基本完事
                // 因为释放q后，会再释放一次E
                // 等待说明E已经释放过一次，还在有效范围内
                Common.Delay(2000);
                _main._聚合.Skills.SetStep(SlotKey.E, 0);
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 立即释放洪流()
    {
        return await _skill.主动技能已就绪后续(Keys.Q, () => { _input.Press(VirtualKey.From(Keys.Q)); }).ConfigureAwait(true);
    }
}
#endif
