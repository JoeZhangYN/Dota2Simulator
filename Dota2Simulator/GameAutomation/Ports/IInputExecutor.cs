using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Perception;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「键鼠输入执行」的需求。
/// 由 Input BC 的 adapter 实现（Interception / Enigo）。
/// </summary>
public interface IInputExecutor
{
    /// <summary>按下并释放一个键。</summary>
    void Press(VirtualKey key);

    /// <summary>按住一个键，返回的 IKeyHold 在 Dispose 时自动释放——编译期保证配对。</summary>
    IKeyHold Hold(VirtualKey key);

    /// <summary>组合键：按住 modifier 时按下 key。</summary>
    void Combo(VirtualKey key, VirtualKey modifier);

    /// <summary>点击鼠标按键。</summary>
    void MouseClick(MouseButton button);

    /// <summary>移动鼠标到指定屏幕坐标。</summary>
    void MouseMoveTo(ScreenPoint point);
}
