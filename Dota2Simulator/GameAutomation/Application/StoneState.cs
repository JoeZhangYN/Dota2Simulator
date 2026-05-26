// Phase 20D (2026-05-26): 命石子聚合 — 拆出 ConditionSlotSet 内 special-case StoneProbe/StoneChoice 双字段, 与 LegSwapState / AttackProfile 同模式独立子聚合.
// 语义差异 (vs ConditionSlot): ConditionSlot 是 event-driven (Active 触发 → Probe 跑, TickAsync 并行调度); Stone 是 polling (HeroLoopHost 显式调用一次, Probe 内部自清 Probe = null 标识完成).
// 业务范围: 海民 / 伐木机 / 骷髅王 等命石选择检测.
#if DOTA2

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 命石检测子聚合 (原 ConditionSlotSet.StoneProbe + StoneChoice 双字段).
/// 与 ConditionSlot 语义不同: 后者 event-driven (Active 触发 → Probe), 前者 polling (HeroLoopHost 显式调一次, Probe 自清).
/// 业务模板: Probe 内部 if (Choice == 0) { Find image; Choice = 1/2; } Probe = null; return false;
/// </summary>
public sealed class StoneState
{
    /// <summary>命石图像检测委托 (原 ConditionSlotSet.StoneProbe). 业务侧执行后自清 (设 null), 一次性触发.</summary>
    public ConditionDelegateBitmap? Probe { get; set; }

    /// <summary>命石选择 (原 ConditionSlotSet.StoneChoice). 多值状态 (0=未检测, 1/2=已选不同命石路径), 业务侧自定义语义.</summary>
    public int Choice { get; set; }

    /// <summary>复位 — 切英雄 / 重置时调用.</summary>
    public void Reset()
    {
        Probe = null;
        Choice = 0;
    }
}

#endif
