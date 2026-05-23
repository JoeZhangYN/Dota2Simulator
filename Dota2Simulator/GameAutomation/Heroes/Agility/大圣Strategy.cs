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

/// <summary>大圣（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "大圣"。</summary>
[HeroStrategy("大圣", HeroAttribute.Agility)]
public sealed class 大圣Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 大圣Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("大圣", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 棒击大地去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 乾坤之跃敏捷;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 猴子猴孙敏捷;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 大圣无限跳跃;
        _main._聚合.LegSwap.配置.修改配置(Keys.Q, false);
        _main._聚合.LegSwap.配置.修改配置(Keys.W, false);
        _skill.重复按键执行间隔阈值 = 100;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = !_main._聚合.Conditions[ConditionSlotKey.C4].Active;
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Conditions[ConditionSlotKey.C4].Active ? "开启无限跳跃" : "关闭无限跳跃");
        }
    }

    private async Task<bool> 棒击大地去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 乾坤之跃敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private async Task<bool> 猴子猴孙敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private async Task<bool> 大圣无限跳跃(ImageHandle 句柄)
    {
        await _skill.技能通用判断(Keys.W, 2).ConfigureAwait(true);
        return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C4].Active).ConfigureAwait(true);
    }
}
#endif
