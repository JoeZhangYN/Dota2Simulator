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

public sealed class 术士Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 术士Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("术士", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 致命链接去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 暗言术去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 混乱之祭去后摇;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
          _item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
          _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
          _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
          _main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒());
        }
        else if (key == VirtualKey.R)
        {
          _main._聚合.Skills.SetTime(SlotKey.R, Common.获取当前时间毫秒());
          _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }

        return Task.CompletedTask;
    }

    private async Task<bool> 致命链接去后摇(ImageHandle 句柄)
    {
        void 致命链接后()
        {
          _skill.通用技能后续动作(false);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
          return await Task.FromResult(true).ConfigureAwait(true);
        }

        致命链接后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 暗言术去后摇(ImageHandle 句柄)
    {
        void 暗言术后()
        {
          _skill.通用技能后续动作(false);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
          return await Task.FromResult(true).ConfigureAwait(true);
        }

        暗言术后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 混乱之祭去后摇(ImageHandle 句柄)
    {
        void 混乱之祭后()
        {
          _skill.通用技能后续动作(false);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
          return await Task.FromResult(true).ConfigureAwait(true);
        }

        混乱之祭后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
