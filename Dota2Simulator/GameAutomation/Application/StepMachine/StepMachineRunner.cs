#if DOTA2
#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.StepMachine;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Application.StepMachine;

/// <summary>
/// Phase 27A retry 2 S1 (2026-05-26): StepMachine 解释器 — switch expression exhaustive (INV4: 0 CS8509).
/// TickMode 默认 AsyncAlignedTo33ms — 与现 33ms × N tick 主循环节奏对齐 (R8 mitigation).
/// ctor 5 参数 (state, hero, itemEngine, skillEngine, handle), S2-S5 严禁改签名 (Wave 3 attempt S4 改 ctor 破 S5 build 教训).
/// Press / PressAlt 通过 itemEngine.Input 间接调 IInputExecutor (避 6 参数; ItemEngine internal getter 复用注入端口).
/// UseItem 通过 itemEngine.根据图片使用物品 (已含 DeferredQueue + AckProbe 完整路径).
/// </summary>
public sealed class StepMachineRunner
{
    private const int _DefaultTickMs = 33;

    private readonly StepMachineState _state;
    private readonly HeroAggregate _hero;
    private readonly ItemEngine _itemEngine;
    private readonly SkillEngine _skillEngine;
    private readonly ImageHandle _handle;

    // S1 一次性 ctor, S2-S5 严禁改签名 (Wave 3 attempt S4 改 ctor 破 S5 build 教训)
    public StepMachineRunner(
        StepMachineState state,
        HeroAggregate hero,
        ItemEngine itemEngine,
        SkillEngine skillEngine,
        ImageHandle handle)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _hero = hero ?? throw new ArgumentNullException(nameof(hero));
        _itemEngine = itemEngine ?? throw new ArgumentNullException(nameof(itemEngine));
        _skillEngine = skillEngine ?? throw new ArgumentNullException(nameof(skillEngine));
        _handle = handle;
    }

    public TickMode TickAlignment { get; init; } = TickMode.AsyncAlignedTo33ms;

    /// <summary>外部入口 — 取 def.LockMode 决定锁形态后跑当前 step.</summary>
    public async Task ExecuteAsync(StepMachineDefinition def, CancellationToken ct = default)
    {
        if (def.LockMode == LockMode.PerMachine)
        {
            using IDisposable _ = await _state.AcquireLockAsync(def.Name).ConfigureAwait(false);
            await ExecuteCurrentStepAsync(def, ct).ConfigureAwait(false);
        }
        else
        {
            await ExecuteCurrentStepAsync(def, ct).ConfigureAwait(false);
        }
    }

    private async Task ExecuteCurrentStepAsync(StepMachineDefinition def, CancellationToken ct)
    {
        int currentStep = _state.GetStep(def.Name);
        if (!def.Steps.TryGetValue(currentStep, out StepDefinition stepDef))
        {
            return;
        }

        if (stepDef.OnEntry is not null)
        {
            foreach (StepCommand cmd in stepDef.OnEntry)
            {
                if (ct.IsCancellationRequested) return;
                _ = await InterpretAsync(cmd, def.Name).ConfigureAwait(false);
            }
        }

        foreach (StepCommand cmd in stepDef.Commands)
        {
            if (ct.IsCancellationRequested) return;
            bool shouldContinue = await InterpretAsync(cmd, def.Name).ConfigureAwait(false);
            if (!shouldContinue) break;
        }
    }

    /// <summary>switch expression exhaustive — INV4 (CS8509 0 warning). 19 原语全列 + 末尾 throw 兜底. 返 false = abort 后续 cmd.</summary>
    public async Task<bool> InterpretAsync(StepCommand cmd, string machineName)
        => cmd switch
        {
            Press p => await InterpretPress(p).ConfigureAwait(false),
            PressAlt pa => InterpretPressAlt(pa),
            WaitForColor w => await InterpretWaitForColor(w).ConfigureAwait(false),
            WaitForRefractoryExpire r => await InterpretWaitForRefractoryExpire(r).ConfigureAwait(false),
            WaitForProbeTrue w => await InterpretWaitForProbeTrue(w).ConfigureAwait(false),
            WaitForProbeAny w => await InterpretWaitForProbeAny(w, machineName).ConfigureAwait(false),
            SetStep s => InterpretSetStep(s, machineName),
            SetStepIf s => InterpretSetStepIf(s, machineName),
            Conditional c => await InterpretConditional(c, machineName).ConfigureAwait(false),
            LoopUntil l => await InterpretLoopUntil(l, machineName).ConfigureAwait(false),
            AbortIf a => InterpretAbortIf(a),
            ResetMachine _ => InterpretResetMachine(machineName),
            ParallelBatch p => await InterpretParallelBatch(p, machineName).ConfigureAwait(false),
            ActivateClause a => InterpretActivateClause(a),
            DeactivateClause d => InterpretDeactivateClause(d),
            DynamicDelay d => await InterpretDynamicDelay(d).ConfigureAwait(false),
            RegisterProbe r => InterpretRegisterProbe(r, machineName),
            UseItem u => InterpretUseItem(u),
            Delay d => await InterpretDelay(d).ConfigureAwait(false),
            AwaitSkillHelper a => await InterpretAwaitSkillHelper(a).ConfigureAwait(false),
            _ => throw new InvalidOperationException($"Unknown StepCommand: {cmd.GetType().Name}")
        };

    private async Task<bool> InterpretPress(Press p)
    {
        VirtualKey vk = VirtualKey.From(p.Key);
        if (p.HoldMs is int holdMs && holdMs > 0)
        {
            _itemEngine.Input.KeyDown(vk);
            await Task.Delay(holdMs).ConfigureAwait(false);
            _itemEngine.Input.KeyUp(vk);
        }
        else
        {
            _itemEngine.Input.Press(vk);
        }
        return true;
    }

    private bool InterpretPressAlt(PressAlt pa)
    {
        VirtualKey vk = VirtualKey.From(pa.Key);
        if (pa.Alt)
        {
            _itemEngine.Input.ComboAlt(vk);
        }
        else
        {
            _itemEngine.Input.Press(vk);
        }
        return true;
    }

    private async Task<bool> InterpretWaitForColor(WaitForColor w)
    {
        long start = Dota2Simulator.Games.Common.获取当前时间毫秒();
        while (Dota2Simulator.Games.Common.获取当前时间毫秒() - start < w.TimeoutMs)
        {
            if (w.Probe(_handle))
            {
                return true;
            }
            await Task.Delay(_DefaultTickMs).ConfigureAwait(false);
        }
        return false; // 超时 abort
    }

    private async Task<bool> InterpretWaitForRefractoryExpire(WaitForRefractoryExpire r)
    {
        while (_hero.Refractory.IsRefractory(r.Key))
        {
            await Task.Delay(8).ConfigureAwait(false);
        }
        return true;
    }

    private async Task<bool> InterpretWaitForProbeTrue(WaitForProbeTrue w)
    {
        while (!w.Probe(_handle))
        {
            await Task.Delay(w.IntervalMs).ConfigureAwait(false);
        }
        return true;
    }

    private async Task<bool> InterpretWaitForProbeAny(WaitForProbeAny w, string machineName)
    {
        while (true)
        {
            foreach (string name in w.ProbeNames)
            {
                string key = $"{machineName}:{name}";
                if (_state.Probes.TryGetValue(key, out Func<ImageHandle, bool>? probe) && probe(_handle))
                {
                    return true;
                }
            }
            await Task.Delay(_DefaultTickMs).ConfigureAwait(false);
        }
    }

    private bool InterpretSetStep(SetStep s, string machineName)
    {
        _state.SetStep(machineName, s.N);
        return true;
    }

    private bool InterpretSetStepIf(SetStepIf s, string machineName)
    {
        if (s.Cond(_handle))
        {
            _state.SetStep(machineName, s.N);
        }
        return true;
    }

    private async Task<bool> InterpretConditional(Conditional c, string machineName)
    {
        StepCommand[] branch = c.Cond(_handle) ? c.IfSteps : c.ElseSteps;
        foreach (StepCommand cmd in branch)
        {
            bool shouldContinue = await InterpretAsync(cmd, machineName).ConfigureAwait(false);
            if (!shouldContinue) return false;
        }
        return true;
    }

    private async Task<bool> InterpretLoopUntil(LoopUntil l, string machineName)
    {
        while (!l.Cond(_handle))
        {
            foreach (StepCommand cmd in l.Body)
            {
                bool shouldContinue = await InterpretAsync(cmd, machineName).ConfigureAwait(false);
                if (!shouldContinue) return false;
            }
        }
        return true;
    }

    private bool InterpretAbortIf(AbortIf a) => !a.Cond(_handle);

    private bool InterpretResetMachine(string machineName)
    {
        _state.SetStep(machineName, 0);
        return false; // 终止本次 Execute
    }

    private async Task<bool> InterpretParallelBatch(ParallelBatch p, string machineName)
    {
        Task<bool>[] tasks = p.Commands.Select(c => InterpretAsync(c, machineName)).ToArray();
        bool[] results = await Task.WhenAll(tasks).ConfigureAwait(false);
        return results.All(r => r);
    }

    private bool InterpretActivateClause(ActivateClause a)
    {
        if (Enum.TryParse(a.ClauseName, out ConditionSlotKey key))
        {
            _hero.Conditions[key].Active = true;
        }
        return true;
    }

    private bool InterpretDeactivateClause(DeactivateClause d)
    {
        if (Enum.TryParse(d.ClauseName, out ConditionSlotKey key))
        {
            _hero.Conditions[key].Active = false;
        }
        return true;
    }

    private async Task<bool> InterpretDynamicDelay(DynamicDelay d)
    {
        TimeSpan delay = d.Formula(_handle);
        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay).ConfigureAwait(false);
        }
        return true;
    }

    private bool InterpretRegisterProbe(RegisterProbe r, string machineName)
    {
        _state.Probes[$"{machineName}:{r.Name}"] = r.Probe;
        return true;
    }

    private bool InterpretUseItem(UseItem u)
    {
        _ = _itemEngine.根据图片使用物品(u.Template);
        return true;
    }

    private static async Task<bool> InterpretDelay(Delay d)
    {
        if (d.Ms > 0)
        {
            await Task.Delay(d.Ms).ConfigureAwait(false);
        }
        return true;
    }

    private static async Task<bool> InterpretAwaitSkillHelper(AwaitSkillHelper a)
    {
        if (a.TimeoutMs is int t && t > 0)
        {
            Task<bool> probeTask = a.Probe();
            Task winner = await Task.WhenAny(probeTask, Task.Delay(t)).ConfigureAwait(false);
            return winner == probeTask && await probeTask.ConfigureAwait(false);
        }
        return await a.Probe().ConfigureAwait(false);
    }
}
#endif
