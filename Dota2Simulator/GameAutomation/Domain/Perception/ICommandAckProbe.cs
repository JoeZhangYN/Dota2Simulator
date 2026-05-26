#if DOTA2
using System.Drawing;
using System.Threading.Tasks;

namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// Phase 26 B1 (2026-05-26): 命令确认探针 — 异步控制语义底层 §B 段.
/// 按下命令前 <see cref="Capture"/> ROI 像素采样 → 实际 Press → 调 <see cref="WaitForAckAsync"/> budget 内重抓比对像素差 → <see cref="AckResult.Acked"/>/<see cref="AckResult.Failed"/>.
/// 目的: 防"被控制时灰图标 → 容差判定能按 → 累积按键导致游戏异常" (Phase 26 grill 子问题 #2).
/// </summary>
public interface ICommandAckProbe
{
    /// <summary>按命令前同帧抓 ROI 像素 snapshot (sampled points), 返回签名供后续 <see cref="WaitForAckAsync"/> 比对.</summary>
    AckSignature Capture(ScreenRegion roi);

    /// <summary>命令按下后调用 — budget 内循环重抓 ROI, 任一 sample 点 RGB diff &gt; 阈值即 <see cref="AckResult.Acked"/>; budget 用尽未变化即 <see cref="AckResult.Failed"/>.</summary>
    Task<AckResult> WaitForAckAsync(AckSignature before, int budgetMs);
}

/// <summary>命令前抓的 ROI 像素签名 — sample 5 点 (4 corner + 1 center) 平衡精度与开销.</summary>
public readonly record struct AckSignature(ScreenRegion Roi, Color TopLeft, Color TopRight, Color BottomLeft, Color BottomRight, Color Center);

/// <summary>命令确认结果.</summary>
public enum AckResult
{
    /// <summary>budget 内检测到 ROI 像素显著变化 — 命令生效.</summary>
    Acked,
    /// <summary>budget 内 ROI 像素无显著变化 — 命令未生效 (灰图标 / 被控 / 命令在途未完成).</summary>
    Failed,
}
#endif
