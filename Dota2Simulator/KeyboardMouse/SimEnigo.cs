using System.Runtime.InteropServices;

namespace Dota2Simulator.KeyboardMouse;

public class SimEnigo
{
    [DllImport("simengio.dll")]
    public static extern void KeyDown(uint s);

    [DllImport("simengio.dll")]
    public static extern void KeyUp(uint s);

    [DllImport("simengio.dll")]
    public static extern void KeyPress(uint s);

    [DllImport("simengio.dll")]
    public static extern void KeyPressWhile(uint s,uint d);

    [DllImport("simengio.dll")]
    public static extern void KeyPressAlt(uint s);

    [DllImport("simengio.dll")]
    public static extern void MouseMove(int x, int y);

    [DllImport("simengio.dll")]
    public static extern void MouseMoveRelative(int x, int y);

    [DllImport("simengio.dll")]
    public static extern void LeftClick();

    [DllImport("simengio.dll")]
    public static extern void Rightlick();
}