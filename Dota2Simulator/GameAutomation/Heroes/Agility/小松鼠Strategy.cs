#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>小松鼠（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "小松鼠"。</summary>
public sealed class 小松鼠Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 小松鼠Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("小松鼠", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 爆栗出击去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 野地奇袭去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 一箭穿心;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 猎手旋标去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (_main._聚合.HasShard)
            {
                _main._聚合.LegSwap.配置.修改配置(Keys.F, true);
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
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.F)
        {
            if (_main._聚合.HasShard)
            {
                _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.ToggleMode(SlotKey.W);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.W) == 0 ? "种树接平A" : "种树接捆");
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            _main._聚合.Skills.ToggleMode(SlotKey.E);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.E) == 0 ? "捆接平A" : "捆接种树");
        }
    }

    private async Task<bool> 爆栗出击去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.Q, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.W) == 1)
            {
                _input.Press(VirtualKey.From(Keys.W));
            }
            else
            {
                _skill.通用技能后续动作();
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 野地奇袭去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.W, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.E) == 1)
            {
                _input.Press(VirtualKey.From(Keys.Q));
            }
            else
            {
                _skill.通用技能后续动作();
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 一箭穿心(ImageHandle 句柄)
    {
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 猎手旋标去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.F, 1).ConfigureAwait(true);
    }
}
#endif
