#nullable enable
using System;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.KeyboardMouse;

namespace Dota2Simulator.InputMonitor.Adapters;

/// <summary>
/// 入站端口 <see cref="IInputMonitor"/> 的 adapter：
/// 把原始全局键盘钩子 <see cref="HookUserActivity"/> 的 .NET KeyDown 事件
/// 翻译成领域内 <see cref="KeyTrigger"/>，经 <see cref="Triggered"/> 转发。
///
/// 4.7 阶段只是建好就位——Form2 仍直连 HookUserActivity，本 adapter 的接入推迟。
/// </summary>
public sealed class HookUserActivityAdapter : IInputMonitor
{
    private readonly HookUserActivity _hook = new();

    public HookUserActivityAdapter()
    {
        // Form2 现状：全局作用域 + 仅安装键盘钩子（Start(false, true)）。
        // 此处保持一致，使 adapter 与现有钩子语义逐字节等价。
        _hook.HookScope = HookUserActivity.HookScopeType.GlobalScope;
        _hook.KeyDown += OnHookKeyDown;
    }

    /// <inheritdoc />
    public event Action<KeyTrigger>? Triggered;

    /// <inheritdoc />
    /// <remarks>仅安装键盘钩子，与 Form2 现状 <c>_hookUser.Start(false, true)</c> 一致。</remarks>
    public void Start() => _hook.Start(false, true);

    /// <inheritdoc />
    public void Stop() => _hook.Stop();

    private void OnHookKeyDown(object? sender, KeyEventArgs e)
        => Triggered?.Invoke(new KeyTrigger(VirtualKey.From(e.KeyCode)));
}
