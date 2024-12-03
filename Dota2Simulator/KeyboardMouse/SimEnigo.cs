using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dota2Simulator.KeyboardMouse
{
    internal class SimEnigo
    {
        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeyDown(Keys s);

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeyUp(Keys s);

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeyPress(Keys s);

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeyPressWhile(Keys s, Keys d);

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeyPressWhileTwo(Keys s, Keys d, Keys a);

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeyPressAlt(Keys s);

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MouseMove(int x, int y, int coord_type);

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MouseLeftClick();

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MouseLeftDown();

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MouseLeftUp();

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MouseRightClick();

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MouseRightDown();

        [DllImport("simengio.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MouseRightUp();
    }

    internal enum KeysE : uint
    {
        Num0 = 0x30,
        Num1 = 0x31,
        Num2 = 0x32,
        Num3 = 0x33,
        Num4 = 0x34,
        Num5 = 0x35,
        Num6 = 0x36,
        Num7 = 0x37,
        Num8 = 0x38,
        Num9 = 0x39,
        A = 0x41,
        B = 0x42,
        C = 0x43,
        D = 0x44,
        E = 0x45,
        F = 0x46,
        G = 0x47,
        H = 0x48,
        I = 0x49,
        J = 0x4A,
        K = 0x4B,
        L = 0x4C,
        M = 0x4D,
        N = 0x4E,
        O = 0x4F,
        P = 0x50,
        Q = 0x51,
        R = 0x52,
        S = 0x53,
        T = 0x54,
        U = 0x55,
        V = 0x56,
        W = 0x57,
        X = 0x58,
        Y = 0x59,
        Z = 0x5A,
        F1 = 0x70,
        F2 = 0x71,
        F3 = 0x72,
        F4 = 0x73,
        F5 = 0x74,
        F6 = 0x75,
        F7 = 0x76,
        F8 = 0x77,
        F9 = 0x78,
        F10 = 0x79,
        F11 = 0x7A,
        F12 = 0x7B,
        Numpad0 = 0x60,
        Numpad1 = 0x61,
        Numpad2 = 0x62,
        Numpad3 = 0x63,
        Numpad4 = 0x64,
        Numpad5 = 0x65,
        Numpad6 = 0x66,
        Numpad7 = 0x67,
        Numpad8 = 0x68,
        Numpad9 = 0x69,
        Add = 0x6B,
        Decimal = 0x6E,
        Divide = 0x6F,
        Multiply = 0x6A,
        Subtract = 0x6D,
        Backspace = 0x08,
        Tab = 0x09,
        Clear = 0x0C,
        Enter = 0x0D,
        Shift = 0x10,
        Control = 0x11,
        Alt = 0x12,
        Pause = 0x13,
        CapsLock = 0x14,
        Escape = 0x1B,
        Space = 0x20,
        PageUp = 0x21,
        PageDown = 0x22,
        End = 0x23,
        Home = 0x24,
        LeftArrow = 0x25,
        UpArrow = 0x26,
        RightArrow = 0x27,
        DownArrow = 0x28,
        Insert = 0x2D,
        Delete = 0x2E,
        Help = 0x2F,
        LWin = 0x5B,
        RWin = 0x5C,
        Apps = 0x5D,
        Sleep = 0x5F,
        NumLock = 0x90,
        ScrollLock = 0x91,
        BrowserBack = 0xA6,
        BrowserForward = 0xA7,
        BrowserRefresh = 0xA8,
        BrowserStop = 0xA9,
        BrowserSearch = 0xAA,
        BrowserFavorites = 0xAB,
        BrowserHome = 0xAC,
        VolumeMute = 0xAD,
        VolumeDown = 0xAE,
        VolumeUp = 0xAF,
        MediaNextTrack = 0xB0,
        MediaPrevTrack = 0xB1,
        MediaStop = 0xB2,
        MediaPlayPause = 0xB3,
        LaunchMail = 0xB4,
        LaunchMediaSelect = 0xB5,
        LaunchApp1 = 0xB6,
        LaunchApp2 = 0xB7,
        OEM1 = 0xBA,
        OEMPlus = 0xBB,
        OEMComma = 0xBC,
        OEMMinus = 0xBD,
        OEMPeriod = 0xBE,
        OEM2 = 0xBF,
        OEM3 = 0xC0,
        OEM4 = 0xDB,
        OEM5 = 0xDC,
        OEM6 = 0xDD,
        OEM7 = 0xDE,
        OEM8 = 0xDF,
        OEM102 = 0xE2,
        ProcessKey = 0xE5,
        Packet = 0xE7,
        Attention = 0xF6,
        Crsel = 0xF7,
        Exsel = 0xF8,
        ErEOF = 0xF9,
        Play = 0xFA,
        Zoom = 0xFB,
        PA1 = 0xFD,
        OEMClear = 0xFE
    }
}