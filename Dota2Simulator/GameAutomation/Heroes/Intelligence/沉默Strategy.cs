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

public sealed class 沉默Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 沉默Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("沉默", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 奥数诅咒去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 全领域沉默去后摇;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }

        return Task.CompletedTask;
    }

    private async Task<bool> 大招前纷争(ImageHandle 句柄)
    {
        Common.Delay(33 * (
            Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.纷争)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4)
            + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)
        ));
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 奥数诅咒去后摇(ImageHandle 句柄)
    {
        async void 奥数诅咒后(ImageHandle 句柄)
        {
            Main._聚合.Skills.SetTime(SlotKey.Q, -1);
            // RightClick();
            // _input.Press(VirtualKey.From(Keys.A));
            switch (Main._聚合.Skills.Mode(SlotKey.Q))
            {
                case < 1:
                    _ = await 大招前纷争(句柄).ConfigureAwait(true);
                    _input.Press(VirtualKey.From(Keys.E));
                    break;
                case 1:
                    _ = await 大招前纷争(句柄).ConfigureAwait(true);
                    Common.Delay(1300);
                    _input.Press(VirtualKey.From(Keys.E));
                    break;
                case 2:
                    _input.Press(VirtualKey.From(Keys.A));
                    break;
            }
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.Q) > 1200 && Main._聚合.Skills.Time(SlotKey.Q) != -1)
        {
            奥数诅咒后(句柄);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        奥数诅咒后(句柄);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 全领域沉默去后摇(ImageHandle 句柄)
    {
        void 全领域沉默后()
        {
            Main._聚合.Skills.SetTime(SlotKey.R, -1);
            // RightClick();
            _input.Press(VirtualKey.From(Keys.A));
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.R) > 1200 && Main._聚合.Skills.Time(SlotKey.R) != -1)
        {
            全领域沉默后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        全领域沉默后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
