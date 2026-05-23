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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("黑鸟", HeroAttribute.Intelligence)]
public sealed class 黑鸟Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 黑鸟Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("黑鸟", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 神智之蚀去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 关接跳;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.D)
        {
            _input.Press(VirtualKey.From(Keys.W));
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            _item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
        }
        else if (key == VirtualKey.E)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }

        return Task.CompletedTask;
    }

    // todo 逻辑修改
    private async Task<bool> 关接陨星锤(ImageHandle 句柄)
    {
        int time = 0;

        Color 技能点颜色 = Color.FromArgb(203, 183, 124);

        if (ColorExtensions.ColorAEqualColorB(_main.获取指定位置颜色(909, 1008, in 句柄), 技能点颜色, 0))
        {
            time = 4000;
        }
        else if (ColorExtensions.ColorAEqualColorB(_main.获取指定位置颜色(897, 1008, in 句柄), 技能点颜色, 0))
        {
            time = 3250;
        }

        void 关后(int time, in ImageHandle 句柄)
        {
            Common.Delay(110);
            _main._聚合.Skills.SetTime(SlotKey.W, Common.获取当前时间毫秒());
            _input.MouseClick(MouseButton.Right);
            Common.Delay(150);
            _input.Press(VirtualKey.From(Keys.S));
            Common.Delay(time - 3000, _main._聚合.Skills.Time(SlotKey.W));
            if (!_main.Session!.IsPaused)
            {
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.陨星锤);
            }
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        关后(time, in 句柄);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 神智之蚀去后摇(ImageHandle 句柄)
    {
        void 神智之蚀后()
        {
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (_skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        神智之蚀后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 关接跳(ImageHandle 句柄)
    {
        return _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀) == 1
            ? await Task.FromResult(false).ConfigureAwait(true)
            : await Task.FromResult(true).ConfigureAwait(true);
    }
}
#endif
