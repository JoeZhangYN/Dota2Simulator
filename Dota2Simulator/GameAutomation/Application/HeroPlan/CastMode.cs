// Phase 12 Chunk 3: 替 SkillEngine.技能通用判断 magic int (0/1/2/10/11) 为 typed enum.
// 数值与原 magic int 一致 (迁移期 (int)mode cast 仍 work, 见 HeroPlan.DispatchAsync 内调用).
#if DOTA2

namespace Dota2Simulator.GameAutomation.Application.HeroPlans;

/// <summary>
/// 技能释放后续判断模式 —— 与 SkillEngine.技能通用判断 的「判断模式」int 参数一一对应.
/// 业务定义 (HeroPlanBuilder.AfterX / WhenReady) 通过具名方法选 mode, 调用层无 magic int.
/// </summary>
public enum CastMode
{
    /// <summary>判断模式 0: 主动技能进入 CD 后接 (无充能进入 CD).</summary>
    AfterEnterCD = 0,

    /// <summary>判断模式 1: 释放技能有抬手 (释放变色后接平 A 或后续动作).</summary>
    AfterCast = 1,

    /// <summary>判断模式 2: 技能准备好就释放 (CD 就绪自动按一次, 不等手动按).</summary>
    WhenReady = 2,

    /// <summary>判断模式 10: 进入 CD 仅切回假腿 (不接 A, 仅 LegSwap 副作用).</summary>
    AfterEnterCDLegOnly = 10,

    /// <summary>判断模式 11: 释放技能仅切回假腿 (释放后仅 LegSwap, 不接 A).</summary>
    AfterCastLegOnly = 11,
}

#endif
