using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.KeyboardMouse;
using NLog;

namespace Dota2Simulator.Input.Adapters;

/// <summary>
/// IInputExecutor 的混合后端实现：复刻原 SimKeyBoard 的固定分发——
/// 点击 / 单键 / 瞬移走 Interception 驱动；组合键 / 相对移动走 Enigo。
/// 两后端 FFI 面已对齐 input_abi 契约 42 符号（任何分发规则改动只需换绑定类名，
/// 不再受单端能力缺口约束）；当前分发策略保持历史配置不变。
/// 后端非 0 返回码记 NLog warn（IInputExecutor 保持 void，不向业务层抛异常）。
/// </summary>
public sealed class HybridInputAdapter : IInputExecutor
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>interception_input 失败路径：rc &gt; 0（InputError）。</summary>
    private static void CheckDriver(int rc, string op)
    {
        if (rc != 0)
        {
            Logger.Warn("interception_input {Op} 失败 rc={Rc}: {Msg}", op, rc, InterceptionInput.LastError());
        }
    }

    /// <summary>simengio 失败路径：rc &lt; 0（SimError）。</summary>
    private static void CheckEnigo(int rc, string op)
    {
        if (rc != 0)
        {
            Logger.Warn("simengio {Op} 失败 rc={Rc}: {Msg}", op, rc, SimEnigo.LastError());
        }
    }

    public void Press(VirtualKey key) => CheckDriver(InterceptionInput.KeyPress(key.ToNative()), nameof(Press));

    public void PressViaEnigo(VirtualKey key) => CheckEnigo(SimEnigo.KeyPress(key.ToNative()), nameof(PressViaEnigo));

    public void KeyDown(VirtualKey key) => CheckDriver(InterceptionInput.KeyDown(key.ToNative()), nameof(KeyDown));

    public void KeyUp(VirtualKey key) => CheckDriver(InterceptionInput.KeyUp(key.ToNative()), nameof(KeyUp));

    public IKeyHold Hold(VirtualKey key)
    {
        CheckDriver(InterceptionInput.KeyDown(key.ToNative()), nameof(Hold));
        return new KeyHoldHandle(this, key);
    }

    public void ComboWhile(VirtualKey key, VirtualKey modifier)
        => CheckEnigo(SimEnigo.KeyPressWhile(key.ToNative(), modifier.ToNative()), nameof(ComboWhile));

    public void ComboWhile(VirtualKey key, VirtualKey modifier1, VirtualKey modifier2)
        => CheckEnigo(
            SimEnigo.KeyPressWhileTwo(key.ToNative(), modifier1.ToNative(), modifier2.ToNative()),
            nameof(ComboWhile));

    public void ComboAlt(VirtualKey key)
        => CheckEnigo(SimEnigo.KeyPressAlt(key.ToNative()), nameof(ComboAlt));

    public void MouseClick(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: CheckDriver(InterceptionInput.MouseLeftClick(), nameof(MouseClick)); break;
            case MouseButton.Right: CheckDriver(InterceptionInput.MouseRightClick(), nameof(MouseClick)); break;
            case MouseButton.Middle: CheckDriver(InterceptionInput.MouseMiddleClick(), nameof(MouseClick)); break;
        }
    }

    public void MouseClickViaEnigo(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: CheckEnigo(SimEnigo.MouseLeftClick(), nameof(MouseClickViaEnigo)); break;
            case MouseButton.Right: CheckEnigo(SimEnigo.MouseRightClick(), nameof(MouseClickViaEnigo)); break;
            case MouseButton.Middle: CheckEnigo(SimEnigo.MouseMiddleClick(), nameof(MouseClickViaEnigo)); break;
            default:
                throw new System.NotSupportedException($"未知 MouseButton: {button}");
        }
    }

    public void MouseDown(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: CheckDriver(InterceptionInput.MouseLeftDown(), nameof(MouseDown)); break;
            case MouseButton.Right: CheckDriver(InterceptionInput.MouseRightDown(), nameof(MouseDown)); break;
            case MouseButton.Middle: CheckDriver(InterceptionInput.MouseMiddleDown(), nameof(MouseDown)); break;
        }
    }

    public void MouseUp(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: CheckDriver(InterceptionInput.MouseLeftUp(), nameof(MouseUp)); break;
            case MouseButton.Right: CheckDriver(InterceptionInput.MouseRightUp(), nameof(MouseUp)); break;
            case MouseButton.Middle: CheckDriver(InterceptionInput.MouseMiddleUp(), nameof(MouseUp)); break;
        }
    }

    public void MouseMoveTo(ScreenPoint point)
        => CheckDriver(InterceptionInput.MouseMoveTo(point.X, point.Y, 3840, 2160), nameof(MouseMoveTo));

    public void MouseMove(int x, int y, bool relative)
    {
        if (relative)
        {
            CheckEnigo(SimEnigo.MouseMoveRelative(x, y), nameof(MouseMove));
        }
        else
        {
            // 旧 simengio MouseMove(x,y,0) 即 enigo 像素 Abs；契约面对应 MouseMoveTo（dims 仅校验）。
            CheckEnigo(SimEnigo.MouseMoveTo(x, y, 3840, 2160), nameof(MouseMove));
        }
    }
}
