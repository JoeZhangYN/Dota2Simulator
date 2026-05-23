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

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>小鱼人（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "小鱼人"。</summary>
[HeroStrategy("小鱼人", HeroAttribute.Agility)]
public sealed partial class 小鱼人Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 黑暗契约去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 跳水去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 深海护罩去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 暗影之舞去后摇;
        // 能量转移被动计数 = 0;
        _main._聚合.Attack.基础攻击间隔 = 1.7;
        _main._聚合.Attack.基础攻击前摇 = 0.5;
        _main._聚合.LegSwap.配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (_main._聚合.HasShard)
            {
                _main._聚合.LegSwap.配置.修改配置(Keys.D, true);
            }
        }
        else if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            if (_main._聚合.HasShard)
            {
                _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            // 径直移动键位
            _input.KeyDown(VirtualKey.From(Keys.L));
            Common.Delay(33);
            _input.MouseClick(MouseButton.Right);
            Common.Delay(33);
            _input.KeyUp(VirtualKey.From(Keys.L));
            // 基本上180°310  90°170 75°135 转身定点，配合A杖效果极佳
            Common.Delay(110);
            _input.Press(VirtualKey.From(Keys.W));
        }
    }

    private async Task<bool> 黑暗契约去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private async Task<bool> 跳水去后摇(ImageHandle 句柄)
    {
        _ = Task.Run(() =>
        {
            _skill.通用技能后续动作(是否保持假腿: false);
            _main._聚合.LegSwap.需要切假腿 = false;
            Common.Delay(200);
            _item.要求保持假腿();
        });
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 深海护罩去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
    }

    private async Task<bool> 暗影之舞去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }
}
#endif
