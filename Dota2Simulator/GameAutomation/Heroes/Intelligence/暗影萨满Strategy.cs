#if DOTA2
using System;
using System.Drawing;
using System.Globalization;
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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 暗影萨满Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    public 暗影萨满Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
    }
    public HeroId Hero => new("暗影萨满", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 苍穹振击取消后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 变羊取消后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 释放群蛇守卫取消后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 推推破林肯秒羊;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 枷锁持续施法隐身;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.物品.中立_祭礼长袍, GlobalScreenCapture.GetCurrentHandle(), ItemEngine.获取中立TP范围(Main._聚合.SkillCount)))
            {
                Main._聚合.Attack.状态抗性倍数 *= 1.1;
            }

            if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.物品.中立_永恒遗物, GlobalScreenCapture.GetCurrentHandle(), ItemEngine.获取中立TP范围(Main._聚合.SkillCount)))
            {
                Main._聚合.Attack.状态抗性倍数 *= 1.2;
            }

            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D1))
        {
            switch (Main._聚合.Skills.Mode(SlotKey.W))
            {
                case 0:
                    Main._聚合.Skills.SetMode(SlotKey.W, 1);
                    TTS.TTS.Speak("羊拉");
                    break;
                case 1:
                    Main._聚合.Skills.SetMode(SlotKey.W, 2);
                    TTS.TTS.Speak("羊电");
                    break;
                case 2:
                    Main._聚合.Skills.SetMode(SlotKey.W, 3);
                    TTS.TTS.Speak("羊电拉");
                    break;
                case 3:
                    Main._聚合.Skills.SetMode(SlotKey.W, 4);
                    TTS.TTS.Speak("羊电大拉");
                    break;
                case 4:
                    Main._聚合.Skills.SetMode(SlotKey.W, 0);
                    TTS.TTS.Speak("羊接平A");
                    break;
            }
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.Q);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.Q) == 0 ? "羊" : "电羊");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     前摇时间基本在
    /// </summary>
    private async Task<bool> 苍穹振击取消后摇(ImageHandle 句柄)
    {
        void 苍穹振击后()
        {
            switch (Main._聚合.Skills.Mode(SlotKey.Q))
            {
                case 1:
                    _input.Press(VirtualKey.From(Keys.W));
                    break;
                default:
                    _input.Press(VirtualKey.From(Keys.A));
                    break;
            }
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        苍穹振击后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    /// <summary>
    ///     前摇时间基本再380-450 之间
    /// </summary>
    private async Task<bool> 枷锁持续施法隐身(ImageHandle 句柄)
    {
        void 枷锁后(in ImageHandle 句柄)
        {
        }

        if (_skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        枷锁后(in 句柄);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 释放群蛇守卫取消后摇(ImageHandle 句柄)
    {
        void 群蛇守卫后()
        {
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (_skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        群蛇守卫后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 变羊取消后摇(ImageHandle 句柄)
    {
        void 萨满变羊后(ImageHandle 句柄)
        {
            Main._聚合.Skills.SetTime(SlotKey.W, Common.获取当前时间毫秒());

            Task.Run(() =>
            {
                int time = 1250;

                Color 技能点颜色 = Color.FromArgb(203, 183, 124);

                if (ColorExtensions.ColorAEqualColorB(Main.获取指定位置颜色(909, 1008, in 句柄), 技能点颜色, 0))
                {
                    time = 3400;
                }
                else if (ColorExtensions.ColorAEqualColorB(Main.获取指定位置颜色(897, 1008, in 句柄), 技能点颜色, 0))
                {
                    time = 2650;
                }
                else if (ColorExtensions.ColorAEqualColorB(Main.获取指定位置颜色(885, 1008, in 句柄), 技能点颜色, 0))
                {
                    time = 1900;
                }
                else if (ColorExtensions.ColorAEqualColorB(Main.获取指定位置颜色(875, 1008, in 句柄), 技能点颜色, 0))
                {
                    time = 1150;
                }

                time = Convert.ToInt32(Main._聚合.Attack.状态抗性倍数 * time);

                TTS.TTS.Speak(string.Concat("延时", time.ToString(CultureInfo.InvariantCulture)));

                _input.Press(VirtualKey.From(Keys.A));

                switch (Main._聚合.Skills.Mode(SlotKey.W))
                {
                    case 1:
                        Common.Delay(time - 435, Main._聚合.Skills.Time(SlotKey.W));
                        _input.Press(VirtualKey.From(Keys.E));
                        break;
                    case 2:
                        _input.Press(VirtualKey.From(Keys.Q));
                        break;
                    case 3:
                        _input.Press(VirtualKey.From(Keys.Q));
                        Common.Delay(time - 435, Main._聚合.Skills.Time(SlotKey.W));
                        _input.Press(VirtualKey.From(Keys.E));
                        break;
                    case 4:
                        _input.Press(VirtualKey.From(Keys.R));
                        Common.Delay(400);
                        _input.Press(VirtualKey.From(Keys.Q));
                        Common.Delay(time - 435, Main._聚合.Skills.Time(SlotKey.W));
                        _input.Press(VirtualKey.From(Keys.E));
                        break;
                }
            });
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        萨满变羊后(句柄);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 推推破林肯秒羊(ImageHandle 句柄)
    {
        if (_item.根据图片使用物品(Dota2_Pictrue.物品.推推棒) == 1)
        {
            Common.Delay(等待延迟);
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        _input.Press(VirtualKey.From(Keys.W));
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
