#if DOTA2
#nullable enable
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Domain.StepMachine;

/// <summary>
/// Phase 27A retry 2 S1 (2026-05-26): StepMachine 运行态子聚合 — 第 7 子聚合 (同 Refractory/LegSwap/Stone/Conditions/Skills/Attack 模式).
/// 4 ConcurrentDictionary (跨线程访问):
/// - CurrentStep: machineName → step 编号 (runtime 步态).
/// - Probes: 命名 probe 注册表 (key = "{machineName}:{name}", 跨 step 共享; RegisterProbe 原语写入).
/// - Locals: machine 局部变量 (e.g. 重试计数).
/// - Locks: PerMachine 模式 SemaphoreSlim (27B+ 船长用; R3 已删但 API 保留).
/// Reset: 跨 hero 切换清残留 (Locks 保留 SemaphoreSlim 重用, 避免 Dispose 复杂度).
/// </summary>
public sealed class StepMachineState
{
    public ConcurrentDictionary<string, int> CurrentStep { get; } = new();
    public ConcurrentDictionary<string, Func<ImageHandle, bool>> Probes { get; } = new();
    public ConcurrentDictionary<string, object> Locals { get; } = new();
    public ConcurrentDictionary<string, SemaphoreSlim> Locks { get; } = new();  // PerMachine, 27B+ 船长用

    public int GetStep(string machineName) => CurrentStep.TryGetValue(machineName, out int s) ? s : 0;
    public void SetStep(string machineName, int n) => CurrentStep[machineName] = n;

    /// <summary>PerMachine 锁 — 取/创 SemaphoreSlim, await + return IDisposable Release 形态.</summary>
    public async Task<IDisposable> AcquireLockAsync(string machineName)
    {
        SemaphoreSlim sem = Locks.GetOrAdd(machineName, _ => new SemaphoreSlim(1, 1));
        await sem.WaitAsync().ConfigureAwait(false);
        return new LockReleaser(sem);
    }

    /// <summary>跨 hero 切换清残留 — CurrentStep/Probes/Locals 全清; Locks 保留 SemaphoreSlim 重用避免 Dispose 复杂度.</summary>
    public void Reset()
    {
        CurrentStep.Clear();
        Probes.Clear();
        Locals.Clear();
        // Locks 保留 (SemaphoreSlim 重用避免 Dispose 复杂度)
    }

    private sealed class LockReleaser : IDisposable
    {
        private SemaphoreSlim? _sem;
        public LockReleaser(SemaphoreSlim sem) { _sem = sem; }
        public void Dispose() { _sem?.Release(); _sem = null; }
    }
}
#endif
