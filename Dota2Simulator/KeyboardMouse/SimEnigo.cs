using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Dota2Simulator.KeyboardMouse
{
    /// <summary>
    /// simengio.dll（enigo 用户态后端）绑定。
    /// FFI 面 = input_abi 契约 42 符号 + <c>simengio_text</c>（契约外专属），
    /// 与 <see cref="InterceptionInput"/> 同面——任何调用点换类名即换后端。
    /// 返回值约定：0 = 成功，负值 = simengio 错误码（诊断文本走 <see cref="LastError"/>）。
    /// </summary>
    internal static class SimEnigo
    {
        private const string DllName = "simengio.dll";

        #region 生命周期

        /// <summary>可选的显式预热（幂等）。首次调用任意操作函数会自动初始化。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Init();

        /// <summary>释放 enigo 实例。可重复调用；再次调用任意操作函数会自动重新初始化。</summary>
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

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyPress(ushort code);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyDown(ushort code);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int KeyUp(ushort code);

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
        public static int KeyPressWhile(Keys key, Keys modifier) => KeyPressWhile((ushort)key, (ushort)modifier);
        public static int KeyPressWhileTwo(Keys key, Keys mod1, Keys mod2) => KeyPressWhileTwo((ushort)key, (ushort)mod1, (ushort)mod2);
        public static int KeyPressAlt(Keys key) => KeyPressAlt((ushort)key);

        #endregion

        #region 鼠标按钮

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

        #region 滚轮 / 移动

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseScrollUp(short clicks);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseScrollDown(short clicks);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseScrollLeft(short clicks);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseScrollRight(short clicks);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMoveRelative(int dx, int dy);

        /// <summary>0-65535 虚拟坐标（simengio 按主显示器比例换算；interception 为虚拟桌面坐标基，单屏一致）。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMoveAbsolute(int x, int y);

        /// <summary>屏幕像素坐标移动。</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MouseMoveTo(int x, int y, int screenWidth, int screenHeight);

        #endregion

        #region 契约外专属（interception 后端无 Unicode 通道，simengio 独有）

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int simengio_text(byte[] utf8, UIntPtr len);

        /// <summary>Unicode 文本注入。</summary>
        public static int Text(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            return simengio_text(bytes, (UIntPtr)bytes.Length);
        }

        #endregion
    }
}
