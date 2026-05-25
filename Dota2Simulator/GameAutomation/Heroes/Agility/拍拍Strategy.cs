// Phase 14 C1: 拍拍 Strategy 迁 HeroPlan + CustomProbe DSL — Q/W/R 简单 mode 0/1/0, E 自定义 (跳拍 Task.Run + 物品组合 + DOTA2释放CD就绪技能 lambda).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>拍拍（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "拍拍"。</summary>
[HeroStrategy("拍拍", HeroAttribute.Agility)]
public sealed partial class 拍拍Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()
        .OnKey(Keys.E).CustomProbe(async () =>
        {
            _ = Task.Run(() =>
            {
                if (_item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_Tpl)
                    + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀_Tpl)
                    + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀_Tpl) == 1)
                {
                    _input.Press(VirtualKey.From(Keys.A));
                    _ = _skill.DOTA2释放CD就绪技能(Keys.Q);
                }
            });
            return await Task.FromResult(false).ConfigureAwait(true);
        })
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .LegSwap(Keys.E, alwaysSwap: false)
        .LegSwap(Keys.R, alwaysSwap: false)
        .Done();
}
#endif
