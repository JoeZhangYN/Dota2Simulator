using Dota2Simulator.KeyboardMouse;

namespace Dota2Simulator.Infrastructure.Input;

/// <summary>
/// 输入后端（simengio / interception_input）的公共面，令 <see cref="HybridInputAdapter"/>
/// 持后端引用而非硬编码静态类名（可多态 / 可注入 mock 测 / 未来可插第 3 后端）。
/// 返回值约定同各绑定：0 = 成功，非 0 = 错误码（诊断走 <see cref="LastError"/>）。
/// </summary>
/// <remarks>
/// 仅覆盖 <see cref="HybridInputAdapter"/> 实际用到的成员（非全 42 面）——
/// 接口随消费需要增长，避免空声明样板。
/// </remarks>
internal interface IInputBackend
{
    int KeyPress(ushort code);
    int KeyDown(ushort code);
    int KeyUp(ushort code);
    int KeyPressWhile(ushort key, ushort modifier);
    int KeyPressWhileTwo(ushort key, ushort mod1, ushort mod2);
    int KeyPressAlt(ushort key);

    int MouseLeftClick();
    int MouseRightClick();
    int MouseMiddleClick();
    int MouseLeftDown();
    int MouseRightDown();
    int MouseMiddleDown();
    int MouseLeftUp();
    int MouseRightUp();
    int MouseMiddleUp();

    int MouseMoveTo(int x, int y, int screenWidth, int screenHeight);
    int MouseMoveRelative(int dx, int dy);

    string LastError();
}

/// <summary>interception_input.dll 后端（驱动级）。委托到静态绑定 <see cref="InterceptionInput"/>。</summary>
internal sealed class DriverBackend : IInputBackend
{
    public int KeyPress(ushort code) => InterceptionInput.KeyPress(code);
    public int KeyDown(ushort code) => InterceptionInput.KeyDown(code);
    public int KeyUp(ushort code) => InterceptionInput.KeyUp(code);
    public int KeyPressWhile(ushort key, ushort modifier) => InterceptionInput.KeyPressWhile(key, modifier);
    public int KeyPressWhileTwo(ushort key, ushort mod1, ushort mod2) => InterceptionInput.KeyPressWhileTwo(key, mod1, mod2);
    public int KeyPressAlt(ushort key) => InterceptionInput.KeyPressAlt(key);

    public int MouseLeftClick() => InterceptionInput.MouseLeftClick();
    public int MouseRightClick() => InterceptionInput.MouseRightClick();
    public int MouseMiddleClick() => InterceptionInput.MouseMiddleClick();
    public int MouseLeftDown() => InterceptionInput.MouseLeftDown();
    public int MouseRightDown() => InterceptionInput.MouseRightDown();
    public int MouseMiddleDown() => InterceptionInput.MouseMiddleDown();
    public int MouseLeftUp() => InterceptionInput.MouseLeftUp();
    public int MouseRightUp() => InterceptionInput.MouseRightUp();
    public int MouseMiddleUp() => InterceptionInput.MouseMiddleUp();

    public int MouseMoveTo(int x, int y, int screenWidth, int screenHeight) => InterceptionInput.MouseMoveTo(x, y, screenWidth, screenHeight);
    public int MouseMoveRelative(int dx, int dy) => InterceptionInput.MouseMoveRelative(dx, dy);

    public string LastError() => InterceptionInput.LastError();
}

/// <summary>simengio.dll 后端（enigo 用户态）。委托到静态绑定 <see cref="SimEnigo"/>。</summary>
internal sealed class EnigoBackend : IInputBackend
{
    public int KeyPress(ushort code) => SimEnigo.KeyPress(code);
    public int KeyDown(ushort code) => SimEnigo.KeyDown(code);
    public int KeyUp(ushort code) => SimEnigo.KeyUp(code);
    public int KeyPressWhile(ushort key, ushort modifier) => SimEnigo.KeyPressWhile(key, modifier);
    public int KeyPressWhileTwo(ushort key, ushort mod1, ushort mod2) => SimEnigo.KeyPressWhileTwo(key, mod1, mod2);
    public int KeyPressAlt(ushort key) => SimEnigo.KeyPressAlt(key);

    public int MouseLeftClick() => SimEnigo.MouseLeftClick();
    public int MouseRightClick() => SimEnigo.MouseRightClick();
    public int MouseMiddleClick() => SimEnigo.MouseMiddleClick();
    public int MouseLeftDown() => SimEnigo.MouseLeftDown();
    public int MouseRightDown() => SimEnigo.MouseRightDown();
    public int MouseMiddleDown() => SimEnigo.MouseMiddleDown();
    public int MouseLeftUp() => SimEnigo.MouseLeftUp();
    public int MouseRightUp() => SimEnigo.MouseRightUp();
    public int MouseMiddleUp() => SimEnigo.MouseMiddleUp();

    public int MouseMoveTo(int x, int y, int screenWidth, int screenHeight) => SimEnigo.MouseMoveTo(x, y, screenWidth, screenHeight);
    public int MouseMoveRelative(int dx, int dy) => SimEnigo.MouseMoveRelative(dx, dy);

    public string LastError() => SimEnigo.LastError();
}
