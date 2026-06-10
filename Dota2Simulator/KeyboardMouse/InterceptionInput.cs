using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Dota2Simulator.KeyboardMouse
{
    /// <summary>
    /// interception_input.dll（Interception 内核驱动后端）绑定。
    /// FFI 面 = input_abi 契约 42 符号，与 <see cref="SimEnigo"/> 同面——
    /// 任何调用点换类名即换后端（simengio 另有契约外 <c>Text</c>）。
    /// 需要：
    /// 1. 安装 Interception 驱动并重启
    /// 2. interception_input.dll 和 interception.dll 放在程序目录
    /// 3. 以管理员权限运行
    /// 返回值约定：0 = 成功，正值 = InputError 错误码（旧绑定声明 bool 时
    /// 非 0→true 的语义是反的，已修正为 int）。
    /// </summary>
    public static class InterceptionInput
    {
        private const string DllName = "interception_input.dll";

        #region 生命周期

        /// <summary>显式初始化（可选，首次调用任意操作函数会自动初始化）。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Init();

        /// <summary>释放 Interception 上下文。可重复调用；再次调用任意输入函数会自动重新初始化。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Cleanup();

        [DllImport(DllName, EntryPoint = "LastError", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr NativeLastError(byte[] buf, UIntPtr bufLen);

        /// <summary>最近一次错误的诊断文本（调用线程局部）。</summary>
        public static string LastError()
        {
            var n = (int)(nuint)NativeLastError(null, UIntPtr.Zero);
            if (n == 0) return string.Empty;
            var buf = new byte[n];
            _ = NativeLastError(buf, (UIntPtr)buf.Length);
            return Encoding.UTF8.GetString(buf, 0, n);
        }

        #endregion

        #region 键盘（code = Windows 虚拟键码）

        /// <summary>按键点击（按下后短暂驻留再抬起）。未映射的 VK 返回 UnmappedKey(2)。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyPress(ushort code);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyDown(ushort code);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyUp(ushort code);

        /// <summary>按下并保持指定毫秒后抬起（原子，期间不会被其他调用交错）。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyPressHold(ushort code, ulong holdMs);

        /// <summary>按住修饰键点击主键：Down(mod) → Click(key) → Up(mod)。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyPressWhile(ushort key, ushort modifier);

        /// <summary>双修饰键组合（契约顺序：Down(mod2)+Down(mod1)+Click(key)+Up(mod2)+Up(mod1)）。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyPressWhileTwo(ushort key, ushort mod1, ushort mod2);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyPressAlt(ushort key);

        public static int KeyPress(Keys key) => KeyPress((ushort)key);
        public static int KeyDown(Keys key) => KeyDown((ushort)key);
        public static int KeyUp(Keys key) => KeyUp((ushort)key);
        public static int KeyPressHold(Keys key, ulong holdMs) => KeyPressHold((ushort)key, holdMs);
        public static int KeyPressWhile(Keys key, Keys modifier) => KeyPressWhile((ushort)key, (ushort)modifier);
        public static int KeyPressWhileTwo(Keys key, Keys mod1, Keys mod2) => KeyPressWhileTwo((ushort)key, (ushort)mod1, (ushort)mod2);
        public static int KeyPressAlt(Keys key) => KeyPressAlt((ushort)key);

        #endregion

        #region 鼠标左键

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseLeftClick();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseLeftDoubleClick();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseLeftDown();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseLeftUp();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseLeftClickHold(ulong holdMs);

        #endregion

        #region 鼠标右键

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseRightClick();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseRightDoubleClick();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseRightDown();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseRightUp();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseRightClickHold(ulong holdMs);

        #endregion

        #region 鼠标中键 + 滚轮

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMiddleClick();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMiddleDoubleClick();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMiddleDown();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMiddleUp();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMiddleClickHold(ulong holdMs);

        /// <summary>鼠标滚轮向上滚动 <paramref name="clicks"/> 格。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseScrollUp(short clicks);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseScrollDown(short clicks);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseScrollLeft(short clicks);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseScrollRight(short clicks);

        #endregion

        #region 鼠标侧键 1 (XButton1 / Back)

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton1Click();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton1DoubleClick();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton1Down();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton1Up();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton1ClickHold(ulong holdMs);

        #endregion

        #region 鼠标侧键 2 (XButton2 / Forward)

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton2Click();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton2DoubleClick();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton2Down();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton2Up();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseXButton2ClickHold(ulong holdMs);

        #endregion

        #region 鼠标移动

        /// <summary>鼠标相对移动（受系统指针速度/加速度影响；simengio 后端不受）。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMoveRelative(int dx, int dy);

        /// <summary>鼠标绝对移动（0-65535 虚拟桌面坐标基；simengio 为主显示器，单屏一致）。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMoveAbsolute(int x, int y);

        /// <summary>鼠标移动到屏幕像素坐标。宽或高 ≤ 0 返回 InvalidScreenSize(3)。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMoveTo(int x, int y, int screenWidth, int screenHeight);

        #endregion
    }
}
