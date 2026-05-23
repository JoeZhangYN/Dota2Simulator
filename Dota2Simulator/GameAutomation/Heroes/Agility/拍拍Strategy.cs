#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>拍拍（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "拍拍"。</summary>
public sealed class 拍拍Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    public 拍拍Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
    }
    public HeroId Hero => new("拍拍", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 震撼大地去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 超强力量去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 跳拍;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 狂怒去后摇;
        Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
        Main._聚合.LegSwap.配置.修改配置(Keys.R, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
    }

    private async Task<bool> 超强力量去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private async Task<bool> 震撼大地去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private async Task<bool> 狂怒去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }

    private async Task<bool> 跳拍(ImageHandle 句柄)
    {
        _ = Task.Run(() =>
        {
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
                + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀)
                + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀) == 1)
            {
                _input.Press(VirtualKey.From(Keys.A));

                _ = _skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);
            }
        });

        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
