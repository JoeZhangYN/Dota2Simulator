// Phase 19D: 戴泽 Strategy 迁 HeroPlan — Q/W/E/R 4 Pre(SetTime) + CustomProbe (1200ms 超时 + CD 检查 + 通用后续 同质). OnActivate 内 基础攻击前摇/间隔 设置仍保留 (聚合属性, 与 Plan 无关).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("戴泽", HeroAttribute.Intelligence)]
public sealed partial class 戴泽Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx)
    {
        GetPlan().Apply(ctx, _skill);
        _main._聚合.Attack.基础攻击前摇 = 0.3;
        _main._聚合.Attack.基础攻击间隔 = 1.7;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).Pre(() => _main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒())).CustomProbe(剧毒之触去后摇)
        .OnKey(Keys.W).Pre(() => _main._聚合.Skills.SetTime(SlotKey.W, Common.获取当前时间毫秒())).CustomProbe(薄葬去后摇)
        .OnKey(Keys.E).Pre(() => _main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒())).CustomProbe(暗影波去后摇)
        .OnKey(Keys.R).Pre(() => _main._聚合.Skills.SetTime(SlotKey.R, Common.获取当前时间毫秒())).CustomProbe(邪能去后摇)
        .Done();

    private async Task<bool> 剧毒之触去后摇() => await 同质后摇(SlotKey.Q, Keys.Q).ConfigureAwait(true);
    private async Task<bool> 薄葬去后摇() => await 同质后摇(SlotKey.W, Keys.W).ConfigureAwait(true);
    private async Task<bool> 暗影波去后摇() => await 同质后摇(SlotKey.E, Keys.E).ConfigureAwait(true);
    private async Task<bool> 邪能去后摇() => await 同质后摇(SlotKey.R, Keys.R).ConfigureAwait(true);

    /// <summary>4 个原 helper 同质: 1200ms 超时 / CD 检查 / SetTime(-1) + 通用技能后续动作.</summary>
    private async Task<bool> 同质后摇(SlotKey slot, Keys key)
    {
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(slot) > 1200 && _main._聚合.Skills.Time(slot) != -1)
        {
            _main._聚合.Skills.SetTime(slot, -1);
            _skill.通用技能后续动作();
            return await Task.FromResult(false).ConfigureAwait(true);
        }
        if (_skill.DOTA2判断技能是否CD(key))
            return await Task.FromResult(true).ConfigureAwait(true);
        _main._聚合.Skills.SetTime(slot, -1);
        _skill.通用技能后续动作();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
