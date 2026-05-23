#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 马尔斯Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 马尔斯Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("马尔斯", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 战神迅矛去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 神之谴击去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 热血竞技场去后摇;
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
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Q);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "矛接大招" : "矛不接大招");
        }
    }

    private async Task<bool> 战神迅矛去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.Q, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.Q) == 1)
            {
                _input.Press(VirtualKey.From(Keys.R));
            }
            else
            {
                _skill.通用技能后续动作();
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 神之谴击去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private async Task<bool> 热血竞技场去后摇(ImageHandle 句柄)
    {
        return await _skill.主动技能释放后续(Keys.R, () =>
        {
            if (_skill.判断技能状态(Keys.E, 句柄, Skill.技能类型.状态))
            {
                _input.Press(VirtualKey.From(Keys.E));
            }

            _skill.通用技能后续动作();
        }).ConfigureAwait(true);
    }
}
#endif
