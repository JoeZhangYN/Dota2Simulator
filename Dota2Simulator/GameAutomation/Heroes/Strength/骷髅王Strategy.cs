// Phase 12 Chunk 3c: 骷髅王 Strategy 迁 HeroPlan DSL + 删命石死代码.
// 命石业务已废弃 (用户告知 2026-05-25):
//   - 删 OnActivate 内 Conditions.StoneProbe ??= 骷髅王获取命石 (StoneChoice 永不更新, 死路径).
//   - 删 OnKeyAsync 内 if (StoneChoice == 1) { LegSwap.修改配置(W, true); C2.Active = true; } (整 W 块死).
//   - 删 helper 骷髅王获取命石 (ImageFinder 命石识别死代码).
//   - 删 helper 白骨守卫去后摇 (W 键已无激活路径 + C2 不再注册 Probe).
// 剩余有效逻辑: Q 键 → 冥火爆击; LegSwap.W/E 初始化配置 (W=false 表非神杖路径).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("骷髅王", HeroAttribute.Strength)]
public sealed partial class 骷髅王Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueAttack: true)
        .LegSwap(Keys.W, alwaysSwap: false)
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();
}
#endif
