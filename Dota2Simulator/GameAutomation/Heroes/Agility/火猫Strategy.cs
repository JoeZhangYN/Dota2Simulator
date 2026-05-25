// Phase 16 C3: 火猫 Strategy 迁 HeroPlan — W CustomProbe (无影拳 ImageFinder + Mode 1 Press Q + 保持假腿 + Press A) + PostAsync (Active 后 Task.Run Delay 330 + 保持假腿), Q/E AfterEnterCD, D AfterCast, D2 Execute ToggleMode + TTS.
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

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("火猫", HeroAttribute.Agility)]
public sealed partial class 火猫Strategy : IHeroStrategy
{
    /// <summary>1080p 增益状态栏区域——内联自 _main.buff状态技能栏。</summary>
    private static readonly Rectangle buff状态技能栏 = new(962, 826, 526, 80);

    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .LegSwap(Keys.D, alwaysSwap: true)
        .LegSwap(Keys.R, alwaysSwap: false)
        .OnKey(Keys.W).PostAsync(async () => await Task.Run(() =>
        {
            Common.Delay(330);
            _item.要求保持假腿();
        }).ConfigureAwait(false)).CustomProbe(async () =>
        {
#pragma warning disable CS0618 // V3 临时妥协调用 Find(ImageHandle, ...) 重载，V6 改 SG 生成 Template 同步删
            bool b = _vision.Find(Dota2_Pictrue.Buff.火猫_无影拳, buff状态技能栏, new MatchRate(0.9), Tolerance.Exact).Found;
#pragma warning restore CS0618
            if (b)
            {
                if (_main._聚合.Skills.Mode(SlotKey.W) == 1)
                {
                    _input.Press(VirtualKey.From(Keys.Q));
                }
                _item.要求保持假腿();
                _input.Press(VirtualKey.From(Keys.A));
            }
            return await Task.FromResult(!b).ConfigureAwait(true);
        })
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()
        .OnKey(Keys.D).CastSkill(Keys.D).AfterCast()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.W);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.W) == 0 ? "不接捆" : "接捆");
        })
        .Done();
}
#endif
