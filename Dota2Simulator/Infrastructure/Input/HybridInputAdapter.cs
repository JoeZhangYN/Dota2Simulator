using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.KeyboardMouse;
using NLog;

namespace Dota2Simulator.Input.Adapters;

/// <summary>
/// IInputExecutor 的混合后端实现：复刻原 SimKeyBoard 的固定分发——
/// 点击 / 单键 / 瞬移走 Interception 驱动；组合键 / 相对移动走 Enigo。
/// 两后端经 <see cref="IInputBackend"/> 注入（默认 <see cref="DriverBackend"/> /
/// <see cref="EnigoBackend"/>），不再硬编码静态类名——可注入 mock 测、未来可换后端。
/// 后端非 0 返回码记 NLog warn（IInputExecutor 保持 void，不向业务层抛异常）。
/// </summary>
public sealed class HybridInputAdapter : IInputExecutor
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly Infrastructure.Input.IInputBackend _driver;
    private readonly Infrastructure.Input.IInputBackend _enigo;

    /// <summary>生产构造：用默认后端绑定（interception 驱动 + enigo）。</summary>
    public HybridInputAdapter()
        : this(new Infrastructure.Input.DriverBackend(), new Infrastructure.Input.EnigoBackend())
    {
    }

    /// <summary>注入构造（同程序集 / InternalsVisibleTo 测试用）：可换 mock 后端。</summary>
    internal HybridInputAdapter(
        Infrastructure.Input.IInputBackend driver,
        Infrastructure.Input.IInputBackend enigo)
    {
        _driver = driver;
        _enigo = enigo;
    }

    /// <summary>interception_input 失败路径：rc &gt; 0（InputError）。</summary>
    private void CheckDriver(int rc, string op)
    {
        if (rc != 0)
        {
            Logger.Warn("interception_input {Op} 失败 rc={Rc}: {Msg}", op, rc, _driver.LastError());
        }
    }

    /// <summary>simengio 失败路径：rc &lt; 0（SimError）。</summary>
    private void CheckEnigo(int rc, string op)
    {
        if (rc != 0)
        {
            Logger.Warn("simengio {Op} 失败 rc={Rc}: {Msg}", op, rc, _enigo.LastError());
        }
    }

    public void Press(VirtualKey key) => CheckDriver(_driver.KeyPress(key.ToNative()), nameof(Press));

    public void PressViaEnigo(VirtualKey key) => CheckEnigo(_enigo.KeyPress(key.ToNative()), nameof(PressViaEnigo));

    public void KeyDown(VirtualKey key) => CheckDriver(_driver.KeyDown(key.ToNative()), nameof(KeyDown));

    public void KeyUp(VirtualKey key) => CheckDriver(_driver.KeyUp(key.ToNative()), nameof(KeyUp));

    public IKeyHold Hold(VirtualKey key)
    {
        CheckDriver(_driver.KeyDown(key.ToNative()), nameof(Hold));
        return new KeyHoldHandle(this, key);
    }

    public void ComboWhile(VirtualKey key, VirtualKey modifier)
        => CheckEnigo(_enigo.KeyPressWhile(key.ToNative(), modifier.ToNative()), nameof(ComboWhile));

    public void ComboWhile(VirtualKey key, VirtualKey modifier1, VirtualKey modifier2)
        => CheckEnigo(
            _enigo.KeyPressWhileTwo(key.ToNative(), modifier1.ToNative(), modifier2.ToNative()),
            nameof(ComboWhile));

    public void ComboAlt(VirtualKey key)
        => CheckEnigo(_enigo.KeyPressAlt(key.ToNative()), nameof(ComboAlt));

    public void MouseClick(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: CheckDriver(_driver.MouseLeftClick(), nameof(MouseClick)); break;
            case MouseButton.Right: CheckDriver(_driver.MouseRightClick(), nameof(MouseClick)); break;
            case MouseButton.Middle: CheckDriver(_driver.MouseMiddleClick(), nameof(MouseClick)); break;
        }
    }

    public void MouseClickViaEnigo(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: CheckEnigo(_enigo.MouseLeftClick(), nameof(MouseClickViaEnigo)); break;
            case MouseButton.Right: CheckEnigo(_enigo.MouseRightClick(), nameof(MouseClickViaEnigo)); break;
            case MouseButton.Middle: CheckEnigo(_enigo.MouseMiddleClick(), nameof(MouseClickViaEnigo)); break;
            default:
                throw new System.NotSupportedException($"未知 MouseButton: {button}");
        }
    }

    public void MouseDown(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: CheckDriver(_driver.MouseLeftDown(), nameof(MouseDown)); break;
            case MouseButton.Right: CheckDriver(_driver.MouseRightDown(), nameof(MouseDown)); break;
            case MouseButton.Middle: CheckDriver(_driver.MouseMiddleDown(), nameof(MouseDown)); break;
        }
    }

    public void MouseUp(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left: CheckDriver(_driver.MouseLeftUp(), nameof(MouseUp)); break;
            case MouseButton.Right: CheckDriver(_driver.MouseRightUp(), nameof(MouseUp)); break;
            case MouseButton.Middle: CheckDriver(_driver.MouseMiddleUp(), nameof(MouseUp)); break;
        }
    }

    public void MouseMoveTo(ScreenPoint point)
        => CheckDriver(_driver.MouseMoveTo(point.X, point.Y, 3840, 2160), nameof(MouseMoveTo));

    public void MouseMove(int x, int y, bool relative)
    {
        if (relative)
        {
            CheckEnigo(_enigo.MouseMoveRelative(x, y), nameof(MouseMove));
        }
        else
        {
            // 旧 simengio MouseMove(x,y,0) 即 enigo 像素 Abs；契约面对应 MouseMoveTo（dims 仅校验）。
            CheckEnigo(_enigo.MouseMoveTo(x, y, 3840, 2160), nameof(MouseMove));
        }
    }
}
