#if DOTA2
#nullable enable
using System.Collections.Immutable;

namespace Dota2Simulator.GameAutomation.Domain.StepMachine;

/// <summary>
/// Phase 27A retry 2 S1 (2026-05-26): 单 step 定义 — N (step 编号) + 主 Commands + 可选 OnEntry/OnExit hook.
/// readonly record struct: 不持可变状态, 描述层与 runtime 态 (StepMachineState) 分离.
/// </summary>
public readonly record struct StepDefinition(int N, StepCommand[] Commands, StepCommand[]? OnEntry = null, StepCommand[]? OnExit = null);

/// <summary>
/// Phase 27A retry 2 S1 (2026-05-26): StepMachine 完整定义 — 描述层 SSOT.
/// 字段语义:
/// - Name: machine 标识 (用于 StepMachineState.CurrentStep / Locks 字典 key).
/// - InitialStep: 起始 step (一般 0).
/// - Steps: step 编号 → StepDefinition (ImmutableDictionary 防意外突变).
/// - LockMode: 并发模式 (默认 PerMachine — 单 machine 内串行执行, 跨 machine 并发).
/// - TickAlignment: tick 对齐 (默认 AsyncAlignedTo33ms — 与现 33ms × N tick 主循环节奏对齐, R8 mitigation).
/// - LocalVars: machine 局部变量 (e.g. 重试计数器), 通过 StepMachineState.Locals 持久化.
/// </summary>
public sealed record StepMachineDefinition(
    string Name,
    int InitialStep,
    ImmutableDictionary<int, StepDefinition> Steps,
    LockMode LockMode = LockMode.PerMachine,
    TickMode TickAlignment = TickMode.AsyncAlignedTo33ms,
    ImmutableDictionary<string, object>? LocalVars = null);

/// <summary>并发锁模式. PerMachine = 单 machine 内 SemaphoreSlim 锁 (默认, 防同 machine 二次进入); None = 无锁; ClassLevel = 跨 machine 同 class 锁 (27B+ 船长用).</summary>
public enum LockMode { None, PerMachine, ClassLevel }

/// <summary>Tick 对齐模式. SyncBurst = 同步连发 (无间隔); AsyncAlignedTo33ms = 异步对齐 33ms tick (默认, R8 主循环节奏匹配).</summary>
public enum TickMode { SyncBurst, AsyncAlignedTo33ms }
#endif
