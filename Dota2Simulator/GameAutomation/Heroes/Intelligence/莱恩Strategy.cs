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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("莱恩", HeroAttribute.Intelligence)]
public sealed class 莱恩Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 莱恩Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("莱恩", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 莱恩羊接技能;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 死亡一指去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 推推破林肯秒羊;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 羊刺刷新秒人;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            await 大招前纷争(GlobalScreenCapture.GetCurrentHandle()).ConfigureAwait(true);
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.S))
        {
            _main.Session!.IsPaused = true;
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D4) && !_main._聚合.Conditions[ConditionSlotKey.C5].Active)
        {
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
            TTS.TTS.Speak("开启刷新秒人");
        }
        else if (key == VirtualKey.From(Keys.D4))
        {
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = false;
            TTS.TTS.Speak("关闭刷新秒人");
        }
        else if (key == VirtualKey.From(Keys.D5) && !_main._聚合.Conditions[ConditionSlotKey.C6].Active)
        {
            _main._聚合.Conditions[ConditionSlotKey.C6].Active = true;
            TTS.TTS.Speak("开启羊接吸");
        }
        else if (key == VirtualKey.From(Keys.D5))
        {
            _main._聚合.Conditions[ConditionSlotKey.C6].Active = false;
            TTS.TTS.Speak("开启羊接A");
        }
    }

    private async Task<bool> 莱恩羊接技能(ImageHandle 句柄)
    {
        void 莱恩羊后()
        {
            if (_main._聚合.Conditions[ConditionSlotKey.C6].Active)
            {
                _input.Press(VirtualKey.From(Keys.E));
            }
            else
            {
                _input.Press(VirtualKey.From(Keys.A));
            }
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        莱恩羊后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 死亡一指去后摇(ImageHandle 句柄)
    {
        void 死亡一指后()
        {
            //RightClick();
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (_skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        死亡一指后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 大招前纷争(ImageHandle 句柄)
    {
        Common.Delay(33 * (
            _item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.纷争)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖2)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖3)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖4)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)
        ));
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

    private async Task<bool> 羊刺刷新秒人(ImageHandle 句柄)
    {
        int 步骤 = _main._聚合.Skills.Step(SlotKey.Global);

        if (步骤 == 1)
        {
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.奥术鞋) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_item.根据图片使用物品(Dota2_Pictrue.物品.魂戒) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                _input.Press(VirtualKey.From(Keys.Q));
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                _input.Press(VirtualKey.From(Keys.R));
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
        }
        else if (步骤 == 0)
        {
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.跳刀) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                _input.Press(VirtualKey.From(Keys.W));
                Common.Delay(等待延迟);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                _input.Press(VirtualKey.From(Keys.Q));
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_item.根据图片使用物品(Dota2_Pictrue.物品.奥术鞋) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_item.根据图片使用物品(Dota2_Pictrue.物品.魂戒) == 1)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                _input.Press(VirtualKey.From(Keys.R));
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (_main._聚合.Conditions[ConditionSlotKey.C5].Active && _item.根据图片使用物品(Dota2_Pictrue.物品.刷新球) == 1)
            {
                _main._聚合.Skills.SetStep(SlotKey.Global, 1);
                Common.Delay(120);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
        }

        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
