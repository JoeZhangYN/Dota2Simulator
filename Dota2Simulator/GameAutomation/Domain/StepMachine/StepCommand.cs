#if DOTA2
#nullable enable
using System;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Domain.StepMachine;

/// <summary>
/// Phase 27A retry 2 S1 (2026-05-26): StepMachine DSL 原语集 v2 = 19 个.
/// 19 原语**一次性整全**, S2-S5 严禁自扩 (BLOCKED file 协议).
/// surface 说明: StepCommand 仅 Application/Domain 内部表示, 业务侧 Strategy.cs 只见 StepMachineSubBuilder 8 维 fluent API.
/// </summary>
public abstract record StepCommand;

// 1-2. 键盘原语
public sealed record Press(Keys Key, int? HoldMs = null) : StepCommand;
public sealed record PressAlt(Keys Key, bool Alt = false) : StepCommand;  // Wave 3 attempt S4 实证收编

// 3-6. 等待原语
public sealed record WaitForColor(Func<ImageHandle, bool> Probe, int TimeoutMs) : StepCommand;
public sealed record WaitForRefractoryExpire(string Key) : StepCommand;
public sealed record WaitForProbeTrue(Func<ImageHandle, bool> Probe, int IntervalMs = 33) : StepCommand;
public sealed record WaitForProbeAny(string[] ProbeNames) : StepCommand;

// 7-9. 步态原语
public sealed record SetStep(int N) : StepCommand;
public sealed record SetStepIf(Func<ImageHandle, bool> Cond, int N) : StepCommand;
public sealed record ResetMachine() : StepCommand;

// 10-12. 控制流原语
public sealed record Conditional(Func<ImageHandle, bool> Cond, StepCommand[] IfSteps, StepCommand[] ElseSteps) : StepCommand;
public sealed record LoopUntil(Func<ImageHandle, bool> Cond, StepCommand[] Body) : StepCommand;
public sealed record AbortIf(Func<ImageHandle, bool> Cond) : StepCommand;

// 13. 并行原语 (军团 4 跳刀变体)
public sealed record ParallelBatch(StepCommand[] Commands) : StepCommand;

// 14-15. 跨 clause 副作用 (天怒 D3 + 27B+ 船长保留)
public sealed record ActivateClause(string ClauseName) : StepCommand;
public sealed record DeactivateClause(string ClauseName) : StepCommand;

// 16. 动态时间 (27B+ 船长保留, v2 一次性加避免 chunk 间扩)
public sealed record DynamicDelay(Func<ImageHandle, TimeSpan> Formula) : StepCommand;

// 17. Probe 注册 (27B+ 船长保留, v2 一次性加)
public sealed record RegisterProbe(string Name, Func<ImageHandle, bool> Probe) : StepCommand;

// 18. 物品原语 (军团 UseItem 实证收编)
public sealed record UseItem(Template Template) : StepCommand;  // Wave 3 attempt S4 实证收编 - Template 是 Dota2Simulator.GameAutomation.Domain.Perception 内类型

// 19. 显式延迟 (R8 tick 节奏)
public sealed record Delay(int Ms) : StepCommand;
#endif
