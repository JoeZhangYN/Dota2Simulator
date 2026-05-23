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

/// <summary>小黑（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "小黑"。</summary>
public sealed class 小黑Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 小黑Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("小黑", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 狂风去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 数箭齐发去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 冰川去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            Main._聚合.Skills.SetMode(SlotKey.E, ColorExtensions.ColorAEqualColorB(Main.获取指定位置颜色(738, 957, GlobalScreenCapture.GetCurrentHandle()),
                Color.FromArgb(246, 178, 60), 0) || ColorExtensions.ColorAEqualColorB(
                Main.获取指定位置颜色(722, 957, GlobalScreenCapture.GetCurrentHandle()),
                Color.FromArgb(246, 178, 60), 0)
                ? 1
                : 0);
            if (Item._是否魔晶)
            {
                Main._聚合.LegSwap.配置.修改配置(Keys.F, true);
            }
        }
        else if (key == VirtualKey.D)
        {
            if (Main._聚合.LegSwap.条件开启切假腿)
            {
                Main._聚合.Skills.ToggleMode(SlotKey.Global);
                switch (Main._聚合.Skills.Mode(SlotKey.Global))
                {
                    case 0:
                        await Item.技能释放前切假腿("智力").ConfigureAwait(true);
                        Dota2Simulator.TTS.TTS.Speak("开启冰箭");
                        break;
                    default:
                        Item.要求保持假腿();
                        Dota2Simulator.TTS.TTS.Speak("关闭冰箭");
                        break;
                }
            }
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.F)
        {
            if (Item._是否魔晶)
            {
                Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }
    }

    private async Task<bool> 狂风去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.W, () =>
        {
            Skill.通用技能后续动作();

            if (Main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                Main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 数箭齐发去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.E, () =>
        {
            Common.Delay(Main._聚合.Skills.Mode(SlotKey.E) == 1 ? 2600 : 1300);
            _input.Press(VirtualKey.From(Keys.S));
            Skill.通用技能后续动作();

            if (Main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                Main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 冰川去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.F, () =>
        {
            Skill.通用技能后续动作();

            if (Main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                Main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true);
    }
}
#endif
