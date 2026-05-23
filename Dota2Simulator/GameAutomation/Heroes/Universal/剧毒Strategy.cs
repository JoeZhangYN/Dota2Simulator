#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>剧毒（全才）策略——迁移自 _main.根据当前英雄增强 的 case "剧毒"。</summary>
public sealed class 剧毒Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 剧毒Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("剧毒", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 瘴气去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 蛇棒去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 恶性瘟疫去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 循环蛇棒;
        _skill.重复按键执行间隔阈值 = 100;
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
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Conditions[ConditionSlotKey.C4].Active ? "循环蛇棒" : "终止循环");
        }
        else if (key == VirtualKey.From(Keys.S))
        {
            _main.Session!.IsPaused = true;
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = false;
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = false;
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = false;
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = false;
        }
    }

    private async Task<bool> 瘴气去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 蛇棒去后摇(ImageHandle 句柄)
    {
        _input.MouseClick(MouseButton.Right);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 恶性瘟疫去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private async Task<bool> 循环蛇棒(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 2).ConfigureAwait(true);
    }
}
#endif
