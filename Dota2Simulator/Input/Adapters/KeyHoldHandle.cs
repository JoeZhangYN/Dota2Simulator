using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Input.Adapters;

/// <summary>IKeyHold 实现：Dispose 时经 IInputExecutor 释放按键，幂等。</summary>
internal sealed class KeyHoldHandle : IKeyHold
{
    private readonly IInputExecutor _executor;
    private bool _released;

    public KeyHoldHandle(IInputExecutor executor, VirtualKey key)
    {
        _executor = executor;
        Key = key;
    }

    public VirtualKey Key { get; }

    public void Dispose()
    {
        if (_released)
            return;
        _released = true;
        _executor.KeyUp(Key);
    }
}
