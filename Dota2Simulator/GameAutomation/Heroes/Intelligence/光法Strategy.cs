// Phase 16 C2: 光法 Strategy 迁 HeroPlan — Q CustomProbe (减少蓄力 Step 状态机), W AfterCast, E AfterCast, E+Alt CustomProbe (循环查克拉 Mode 2 + 返 Active 自循环) → C4, D AfterCast (释放 W 致盲之光), F AfterCast (灵光接炎阳), D2 Execute toggle C4 Active + TTS, RepeatThreshold(100).
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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("光法", HeroAttribute.Intelligence)]
public sealed partial class 光法Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .RepeatThreshold(100)
        .OnKey(Keys.Q).CustomProbe(async () =>
        {
            int step = _main._聚合.Skills.Step(SlotKey.Q);
            if (step == 1)
            {
                return await _skill.主动技能进入CD后续(Keys.Q, () =>
                {
                    _main._聚合.Skills.SetStep(SlotKey.Q, 0);
                    _main._聚合.LegSwap.配置.修改配置(Keys.Q, true);
                }).ConfigureAwait(true);
            }
            _main._聚合.Skills.SetStep(SlotKey.Q, 1);
#pragma warning disable CS0618 // V3 临时妥协调用 Find(ImageHandle, ...) 重载，V6 改 SG 生成 Template 同步删
            if (_vision.Find(Dota2_Pictrue.Buff.光法_大招, new Rectangle(962, 826, 526, 80), new MatchRate(0.9), Tolerance.Exact).Found)
#pragma warning restore CS0618
            {
                _main._聚合.LegSwap.配置.修改配置(Keys.Q, false);
                _input.MouseClick(MouseButton.Right);
            }
            _ = Task.Run(() =>
            {
                Common.Delay(2700);
                _input.Press(VirtualKey.From(Keys.Q));
            }).ConfigureAwait(false);
            return true;
        })
        .OnKey(Keys.W).CastSkill(Keys.D).AfterCast()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterCast()
        .OnKey(Keys.E, KeyModifiers.Alt).CustomProbe(async () =>
        {
            await _skill.技能通用判断(Keys.E, 2).ConfigureAwait(true);
            return _main._聚合.Conditions[ConditionSlotKey.C4].Active;
        })
        .OnKey(Keys.D).CastSkill(Keys.W).AfterCast()
        .OnKey(Keys.F).CastSkill(Keys.F).AfterCast()
        .OnKey(Keys.D2).ToggleConditionSlot(ConditionSlotKey.C4, "开启循环查克拉", "关闭循环查克拉")
        .Done();
}
#endif
