using System;
using Dota2Simulator.GameAutomation.Domain.Actuation;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「全局按键监听」的需求。
/// 由 InputMonitor BC 的 adapter 实现（全局键盘钩子）。
/// </summary>
public interface IInputMonitor
{
    /// <summary>用户按键时触发。</summary>
    event Action<KeyTrigger> Triggered;

    void Start();

    void Stop();
}
