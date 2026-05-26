// Phase 19G-4: 斧王 Strategy 迁 HeroPlan — Q/W/R Pre(_item 魂戒) + CustomProbe + E 简单 setup + D4 ToggleMode Execute + D3 Execute 快速触发激怒.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("斧王", HeroAttribute.Strength)]
public sealed partial class 斧王Strategy : IHeroStrategy
{
    private const int 等待延迟 = 33;

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.Q).Pre(() => _item.根据图片使用物品(Dota2_Pictrue.物品.魂戒_Tpl)).CustomProbe(吼去后摇)  // C1
        .OnKey(Keys.W).Pre(() => _item.根据图片使用物品(Dota2_Pictrue.物品.魂戒_Tpl)).CastSkill(Keys.W).AfterCast()  // C2: 战斗饥渴 (mode 1)
        .OnKey(Keys.R).Pre(() => _item.根据图片使用物品(Dota2_Pictrue.物品.魂戒_Tpl)).CastSkill(Keys.R).AfterCast()  // C3: 淘汰之刃 (mode 1)
        .OnKey(Keys.E).CustomProbe(跳吼)  // C4
        .OnKey(Keys.D4).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Q);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "吼接刃甲" : "吼不接刃甲");
        })
        .OnKey(Keys.D3).Execute(快速触发激怒)
        .Done();

    private async Task<bool> 吼去后摇()
    {
        return await _skill.主动技能释放后续(Keys.Q, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.Q) == 1)
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.刃甲_Tpl);
            _input.Press(VirtualKey.From(Keys.A));
            _input.Press(VirtualKey.From(Keys.W));
        }).ConfigureAwait(true);
    }

    private async Task<bool> 跳吼()
    {
        if (_item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀_Tpl) == 1)
            Common.Delay(等待延迟);
        _ = _skill.DOTA2释放CD就绪技能(Keys.Q);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private void 快速触发激怒()
    {
        var 原始位置 = Control.MousePosition;
        for (int i = 0; i < 10; i++)
        {
            _input.MouseMoveTo(new ScreenPoint(575 + 515 + 61 * i, 20));
            _input.Press(VirtualKey.From(Keys.A));
            Common.Delay(2);
        }
        _input.MouseMoveTo(new ScreenPoint(原始位置.X, 原始位置.Y));
    }
}
#endif
