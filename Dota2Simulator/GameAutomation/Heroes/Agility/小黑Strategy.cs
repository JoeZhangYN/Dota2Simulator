#if DOTA2
using System.Drawing;
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

/// <summary>小黑（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "小黑"。</summary>
[HeroStrategy("小黑", HeroAttribute.Agility)]
public sealed partial class 小黑Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 狂风去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 数箭齐发去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 冰川去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            _main._聚合.Skills.SetMode(SlotKey.E, ColorExtensions.ColorAEqualColorB(_main.获取指定位置颜色(738, 957, GlobalScreenCapture.GetCurrentHandle()),
                Color.FromArgb(246, 178, 60), 0) || ColorExtensions.ColorAEqualColorB(
                _main.获取指定位置颜色(722, 957, GlobalScreenCapture.GetCurrentHandle()),
                Color.FromArgb(246, 178, 60), 0)
                ? 1
                : 0);
            if (_main._聚合.HasShard)
            {
                _main._聚合.LegSwap.配置.修改配置(Keys.F, true);
            }
        }
        else if (key == VirtualKey.D)
        {
            if (_main._聚合.LegSwap.条件开启切假腿)
            {
                _main._聚合.Skills.ToggleMode(SlotKey.Global);
                switch (_main._聚合.Skills.Mode(SlotKey.Global))
                {
                    case 0:
                        await _item.技能释放前切假腿("智力").ConfigureAwait(true);
                        Dota2Simulator.TTS.TTS.Speak("开启冰箭");
                        break;
                    default:
                        _item.要求保持假腿();
                        Dota2Simulator.TTS.TTS.Speak("关闭冰箭");
                        break;
                }
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
        else if (key == VirtualKey.F)
        {
            if (_main._聚合.HasShard)
            {
                _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }
    }

    private async Task<bool> 狂风去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.W, () =>
        {
            _skill.通用技能后续动作();

            if (_main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                _main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 数箭齐发去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能进入CD后续(Keys.E, () =>
        {
            Common.Delay(_main._聚合.Skills.Mode(SlotKey.E) == 1 ? 2600 : 1300);
            _input.Press(VirtualKey.From(Keys.S));
            _skill.通用技能后续动作();

            if (_main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                _main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 冰川去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能进入CD后续(Keys.F, () =>
        {
            _skill.通用技能后续动作();

            if (_main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                _main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true);
    }
}
#endif
