using System;
using Dota2Simulator.GameAutomation.Domain.Actuation;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 按键按住守卫。Dispose 时释放按键——配合 using 语句，编译期保证 KeyDown/KeyUp 配对。
/// </summary>
public interface IKeyHold : IDisposable
{
    VirtualKey Key { get; }
}
