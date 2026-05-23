#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>猴子（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "猴子"。</summary>
public sealed class 猴子Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    public 猴子Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
    }
    public HeroId Hero => new("猴子", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 灵魂之矛敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 神行百变选择幻象;
        Main._聚合.LegSwap.配置.修改配置(Keys.W, true, "力量");
        Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
        Main._聚合.LegSwap.配置.修改配置(Keys.R, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            if (!_skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
            {
                _input.Press(VirtualKey.From(Keys.E));
            }

            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            if (!_skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
            {
                _input.Press(VirtualKey.From(Keys.E));
            }

            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            if (!_skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
            {
                _input.Press(VirtualKey.From(Keys.E));
            }
        }
    }

    private async Task<bool> 灵魂之矛敏捷(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 神行百变选择幻象(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.W, () =>
        {
            Common.Delay(1000);
            _input.Press(VirtualKey.From(Keys.D1));
            Common.Delay(33);
            _input.MouseClick(MouseButton.Right);
            _input.Press(VirtualKey.From(Keys.F1));
            _item.要求保持假腿();
        }).ConfigureAwait(true);
    }
}
#endif
