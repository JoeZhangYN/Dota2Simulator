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

[HeroStrategy("双头龙", HeroAttribute.Intelligence)]
public sealed class 双头龙Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 双头龙Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("双头龙", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 冰火交加去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 冰封路径去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 烈焰焚身去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 吹风接冰封路径;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
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
        else if (key == VirtualKey.From(Keys.D3))
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
    }

    private async Task<bool> 冰火交加去后摇(ImageHandle 句柄)
    {
        void 冰火交加后()
        {
            _main._聚合.Skills.SetTime(SlotKey.Q, -1);
            _input.MouseClick(MouseButton.Right);
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Q) > 1200 && _main._聚合.Skills.Time(SlotKey.Q) != -1)
        {
            冰火交加后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        冰火交加后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 冰封路径去后摇(ImageHandle 句柄)
    {
        void 冰封路径后()
        {
            _main._聚合.Skills.SetTime(SlotKey.W, -1);
            _input.MouseClick(MouseButton.Right);
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.W) > 1200 && _main._聚合.Skills.Time(SlotKey.W) != -1)
        {
            冰封路径后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        冰封路径后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 烈焰焚身去后摇(ImageHandle 句柄)
    {
        void 烈焰焚身后()
        {
            _main._聚合.Skills.SetTime(SlotKey.R, -1);
            // RightClick();
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.R) > 1200 && _main._聚合.Skills.Time(SlotKey.R) != -1)
        {
            烈焰焚身后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        烈焰焚身后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 吹风接冰封路径(ImageHandle 句柄)
    {
        if (_item.根据图片使用物品(Dota2_Pictrue.物品.吹风) == 1)
        {
            Common.Delay(等待延迟);
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        if (!ImageFinder.FindImageInRegionBool(Dota2_Pictrue.物品.吹风, in 句柄, ItemEngine.获取物品范围(_main._聚合.SkillCount)) && _main._聚合.Skills.Time(SlotKey.Global) == -1)
        {
            _main._聚合.Skills.SetTime(SlotKey.Global, Common.获取当前时间毫秒());
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Global) < 2500 - 650 - 600)
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        _input.Press(VirtualKey.From(Keys.W));
        _main._聚合.Skills.SetTime(SlotKey.Global, -1);
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
