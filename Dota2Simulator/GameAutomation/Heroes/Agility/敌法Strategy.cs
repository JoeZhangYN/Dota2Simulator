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

/// <summary>敌法（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "敌法"。</summary>
[HeroStrategy("敌法", HeroAttribute.Agility)]
public sealed class 敌法Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 敌法Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("敌法", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 闪烁敏捷;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 法术反制敏捷;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 法力虚空取消后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 友军法术反制敏捷;
        _main._聚合.LegSwap.配置.修改配置(Keys.Q, false);
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
        else if (key == VirtualKey.W)
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
        else if (key == VirtualKey.D)
        {
            if (_main._聚合.HasShard)
            {
                _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.SetMode(SlotKey.W, 1);
            Dota2Simulator.TTS.TTS.Speak("闪烁分身晕锤一次");
        }
    }

    private async Task<bool> 闪烁敏捷(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.W, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.W) == 1)
            {
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧);
                分身一齐攻击();
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.深渊之刃);
                _main._聚合.Skills.SetMode(SlotKey.W, 0);
            }

            _skill.通用技能后续动作();
        }).ConfigureAwait(true);
    }

    private async Task<bool> 法力虚空取消后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private async Task<bool> 法术反制敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 10).ConfigureAwait(true);
    }

    private async Task<bool> 友军法术反制敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.D, 10).ConfigureAwait(true);
    }

    /// <summary>因为有0.1秒的分裂时间，所以必须等待——复制自 _main.分身一齐攻击。</summary>
    private void 分身一齐攻击()
    {
        Common.Delay(140);
        _input.KeyDown(VirtualKey.From(Keys.Control));
        _input.Press(VirtualKey.From(Keys.A));
        _input.KeyUp(VirtualKey.From(Keys.Control));
    }
}
#endif
