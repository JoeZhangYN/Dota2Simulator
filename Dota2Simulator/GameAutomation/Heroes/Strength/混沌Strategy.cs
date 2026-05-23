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

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 混沌Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    private readonly HeroLoopHost _main;
    public 混沌Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
        _main = main;
    }
    public HeroId Hero => new("混沌", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 混乱之箭去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 实相裂隙去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 混沌之军去后摇;
        _main._聚合.LegSwap.配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _item.根据图片使用物品(Dota2_Pictrue.物品.紫苑);
            _item.根据图片使用物品(Dota2_Pictrue.物品.血棘);
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
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "混乱之箭接拉" : "混乱之箭接A");
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            await 切臂章().ConfigureAwait(true);
        }
    }

    private async Task<bool> 混乱之箭去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 1, 要接的按键: _main._聚合.Skills.Mode(SlotKey.Q) == 1 ? Keys.W : Keys.A).ConfigureAwait(true);
    }

    private async Task<bool> 实相裂隙去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 11).ConfigureAwait(true);
    }

    private async Task<bool> 混沌之军去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    // 沿用 _main.切臂章
    private async Task 切臂章()
    {
        Keys key = _item.根据图片获取物品按键(Dota2_Pictrue.物品.臂章_开启);
        if (key != Keys.Escape)
        {
            _input.Press(VirtualKey.From(key));
            Common.Delay(15);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.魔棒);
            _ = _item.根据图片自我使用物品(Dota2_Pictrue.物品.吊坠);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.仙草);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.假腿_力量腿);
            Common.Delay(15);
            _input.Press(VirtualKey.From(key));
            _main._聚合.LegSwap.条件假腿敏捷 = false;
            _item.要求保持假腿();

            _ = await Task.FromResult(false).ConfigureAwait(true);
        }
    }
}
#endif
