// Phase 19G-4: 伐木机 Strategy 迁 HeroPlan — RegisterStoneProbe DSL (Phase 19G-4 新增 命石单字段) + 5 CustomProbe + override OnKeyAsync 短路 D 键 StoneChoice==2 Guard.
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
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("伐木机", HeroAttribute.Strength)]
public sealed partial class 伐木机Strategy : IHeroStrategy
{
    private static readonly Rectangle 命石区域 = new(738, 945, 70, 26);

    // Phase 21A: D 键 short-circuit (Stone.Choice != 2 跳过) 改用 WhenStoneChoiceEq DSL Guard, 删 override OnKeyAsync.
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()  // C1: 死亡旋风
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()  // C2: 伐木聚链
        .OnKey(Keys.D).WhenStoneChoiceEq(2).CastSkill(Keys.D).AfterCast()  // C3: 锯齿轮旋 (Phase 21A: WhenStoneChoiceEq Guard 替原 override 短路)
        .OnKey(Keys.F).CastSkill(Keys.F).AfterEnterCD()  // C4: 喷火装置
        .OnKey(Keys.R).CustomProbe(锯齿飞轮去后摇)  // C5: 锯齿飞轮 (释放技能后替换图标技能后续 lambda)
        .RegisterStoneProbe(伐木机获取命石)  // Phase 19G-4 RegisterStoneProbe DSL
        .Done();

    private async Task<bool> 伐木机获取命石()
    {
        if (_main._聚合.Stone.Choice == 0)
        {
            if (_vision.Find(Dota2_Pictrue.命石.伐木机_碎木击_Tpl, 命石区域, new MatchRate(0.9), Tolerance.Exact).Found)
                _main._聚合.Stone.Choice = 1;
            else if (_vision.Find(Dota2_Pictrue.命石.伐木机_锯齿轮旋_Tpl, 命石区域, new MatchRate(0.9), Tolerance.Exact).Found)
            {
                _main._聚合.Stone.Choice = 2;
                _main._聚合.LegSwap.配置.修改配置(Keys.D, true);
            }
        }
        _main._聚合.Stone.Probe = null;
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 锯齿飞轮去后摇()
    {
        return await _skill.释放技能后替换图标技能后续(Keys.R, () => _main._聚合.Skills.Step(Domain.Loop.SlotKey.R), v => _main._聚合.Skills.SetStep(Domain.Loop.SlotKey.R, v)).ConfigureAwait(true);
    }
}
#endif
