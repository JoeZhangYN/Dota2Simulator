#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("骷髅王", HeroAttribute.Strength)]
public sealed class 骷髅王Strategy : IHeroStrategy
{
    /// <summary>命石范围（沿用 _main.命石区域）。</summary>
    private static readonly Rectangle 命石区域 = new(738, 945, 70, 26);


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 骷髅王Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("骷髅王", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions.StoneProbe ??= 骷髅王获取命石;
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 冥火爆击去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 白骨守卫去后摇;
        _main._聚合.LegSwap.配置.修改配置(Keys.W, false);
        _main._聚合.LegSwap.配置.修改配置(Keys.E, false);
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
            if (_main._聚合.Conditions.StoneChoice == 1)
            {
                _main._聚合.LegSwap.配置.修改配置(Keys.W, true);
                _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
            }
        }
    }

    private async Task<bool> 骷髅王获取命石(ImageHandle 句柄)
    {
        if (_main._聚合.Conditions.StoneChoice == 0)
        {
            _main._聚合.Conditions.StoneChoice = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.骷髅王_白骨守卫, GlobalScreenCapture.GetCurrentHandle(), 命石区域) ? 1 : 2;
        }

        _main._聚合.Conditions.StoneProbe = null;
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 冥火爆击去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 白骨守卫去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }
}
#endif
