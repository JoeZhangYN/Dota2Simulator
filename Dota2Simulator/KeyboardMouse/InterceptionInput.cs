using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dota2Simulator
{
    /// <summary>
    /// Interception 驱动级输入模拟库
    /// 需要：
    /// 1. 安装 Interception 驱动并重启
    /// 2. interception_input.dll 和 interception.dll 放在程序目录
    /// 3. 以管理员权限运行
    /// </summary>
    public static class InterceptionInput
    {
        private const string DllName = "interception_input.dll";

        #region 初始化

        /// <summary>
        /// 初始化 Interception（可选，会自动初始化）
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Init();

        /// <summary>
        /// 释放资源
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Cleanup();

        /// <summary>
        /// 延迟指定毫秒
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Delay(ulong ms);

        #endregion

        #region 键盘

        /// <summary>
        /// 按键点击（按下后立即抬起）
        /// </summary>
        /// <param name="code">Windows 虚拟键码 (Keys 枚举值)</param>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool KeyPress(ushort code);

        /// <summary>
        /// 按键按下
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool KeyDown(ushort code);

        /// <summary>
        /// 按键抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool KeyUp(ushort code);

        /// <summary>
        /// 按键按下并保持指定毫秒后抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool KeyPressHold(ushort code, ulong holdMs);

        // Keys 枚举重载版本
        public static bool KeyPress(Keys key) => KeyPress((ushort)key);
        public static bool KeyDown(Keys key) => KeyDown((ushort)key);
        public static bool KeyUp(Keys key) => KeyUp((ushort)key);
        public static bool KeyPressHold(Keys key, ulong holdMs) => KeyPressHold((ushort)key, holdMs);

        #endregion

        #region 鼠标左键

        /// <summary>
        /// 鼠标左键点击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseLeftClick();

        /// <summary>
        /// 鼠标左键双击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseLeftDoubleClick();

        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseLeftDown();

        /// <summary>
        /// 鼠标左键抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseLeftUp();

        /// <summary>
        /// 鼠标左键按下并保持指定毫秒后抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseLeftClickHold(ulong holdMs);

        #endregion

        #region 鼠标右键

        /// <summary>
        /// 鼠标右键点击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseRightClick();

        /// <summary>
        /// 鼠标右键双击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseRightDoubleClick();

        /// <summary>
        /// 鼠标右键按下
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseRightDown();

        /// <summary>
        /// 鼠标右键抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseRightUp();

        /// <summary>
        /// 鼠标右键按下并保持指定毫秒后抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseRightClickHold(ulong holdMs);

        #endregion

        #region 鼠标中键

        /// <summary>
        /// 鼠标中键点击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseMiddleClick();

        /// <summary>
        /// 鼠标中键双击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseMiddleDoubleClick();

        /// <summary>
        /// 鼠标中键按下
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseMiddleDown();

        /// <summary>
        /// 鼠标中键抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseMiddleUp();

        /// <summary>
        /// 鼠标中键按下并保持指定毫秒后抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseMiddleClickHold(ulong holdMs);

        /// <summary>
        /// 鼠标滚轮向上滚动
        /// </summary>
        /// <param name="clicks">滚动格数</param>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseScrollUp(short clicks);

        /// <summary>
        /// 鼠标滚轮向下滚动
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseScrollDown(short clicks);

        /// <summary>
        /// 鼠标滚轮向左滚动（水平滚轮）
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseScrollLeft(short clicks);

        /// <summary>
        /// 鼠标滚轮向右滚动（水平滚轮）
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseScrollRight(short clicks);

        #endregion

        #region 鼠标侧键1 (XButton1 / Back)

        /// <summary>
        /// 鼠标侧键1点击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton1Click();

        /// <summary>
        /// 鼠标侧键1双击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton1DoubleClick();

        /// <summary>
        /// 鼠标侧键1按下
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton1Down();

        /// <summary>
        /// 鼠标侧键1抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton1Up();

        /// <summary>
        /// 鼠标侧键1按下并保持指定毫秒后抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton1ClickHold(ulong holdMs);

        #endregion

        #region 鼠标侧键2 (XButton2 / Forward)

        /// <summary>
        /// 鼠标侧键2点击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton2Click();

        /// <summary>
        /// 鼠标侧键2双击
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton2DoubleClick();

        /// <summary>
        /// 鼠标侧键2按下
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton2Down();

        /// <summary>
        /// 鼠标侧键2抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton2Up();

        /// <summary>
        /// 鼠标侧键2按下并保持指定毫秒后抬起
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseXButton2ClickHold(ulong holdMs);

        #endregion

        #region 鼠标移动

        /// <summary>
        /// 鼠标相对移动
        /// </summary>
        /// <param name="dx">X 方向偏移（像素）</param>
        /// <param name="dy">Y 方向偏移（像素）</param>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseMoveRelative(int dx, int dy);

        /// <summary>
        /// 鼠标绝对移动（坐标范围 0-65535）
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseMoveAbsolute(int x, int y);

        /// <summary>
        /// 鼠标移动到屏幕坐标（像素）
        /// </summary>
        /// <param name="x">屏幕 X 坐标</param>
        /// <param name="y">屏幕 Y 坐标</param>
        /// <param name="screenWidth">屏幕宽度</param>
        /// <param name="screenHeight">屏幕高度</param>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MouseMoveTo(int x, int y, int screenWidth, int screenHeight);

        #endregion
    }

    ///// <summary>
    ///// Windows 虚拟键码
    ///// </summary>
    //public enum Keys : ushort
    //{
    //    // 鼠标按钮（一般不用于键盘函数）
    //    LButton = 0x01,
    //    RButton = 0x02,
    //    Cancel = 0x03,
    //    MButton = 0x04,
    //    XButton1 = 0x05,
    //    XButton2 = 0x06,

    //    // 控制键
    //    Back = 0x08,        // Backspace
    //    Tab = 0x09,
    //    Clear = 0x0C,
    //    Return = 0x0D,      // Enter
    //    Enter = 0x0D,
    //    Shift = 0x10,
    //    Control = 0x11,
    //    Ctrl = 0x11,
    //    Menu = 0x12,        // Alt
    //    Alt = 0x12,
    //    Pause = 0x13,
    //    Capital = 0x14,     // CapsLock
    //    CapsLock = 0x14,

    //    // IME 键
    //    Kana = 0x15,
    //    Hangul = 0x15,
    //    Junja = 0x17,
    //    Final = 0x18,
    //    Hanja = 0x19,
    //    Kanji = 0x19,

    //    // 特殊键
    //    Escape = 0x1B,
    //    Esc = 0x1B,
    //    Convert = 0x1C,
    //    NonConvert = 0x1D,
    //    Accept = 0x1E,
    //    ModeChange = 0x1F,
    //    Space = 0x20,

    //    // 导航键
    //    PageUp = 0x21,
    //    Prior = 0x21,
    //    PageDown = 0x22,
    //    Next = 0x22,
    //    End = 0x23,
    //    Home = 0x24,
    //    Left = 0x25,
    //    Up = 0x26,
    //    Right = 0x27,
    //    Down = 0x28,

    //    // 编辑键
    //    Select = 0x29,
    //    Print = 0x2A,
    //    Execute = 0x2B,
    //    Snapshot = 0x2C,    // PrintScreen
    //    PrintScreen = 0x2C,
    //    Insert = 0x2D,
    //    Delete = 0x2E,
    //    Help = 0x2F,

    //    // 数字键 (主键盘)
    //    D0 = 0x30,
    //    D1 = 0x31,
    //    D2 = 0x32,
    //    D3 = 0x33,
    //    D4 = 0x34,
    //    D5 = 0x35,
    //    D6 = 0x36,
    //    D7 = 0x37,
    //    D8 = 0x38,
    //    D9 = 0x39,

    //    // 字母键
    //    A = 0x41,
    //    B = 0x42,
    //    C = 0x43,
    //    D = 0x44,
    //    E = 0x45,
    //    F = 0x46,
    //    G = 0x47,
    //    H = 0x48,
    //    I = 0x49,
    //    J = 0x4A,
    //    K = 0x4B,
    //    L = 0x4C,
    //    M = 0x4D,
    //    N = 0x4E,
    //    O = 0x4F,
    //    P = 0x50,
    //    Q = 0x51,
    //    R = 0x52,
    //    S = 0x53,
    //    T = 0x54,
    //    U = 0x55,
    //    V = 0x56,
    //    W = 0x57,
    //    X = 0x58,
    //    Y = 0x59,
    //    Z = 0x5A,

    //    // Windows 键
    //    LWin = 0x5B,
    //    RWin = 0x5C,
    //    Apps = 0x5D,

    //    // 其他
    //    Sleep = 0x5F,

    //    // 小键盘
    //    NumPad0 = 0x60,
    //    NumPad1 = 0x61,
    //    NumPad2 = 0x62,
    //    NumPad3 = 0x63,
    //    NumPad4 = 0x64,
    //    NumPad5 = 0x65,
    //    NumPad6 = 0x66,
    //    NumPad7 = 0x67,
    //    NumPad8 = 0x68,
    //    NumPad9 = 0x69,
    //    Multiply = 0x6A,
    //    Add = 0x6B,
    //    Separator = 0x6C,
    //    Subtract = 0x6D,
    //    Decimal = 0x6E,
    //    Divide = 0x6F,

    //    // 功能键
    //    F1 = 0x70,
    //    F2 = 0x71,
    //    F3 = 0x72,
    //    F4 = 0x73,
    //    F5 = 0x74,
    //    F6 = 0x75,
    //    F7 = 0x76,
    //    F8 = 0x77,
    //    F9 = 0x78,
    //    F10 = 0x79,
    //    F11 = 0x7A,
    //    F12 = 0x7B,
    //    F13 = 0x7C,
    //    F14 = 0x7D,
    //    F15 = 0x7E,
    //    F16 = 0x7F,
    //    F17 = 0x80,
    //    F18 = 0x81,
    //    F19 = 0x82,
    //    F20 = 0x83,
    //    F21 = 0x84,
    //    F22 = 0x85,
    //    F23 = 0x86,
    //    F24 = 0x87,

    //    // 锁定键
    //    NumLock = 0x90,
    //    Scroll = 0x91,
    //    ScrollLock = 0x91,

    //    // 左右修饰键
    //    LShift = 0xA0,
    //    RShift = 0xA1,
    //    LControl = 0xA2,
    //    LCtrl = 0xA2,
    //    RControl = 0xA3,
    //    RCtrl = 0xA3,
    //    LMenu = 0xA4,
    //    LAlt = 0xA4,
    //    RMenu = 0xA5,
    //    RAlt = 0xA5,

    //    // 浏览器键
    //    BrowserBack = 0xA6,
    //    BrowserForward = 0xA7,
    //    BrowserRefresh = 0xA8,
    //    BrowserStop = 0xA9,
    //    BrowserSearch = 0xAA,
    //    BrowserFavorites = 0xAB,
    //    BrowserHome = 0xAC,

    //    // 音量键
    //    VolumeMute = 0xAD,
    //    VolumeDown = 0xAE,
    //    VolumeUp = 0xAF,

    //    // 媒体键
    //    MediaNextTrack = 0xB0,
    //    MediaPreviousTrack = 0xB1,
    //    MediaStop = 0xB2,
    //    MediaPlayPause = 0xB3,

    //    // 启动键
    //    LaunchMail = 0xB4,
    //    SelectMedia = 0xB5,
    //    LaunchApplication1 = 0xB6,
    //    LaunchApplication2 = 0xB7,

    //    // OEM 键 (符号键)
    //    OemSemicolon = 0xBA,    // ;:
    //    Oem1 = 0xBA,
    //    OemPlus = 0xBB,         // =+
    //    OemComma = 0xBC,        // ,<
    //    OemMinus = 0xBD,        // -_
    //    OemPeriod = 0xBE,       // .>
    //    OemQuestion = 0xBF,     // /?
    //    Oem2 = 0xBF,
    //    OemTilde = 0xC0,        // `~
    //    Oem3 = 0xC0,
    //    OemOpenBrackets = 0xDB, // [{
    //    Oem4 = 0xDB,
    //    OemPipe = 0xDC,         // \|
    //    Oem5 = 0xDC,
    //    OemCloseBrackets = 0xDD,// ]}
    //    Oem6 = 0xDD,
    //    OemQuotes = 0xDE,       // '"
    //    Oem7 = 0xDE,
    //    Oem8 = 0xDF,
    //    OemBackslash = 0xE2,
    //    Oem102 = 0xE2,

    //    // 其他
    //    ProcessKey = 0xE5,
    //    Packet = 0xE7,
    //    Attn = 0xF6,
    //    Crsel = 0xF7,
    //    Exsel = 0xF8,
    //    EraseEof = 0xF9,
    //    Play = 0xFA,
    //    Zoom = 0xFB,
    //    NoName = 0xFC,
    //    Pa1 = 0xFD,
    //    OemClear = 0xFE,
    //}
}
