// Phase 19D: 双头龙 Strategy 迁 HeroPlan — Q/W/R 3 CustomProbe (冰火交加/冰封路径/烈焰焚身 同质 1200ms 超时 + CD 检查) + D3 CustomProbe (吹风接冰封路径 物品组合).
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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("双头龙", HeroAttribute.Intelligence)]
public sealed partial class 双头龙Strategy : IHeroStrategy
{

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(冰火交加去后摇)
        .OnKey(Keys.W).CustomProbe(冰封路径去后摇)
        .OnKey(Keys.R).CustomProbe(烈焰焚身去后摇)
        .OnKey(Keys.D3).CustomProbe(吹风接冰封路径)
        .Done();

    private async Task<bool> 冰火交加去后摇()
    {
        void 冰火交加后()
        {
            _main._聚合.Skills.SetTime(SlotKey.Q, -1);
            _input.MouseClick(MouseButton.Right);
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Q) > 1200 && _main._聚合.Skills.Time(SlotKey.Q) != -1)
        {
            冰火交加后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q))
            return await Task.FromResult(true).ConfigureAwait(true);

        冰火交加后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 冰封路径去后摇()
    {
        void 冰封路径后()
        {
            _main._聚合.Skills.SetTime(SlotKey.W, -1);
            _input.MouseClick(MouseButton.Right);
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.W) > 1200 && _main._聚合.Skills.Time(SlotKey.W) != -1)
        {
            冰封路径后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W))
            return await Task.FromResult(true).ConfigureAwait(true);

        冰封路径后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 烈焰焚身去后摇()
    {
        void 烈焰焚身后()
        {
            _main._聚合.Skills.SetTime(SlotKey.R, -1);
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.R) > 1200 && _main._聚合.Skills.Time(SlotKey.R) != -1)
        {
            烈焰焚身后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.R))
            return await Task.FromResult(true).ConfigureAwait(true);

        烈焰焚身后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 吹风接冰封路径()
    {
        if (_item.根据图片使用物品(Dota2_Pictrue.物品.吹风_Tpl) == 1)
        {
            Common.Delay(等待延迟);
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        if (!_vision.Find(Dota2_Pictrue.物品.吹风_Tpl, ItemEngine.获取物品范围(_main._聚合.SkillCount), new MatchRate(0.9), Tolerance.Exact).Found && _main._聚合.Skills.Time(SlotKey.Global) == -1)
        {
            _main._聚合.Skills.SetTime(SlotKey.Global, Common.获取当前时间毫秒());
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Global) < 2500 - 650 - 600)
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        Press(Keys.W);
        _main._聚合.Skills.SetTime(SlotKey.Global, -1);
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
