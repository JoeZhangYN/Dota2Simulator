#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>沙王（全才）策略——迁移自 _main.根据当前英雄增强 的 case "沙王"。</summary>
[HeroStrategy("沙王", HeroAttribute.Universal)]
public sealed class 沙王Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 沙王Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("沙王", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
        _skill.重复按键执行间隔阈值 = 150;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        //if (key == VirtualKey.Q)
        //{
        //    _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        //}
        //else if (key == VirtualKey.W)
        //{
        //    Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃));
        //    Common.Delay(33 * (_item.根据图片使用物品(Dota2_Pictrue.物品.红杖) +
        //                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖2) +
        //                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖3) +
        //                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖4) +
        //                _item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)));
        //    _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        //}
        //else if (key == VirtualKey.E)
        //{
        //    _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        //}
    }
}
#endif
