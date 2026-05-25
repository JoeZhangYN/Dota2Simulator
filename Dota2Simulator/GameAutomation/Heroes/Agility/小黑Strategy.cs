// Phase 16 C1a: 小黑 Strategy 迁 HeroPlan — F1 Execute (颜色检测 SetMode 无 Guard) + F1+HasShard AdjustLegSwap(F,true), D Execute (条件开启切假腿 toggle + Global Mode switch 假腿切换 + TTS, async via fire-and-forget Task.Run), W/E CustomProbe + F+HasShard CustomProbe.
#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("小黑", HeroAttribute.Agility)]
public sealed partial class 小黑Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.F1).Execute(() =>
        {
            ImageHandle 句柄 = GlobalScreenCapture.GetCurrentHandle();
            _main._聚合.Skills.SetMode(SlotKey.E, ColorExtensions.ColorAEqualColorB(ImageManager.GetColor(in 句柄, 738, 957),
                Color.FromArgb(246, 178, 60), 0) || ColorExtensions.ColorAEqualColorB(
                ImageManager.GetColor(in 句柄, 722, 957),
                Color.FromArgb(246, 178, 60), 0)
                ? 1
                : 0);
        })
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.F, paramBool: true)
        .OnKey(Keys.D).Execute(() =>
        {
            if (!_main._聚合.LegSwap.条件开启切假腿) return;
            _main._聚合.Skills.ToggleMode(SlotKey.Global);
            if (_main._聚合.Skills.Mode(SlotKey.Global) == 0)
            {
                _ = Task.Run(async () =>
                {
                    await _item.技能释放前切假腿("智力").ConfigureAwait(true);
                    Dota2Simulator.TTS.TTS.Speak("开启冰箭");
                });
            }
            else
            {
                _item.要求保持假腿();
                Dota2Simulator.TTS.TTS.Speak("关闭冰箭");
            }
        })
        .OnKey(Keys.W).CustomProbe(async () => await _skill.主动技能释放后续(Keys.W, () =>
        {
            _skill.通用技能后续动作();
            if (_main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                _main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true))
        .OnKey(Keys.E).CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.E, () =>
        {
            Common.Delay(_main._聚合.Skills.Mode(SlotKey.E) == 1 ? 2600 : 1300);
            _input.Press(VirtualKey.From(Keys.S));
            _skill.通用技能后续动作();
            if (_main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                _main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true))
        .OnKey(Keys.F).WhenHasShard().CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.F, () =>
        {
            _skill.通用技能后续动作();
            if (_main._聚合.Skills.Mode(SlotKey.Global) == 1)
            {
                _main._聚合.LegSwap.需要切假腿 = false;
            }
        }).ConfigureAwait(true))
        .Done();
}
#endif
