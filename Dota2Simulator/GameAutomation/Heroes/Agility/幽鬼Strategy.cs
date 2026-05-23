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

/// <summary>幽鬼（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "幽鬼"。</summary>
public sealed class 幽鬼Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 幽鬼Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("幽鬼", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 幽鬼之刃去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 如影随形去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 空降去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 折射去后摇;
        _main._聚合.LegSwap.配置.修改配置(Keys.W, false);
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
                _main._聚合.LegSwap.配置.修改配置(Keys.E, true);
            }
        }
        else if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.ToggleMode(SlotKey.F);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.F) == 1 ? "如影随形分身" : "关闭随形分身");
        }
    }

    private async Task<bool> 幽鬼之刃去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1, false).ConfigureAwait(true);
    }

    private async Task<bool> 如影随形去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.R, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                _input.Press(VirtualKey.From(Keys.D));
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 空降去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能进入CD后续(Keys.D, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                if (_item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧) == 1)
                {
                    分身一齐攻击();
                }

                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.否决));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.紫苑));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.血棘));
            }

            _item.要求保持假腿();

            _input.Press(VirtualKey.From(Keys.A));
        }).ConfigureAwait(true);
    }

    private async Task<bool> 折射去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
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
