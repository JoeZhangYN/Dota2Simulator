using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Perception;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「键鼠输入执行」的需求。由 Input BC 的 adapter 实现。
/// Phase 2 端口贴合既有 SimKeyBoard 能力面，保证 adapter 纯搬运、零行为变更；
/// 方法归并优化留待 Phase 4 领域逻辑重写。
/// </summary>
public interface IInputExecutor
{
    /// <summary>按下并释放一个键。</summary>
    void Press(VirtualKey key);

    /// <summary>
    /// 经 SimEnigo 后端按下并释放一个键（simengio.dll）。
    /// 仅供需要 Enigo 后端行为的策略类使用（如进化岛），不要与 <see cref="Press"/> 混用——两者驱动栈不同。
    /// </summary>
    void PressViaEnigo(VirtualKey key);

    /// <summary>按下一个键（不释放）。</summary>
    void KeyDown(VirtualKey key);

    /// <summary>释放一个键。</summary>
    void KeyUp(VirtualKey key);

    /// <summary>按住一个键，返回的 IKeyHold 在 Dispose 时自动释放——编译期保证配对。</summary>
    IKeyHold Hold(VirtualKey key);

    /// <summary>按住 modifier 期间按一次 key。</summary>
    void ComboWhile(VirtualKey key, VirtualKey modifier);

    /// <summary>按住两个 modifier 期间按一次 key。</summary>
    void ComboWhile(VirtualKey key, VirtualKey modifier1, VirtualKey modifier2);

    /// <summary>按住 Alt 期间按一次 key。</summary>
    void ComboAlt(VirtualKey key);

    /// <summary>点击鼠标按键。</summary>
    void MouseClick(MouseButton button);

    /// <summary>
    /// 经 SimEnigo 后端点击鼠标按键（simengio.dll）。
    /// 仅供需要 Enigo 后端行为的业务使用（如绝地潜兵 2 战备菜单确认），不要与 <see cref="MouseClick"/> 混用——两者驱动栈不同。
    /// </summary>
    void MouseClickViaEnigo(MouseButton button);

    /// <summary>按下鼠标按键（不释放）。</summary>
    void MouseDown(MouseButton button);

    /// <summary>释放鼠标按键。</summary>
    void MouseUp(MouseButton button);

    /// <summary>鼠标瞬移到指定屏幕坐标。</summary>
    void MouseMoveTo(ScreenPoint point);

    /// <summary>鼠标移动：relative=true 为相对位移，false 为绝对坐标。</summary>
    void MouseMove(int x, int y, bool relative);
}
