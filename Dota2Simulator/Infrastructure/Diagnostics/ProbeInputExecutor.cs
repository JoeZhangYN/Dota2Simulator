using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Diagnostics;

/// <summary>
/// <see cref="IInputExecutor"/> 探针装饰器：每次键鼠输出调用录入 <see cref="RecordReplayProbe"/>
/// 后转发真实 adapter。探针关闭时仅多一次 bool 判断、不构造参数字符串。
/// </summary>
public sealed class ProbeInputExecutor : IInputExecutor
{
    private const string Port = "Input";
    private readonly IInputExecutor _inner;

    public ProbeInputExecutor(IInputExecutor inner) => _inner = inner;

    public void Press(VirtualKey key)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(Press), key.ToString());
        _inner.Press(key);
    }

    public void PressViaEnigo(VirtualKey key)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(PressViaEnigo), key.ToString());
        _inner.PressViaEnigo(key);
    }

    public void KeyDown(VirtualKey key)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(KeyDown), key.ToString());
        _inner.KeyDown(key);
    }

    public void KeyUp(VirtualKey key)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(KeyUp), key.ToString());
        _inner.KeyUp(key);
    }

    public IKeyHold Hold(VirtualKey key)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(Hold), key.ToString());
        return _inner.Hold(key);
    }

    public void ComboWhile(VirtualKey key, VirtualKey modifier)
    {
        if (RecordReplayProbe.Enabled)
            RecordReplayProbe.Record(Port, nameof(ComboWhile), $"{key}, {modifier}");
        _inner.ComboWhile(key, modifier);
    }

    public void ComboWhile(VirtualKey key, VirtualKey modifier1, VirtualKey modifier2)
    {
        if (RecordReplayProbe.Enabled)
            RecordReplayProbe.Record(Port, nameof(ComboWhile), $"{key}, {modifier1}, {modifier2}");
        _inner.ComboWhile(key, modifier1, modifier2);
    }

    public void ComboAlt(VirtualKey key)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(ComboAlt), key.ToString());
        _inner.ComboAlt(key);
    }

    public void MouseClick(MouseButton button)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(MouseClick), button.ToString());
        _inner.MouseClick(button);
    }

    public void MouseClickViaEnigo(MouseButton button)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(MouseClickViaEnigo), button.ToString());
        _inner.MouseClickViaEnigo(button);
    }

    public void MouseDown(MouseButton button)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(MouseDown), button.ToString());
        _inner.MouseDown(button);
    }

    public void MouseUp(MouseButton button)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(MouseUp), button.ToString());
        _inner.MouseUp(button);
    }

    public void MouseMoveTo(ScreenPoint point)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(MouseMoveTo), point.ToString());
        _inner.MouseMoveTo(point);
    }

    public void MouseMove(int x, int y, bool relative)
    {
        if (RecordReplayProbe.Enabled)
            RecordReplayProbe.Record(Port, nameof(MouseMove), $"{x}, {y}, relative={relative}");
        _inner.MouseMove(x, y, relative);
    }
}
