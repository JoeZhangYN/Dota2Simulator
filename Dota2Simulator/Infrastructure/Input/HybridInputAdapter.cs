using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.KeyboardMouse;
using WinFormsKeys = System.Windows.Forms.Keys;

namespace Dota2Simulator.Input.Adapters;

/// <summary>
/// IInputExecutor 的混合后端实现：复刻原 SimKeyBoard 的固定分发——
/// 点击 / 单键 / 瞬移走 Interception 驱动；组合键 / 相对移动走 Enigo。
/// 这不是「可替换 backend」——当前只有这一种固定分发配置。
/// </summary>
public sealed class HybridInputAdapter : IInputExecutor
{
    public void Press(VirtualKey key) => InterceptionInput.KeyPress(key.ToNative());

    public void PressViaEnigo(VirtualKey key) => SimEnigo.KeyPress((WinFormsKeys)key.ToNative());

    public void KeyDown(VirtualKey key) => InterceptionInput.KeyDown(key.ToNative());

    public void KeyUp(VirtualKey key) => InterceptionInput.KeyUp(key.ToNative());

    public IKeyHold Hold(VirtualKey key)
    {
        InterceptionInput.KeyDown(key.ToNative());
        return new KeyHoldHandle(this, key);
    }

    public void ComboWhile(VirtualKey key, VirtualKey modifier)
        => SimEnigo.KeyPressWhile((WinFormsKeys)key.ToNative(), (WinFormsKeys)modifier.ToNative());

    public void ComboWhile(VirtualKey key, VirtualKey modifier1, VirtualKey modifier2)
        => SimEnigo.KeyPressWhileTwo(
            (WinFormsKeys)key.ToNative(),
            (WinFormsKeys)modifier1.ToNative(),
            (WinFormsKeys)modifier2.ToNative());

    public void ComboAlt(VirtualKey key)
        => SimEnigo.KeyPressAlt((WinFormsKeys)key.ToNative());

    public void MouseClick(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: InterceptionInput.MouseLeftClick(); break;
            case MouseButton.Right: InterceptionInput.MouseRightClick(); break;
            case MouseButton.Middle: InterceptionInput.MouseMiddleClick(); break;
        }
    }

    public void MouseClickViaEnigo(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: SimEnigo.MouseLeftClick(); break;
            case MouseButton.Right: SimEnigo.MouseRightClick(); break;
            case MouseButton.Middle:
                throw new System.NotSupportedException("SimEnigo 后端未导出 MouseMiddleClick");
            default:
                throw new System.NotSupportedException($"未知 MouseButton: {button}");
        }
    }

    public void MouseDown(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: InterceptionInput.MouseLeftDown(); break;
            case MouseButton.Right: InterceptionInput.MouseRightDown(); break;
            case MouseButton.Middle: InterceptionInput.MouseMiddleDown(); break;
        }
    }

    public void MouseUp(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: InterceptionInput.MouseLeftUp(); break;
            case MouseButton.Right: InterceptionInput.MouseRightUp(); break;
            case MouseButton.Middle: InterceptionInput.MouseMiddleUp(); break;
        }
    }

    public void MouseMoveTo(ScreenPoint point)
        => InterceptionInput.MouseMoveTo(point.X, point.Y, 3840, 2160);

    public void MouseMove(int x, int y, bool relative)
        => SimEnigo.MouseMove(x, y, relative ? 1 : 0);
}
