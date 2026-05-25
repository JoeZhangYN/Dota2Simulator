// Phase 12 Chunk 3c (DSL 扩展验证): 夜魔 Strategy 迁 HeroPlan + WhenHasShard + AdjustLegSwap setup.
// 关键迁移点:
//   - F1 + HasShard → LegSwap.修改配置(E, true): 走 .OnKey(F1).WhenHasShard().AdjustLegSwap(E, true).
//   - E + HasShard → Conditions[C4].Active: 走 .OnKey(E).WhenHasShard().CastSkill(E).AfterEnterCD(...).
//   - 其它 Q/W/R: 简单 .OnKey().CastSkill().AfterX().
// 原 4 helper (虚空/伤残恐惧/暗夜猎影/黑暗飞升去后摇) 全简单一行式 → Plan 内化.
//
// 注意 ConditionSlot 槽位顺序 (业务无序号意识, Builder 按 OnKey 顺序累加 C1..Cn):
//   原 C1=虚空 (Q), C2=伤残恐惧 (W), C3=黑暗飞升 (R), C4=暗夜猎影 (E+HasShard).
//   Plan 迁 OnKey 顺序: Q→C1, W→C2, R→C3, E→C4. 与原一致.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("夜魔", HeroAttribute.Strength)]
public sealed partial class 夜魔Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        // F1 + HasShard → 改 LegSwap.E = true (神杖切假腿配置).
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.E, true)
        // 4 个 OnKey-CastSkill clause; Plan 按 OnKey 顺序占 C1..C4.
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueAttack: true)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast(continueAttack: true)
        .OnKey(Keys.E).WhenHasShard().CastSkill(Keys.E).AfterEnterCD(continueAttack: true)
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
