#if DOTA2
using System.Windows.Forms;

namespace Dota2Simulator.GameAutomation.Domain.Items;

/// <summary>
/// Phase 26 E1 (2026-05-26): 物品元数据 — 异步控制语义底层 §E 段.
/// <see cref="IsInsertable"/>: Dota2 部分主动物品引擎自 queue (按下即触发, 即使 hero 不能行动), 部分必须 hero 能行动 (受 stun/silence 阻断).
/// 双发送路径决策: Insertable → 直 Press (ItemEngine 直发); !Insertable → DeferredQueue (经 BuffObservable state-change-driven flush 在 hero 解控瞬间补发).
/// <see cref="AckBudgetMs"/>: 命令确认等待预算 (覆写 ICommandAckProbe 默认 200ms; 慢图标可设 400ms+).
/// </summary>
public sealed record ItemDescriptor(string Name, bool IsInsertable, int? AckBudgetMs = null);
#endif
