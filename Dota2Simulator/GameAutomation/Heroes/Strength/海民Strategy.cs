// Phase 25A-C2 (2026-05-26): 海民 Strategy 迁 DSL — 删 override OnKeyAsync.
// E 双路由 (StoneChoice 1→C2 / 2→C4) 用同 trigger 双开 OnKey + WhenStoneChoiceEq + SetActive (C1 SetupAction GuardPredicate 修旧 bug 后生效).
// F 条件 PreAction (StoneChoice==1 时先释放 E, 后无条件 Active C3) 用 C1 SetupAction PreAction 注入: WhenStoneChoiceEq(1).Pre + SetActive(C3) / WhenStoneChoiceNotEq(1).SetActive(C3).
#if DOTA2
using System.Drawing;
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
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("海民", HeroAttribute.Strength)]
public sealed partial class 海民Strategy : IHeroStrategy
{
    private static readonly Rectangle 命石区域 = new(738, 945, 70, 26);

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()  // C1 冰片
        .RegisterProbe(ConditionSlotKey.C2, 摔角行家去后摇)  // C2 摔角行家 (E + StoneChoice==1)
        .RegisterProbe(ConditionSlotKey.C3, 海象神拳接雪球)  // C3 海象神拳接雪球 (F 触发)
        .RegisterProbe(ConditionSlotKey.C4, 酒友去后摇)  // C4 酒友 (E + StoneChoice==2)
        .RegisterStoneProbe(海民获取命石)
        // E 双路由: StoneChoice 1→C2 / 2→C4. C1 GuardPredicate 修旧 bug 后, SetupAction 也支持 WhenStoneChoiceEq.
        .OnKey(Keys.E).WhenStoneChoiceEq(1).SetActive(ConditionSlotKey.C2)
        .OnKey(Keys.E).WhenStoneChoiceEq(2).SetActive(ConditionSlotKey.C4)
        // F 条件 PreAction: StoneChoice==1 时先释放 E, 后无条件 Active C3 (C1 SetupAction PreAction 形态; 拆 2 SetupAction 表"无条件 Active + 条件 Pre"语义).
        .OnKey(Keys.F).WhenStoneChoiceEq(1).Pre(() => _skill.DOTA2释放CD就绪技能(Keys.E)).SetActive(ConditionSlotKey.C3)
        .OnKey(Keys.F).WhenStoneChoiceNotEq(1).SetActive(ConditionSlotKey.C3)
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.SetTarget(SlotKey.D, Control.MousePosition);
            TTS.TTS.Speak("确定指定地点");
        })
        .Done();

    private async Task<bool> 海民获取命石()
    {
        if (_main._聚合.Stone.Choice == 0)
        {
            _main._聚合.Stone.Choice = _vision.Find(Dota2_Pictrue.命石.海民_酒友_Tpl, 命石区域, new MatchRate(0.9), Tolerance.Exact).Found ? 2 : 1;
        }
        _main._聚合.Stone.Probe = null;
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 摔角行家去后摇() => await _skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    private async Task<bool> 酒友去后摇() => await _skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);

    private async Task<bool> 海象神拳接雪球()
    {
        return await _skill.法球技能进入CD后续(Keys.R, () =>
        {
            Point p = Control.MousePosition;
            for (int i = 0; i < 2; i++)
            {
                Common.Delay(33);
                _input.MouseMoveTo(new ScreenPoint(p.X, p.Y - 60 * i));
                Press(Keys.W);
            }
            _ = Task.Run(() =>
            {
                Common.Delay(100);
                _input.MouseMoveTo(new ScreenPoint(p.X, p.Y));
                Common.Delay(850);
                if (_main.Session!.IsPaused) return;
                _input.KeyDown(VirtualKey.From(Keys.D));
                Common.Delay(100);
                Point target = _main._聚合.Skills.Target(SlotKey.D);
                _input.MouseMoveTo(new ScreenPoint(target.X, target.Y));
                Common.Delay(100);
                _input.KeyUp(VirtualKey.From(Keys.D));
                Common.Delay(600);
                Press(Keys.W);
            });
        }).ConfigureAwait(true);
    }
}
#endif
