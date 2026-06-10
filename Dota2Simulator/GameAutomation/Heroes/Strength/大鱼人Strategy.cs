// Phase 14 C2: 大鱼人 Strategy 迁 HeroPlan + CustomProbe DSL — Q/W 都触发 W 释放 (踩, 动态 continueKey: HasShard ? A : R), R 简单 mode 1, E 跳刀接踩 自定义 (物品组合 + DOTA2释放CD就绪技能).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("大鱼人", HeroAttribute.Strength)]
public sealed partial class 大鱼人Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(async () => await _skill.技能通用判断(Keys.W, 1, 要接的按键: _main._聚合.HasShard ? Keys.A : Keys.R).ConfigureAwait(true))
        .OnKey(Keys.W).CustomProbe(async () => await _skill.技能通用判断(Keys.W, 1, 要接的按键: _main._聚合.HasShard ? Keys.A : Keys.R).ConfigureAwait(true))
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.E).CustomProbe(async () =>
        {
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.魂戒_Tpl) == 1)
            {
                Common.Delay(等待延迟);
            }
            _item.批量使用物品(物品连招.跳刀全变体);
            _ = _skill.DOTA2释放CD就绪技能(Keys.W);
            return await Task.FromResult(false).ConfigureAwait(true);
        })
        .Done();
}
#endif
