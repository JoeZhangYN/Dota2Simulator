// Phase 19D: 狼人 Strategy 迁 HeroPlan — 4 CustomProbe helper (招狼/嚎叫/撕咬 同质 400ms 时间检查 + 假腿模板 / 变狼 1200ms 时间检查 only). Probe lambda 内访问 _main / _skill / _input 须 instance _plan lazy-init.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("狼人", HeroAttribute.Universal)]
public sealed partial class 狼人Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(招狼去后摇)
        .OnKey(Keys.W).CustomProbe(嚎叫去后摇)
        .OnKey(Keys.D).CustomProbe(撕咬去后摇)
        .OnKey(Keys.R).CustomProbe(变狼去后摇)
        .Done();

    private async Task<bool> 招狼去后摇()
    {
        void 招狼后()
        {
            _main._聚合.Skills.SetTime(SlotKey.Q, -1);
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Q) > 400 && _main._聚合.Skills.Time(SlotKey.Q) != -1 && _main._聚合.LegSwap.条件开启切假腿)
        {
            招狼后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        招狼后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 嚎叫去后摇()
    {
        void 嚎叫后()
        {
            _main._聚合.Skills.SetTime(SlotKey.W, -1);
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.W) > 400 && _main._聚合.Skills.Time(SlotKey.W) != -1 && _main._聚合.LegSwap.条件开启切假腿)
        {
            嚎叫后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        嚎叫后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 撕咬去后摇()
    {
        void 撕咬后()
        {
            _main._聚合.Skills.SetTime(SlotKey.D, -1);
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.D) > 400 && _main._聚合.Skills.Time(SlotKey.D) != -1 && _main._聚合.LegSwap.条件开启切假腿)
        {
            撕咬后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.D))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        撕咬后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 变狼去后摇()
    {
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.R) > 1200 && _main._聚合.Skills.Time(SlotKey.R) != -1 && _main._聚合.LegSwap.条件开启切假腿)
        {
            _main._聚合.Skills.SetTime(SlotKey.R, -1);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        return await Task.FromResult(true).ConfigureAwait(true);
    }
}
#endif
