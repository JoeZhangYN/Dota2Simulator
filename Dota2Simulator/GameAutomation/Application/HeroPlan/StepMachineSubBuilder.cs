// Phase 27A retry 2 S2 (2026-05-26): StepMachine 子构造器 — 8 维 fluent API.
// **显式破坏** Phase 26 之前的"单 fluent type 不变量". 详 .claude/CLAUDE.md "Phase 27A retry 2 显式破坏点" 段 (INV5).
// 业务侧通过 .StepMachine(name, sub => sub.Initial(0).Step(0).Do(...).End()) 闭包配置, 返主 HeroPlanBuilder.
#if DOTA2
#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dota2Simulator.GameAutomation.Domain.StepMachine;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Application.HeroPlans;

/// <summary>
/// Phase 27A retry 2 S2 (2026-05-26): StepMachine 子构造器 — 8 维 fluent API.
/// 维度: Initial / Step / OnEntry / OnExit / Local / Probe / WithLockMode / WithTickAlignment.
/// **打破** Phase 26 之前的"单 fluent type 不变量". 详 .claude/CLAUDE.md "Phase 27A retry 2 显式破坏点" 段 (INV5).
/// 默认: LockMode=PerMachine, TickAlignment=AsyncAlignedTo33ms.
/// </summary>
public sealed class StepMachineSubBuilder
{
    private readonly string _name;
    private int _initialStep;
    private readonly Dictionary<int, StepDefinition> _steps = new();
    private readonly Dictionary<string, object> _locals = new(StringComparer.Ordinal);
    private readonly List<StepCommand> _onEntry = new();
    private readonly List<StepCommand> _onExit = new();
    private LockMode _lockMode = LockMode.PerMachine;
    private TickMode _tickMode = TickMode.AsyncAlignedTo33ms;

    internal StepMachineSubBuilder(string name)
    {
        _name = name;
    }

    /// <summary>维 1: 设 InitialStep (默认 0).</summary>
    public StepMachineSubBuilder Initial(int n)
    {
        _initialStep = n;
        return this;
    }

    /// <summary>维 2: 进入 Step n 定义子上下文 — 必须以 .End() 结束返主 sub-builder.</summary>
    public StepDefinitionBuilder Step(int n) => new(this, n);

    /// <summary>
    /// 维 3: 全局 OnEntry — 累加到 InitialStep 的 OnEntry 前缀 (Build() 时注入).
    /// 多次调用累加.
    /// </summary>
    public StepMachineSubBuilder OnEntry(params StepCommand[] cmds)
    {
        if (cmds is not null && cmds.Length > 0) _onEntry.AddRange(cmds);
        return this;
    }

    /// <summary>维 4: 全局 OnExit — 累加到 InitialStep 的 OnExit 后缀 (Build() 时注入). 多次调用累加.</summary>
    public StepMachineSubBuilder OnExit(params StepCommand[] cmds)
    {
        if (cmds is not null && cmds.Length > 0) _onExit.AddRange(cmds);
        return this;
    }

    /// <summary>维 5: 设 machine 局部变量初值 (持久化到 StepMachineState.Locals).</summary>
    public StepMachineSubBuilder Local(string key, object initValue)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Local key 不能为空", nameof(key));
        _locals[key] = initValue;
        return this;
    }

    /// <summary>
    /// 维 6: 注册命名 probe — 转 RegisterProbe StepCommand 注入 InitialStep OnEntry 前缀.
    /// Runner 会写入 StepMachineState.Probes["{machineName}:{name}"].
    /// </summary>
    public StepMachineSubBuilder Probe(string name, Func<ImageHandle, bool> probe)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Probe name 不能为空", nameof(name));
        if (probe is null) throw new ArgumentNullException(nameof(probe));
        _onEntry.Add(new RegisterProbe(name, probe));
        return this;
    }

    /// <summary>维 7: 设 LockMode (默认 PerMachine).</summary>
    public StepMachineSubBuilder WithLockMode(LockMode mode)
    {
        _lockMode = mode;
        return this;
    }

    /// <summary>维 8: 设 TickAlignment (默认 AsyncAlignedTo33ms).</summary>
    public StepMachineSubBuilder WithTickAlignment(TickMode mode)
    {
        _tickMode = mode;
        return this;
    }

    /// <summary>StepDefinitionBuilder.End() 内部调 — 写入 _steps 字典.</summary>
    internal void AddStep(int n, StepCommand[] commands, StepCommand[]? onEntry, StepCommand[]? onExit)
    {
        _steps[n] = new StepDefinition(n, commands, onEntry, onExit);
    }

    /// <summary>HeroPlanBuilder.StepMachine() 终结时调 — 合并全局 OnEntry/OnExit 到 InitialStep, 返不可变 definition.</summary>
    internal StepMachineDefinition Build()
    {
        // 全局 OnEntry/OnExit 累加注入 InitialStep (描述层概念, 与 step-level OnEntry/OnExit 拼接).
        if (_onEntry.Count > 0 || _onExit.Count > 0)
        {
            _steps.TryGetValue(_initialStep, out StepDefinition initStep);
            StepCommand[] commands = initStep.Commands ?? Array.Empty<StepCommand>();
            StepCommand[] entry = (initStep.OnEntry ?? Array.Empty<StepCommand>()).Concat(_onEntry).ToArray();
            StepCommand[] exit = (initStep.OnExit ?? Array.Empty<StepCommand>()).Concat(_onExit).ToArray();
            _steps[_initialStep] = new StepDefinition(
                _initialStep,
                commands,
                entry.Length > 0 ? entry : null,
                exit.Length > 0 ? exit : null);
        }

        return new StepMachineDefinition(
            Name: _name,
            InitialStep: _initialStep,
            Steps: _steps.ToImmutableDictionary(),
            LockMode: _lockMode,
            TickAlignment: _tickMode,
            LocalVars: _locals.Count > 0 ? _locals.ToImmutableDictionary(StringComparer.Ordinal) : null);
    }
}

/// <summary>
/// .Step(n) 返回的子上下文 — 配置完成调 .End() 返 StepMachineSubBuilder.
/// 3 维: Do (主 Commands) / OnEntry / OnExit.
/// </summary>
public sealed class StepDefinitionBuilder
{
    private readonly StepMachineSubBuilder _parent;
    private readonly int _n;
    private StepCommand[]? _commands;
    private StepCommand[]? _onEntry;
    private StepCommand[]? _onExit;

    internal StepDefinitionBuilder(StepMachineSubBuilder parent, int n)
    {
        _parent = parent;
        _n = n;
    }

    /// <summary>主 Commands — step 主体执行序列 (Runner 按序解释).</summary>
    public StepDefinitionBuilder Do(params StepCommand[] cmds)
    {
        _commands = cmds;
        return this;
    }

    /// <summary>step-level OnEntry — 进入本 step 时跑 (本 step Commands 之前).</summary>
    public StepDefinitionBuilder OnEntry(params StepCommand[] cmds)
    {
        _onEntry = cmds;
        return this;
    }

    /// <summary>step-level OnExit — 离开本 step 时跑 (当前 Runner 暂未消费, S1 原语集预留).</summary>
    public StepDefinitionBuilder OnExit(params StepCommand[] cmds)
    {
        _onExit = cmds;
        return this;
    }

    /// <summary>结束 step 子上下文, 返 StepMachineSubBuilder 继续链式配置.</summary>
    public StepMachineSubBuilder End()
    {
        _parent.AddStep(_n, _commands ?? Array.Empty<StepCommand>(), _onEntry, _onExit);
        return _parent;
    }
}

#endif
