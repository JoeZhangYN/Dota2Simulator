using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Dota2Simulator.Infrastructure.Input;

namespace Dota2Simulator.KeyboardMouse
{
    /// <summary>
    /// interception_input.dll（Interception 内核驱动后端）绑定。
    /// FFI 面 = input_abi 契约 42 符号，与 <see cref="SimEnigo"/> 同面——
    /// 任何调用点换类名即换后端（simengio 另有契约外 <c>Text</c>）。
    /// 需要：1. 安装 Interception 驱动并重启；2. interception_input.dll 和 interception.dll
    /// 放在程序目录；3. 以管理员权限运行。
    /// 返回值约定：0 = 成功，正值 = InputError 错误码。
    /// </summary>
    /// <remarks>
    /// 契约 41 个标量签名 extern 由 <c>InputAbiExternGenerator</c> 从 SSOT manifest 生成
    /// （见 <see cref="InputAbiBindingsAttribute"/>）。本文件只手写：LastError（缓冲协议）、Keys 重载。
    /// </remarks>
    [InputAbiBindings("interception_input.dll")]
    public static partial class InterceptionInput
    {
        private const string DllName = "interception_input.dll";

        #region 生命周期诊断（LastError 缓冲协议，手写）

        [DllImport(DllName, EntryPoint = "LastError", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr NativeLastError(byte[]? buf, UIntPtr bufLen);

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

        #region 键盘 Keys 重载（手写，转发到生成的 ushort extern）

        public static int KeyPress(Keys key) => KeyPress((ushort)key);
        public static int KeyDown(Keys key) => KeyDown((ushort)key);
        public static int KeyUp(Keys key) => KeyUp((ushort)key);
        public static int KeyPressHold(Keys key, ulong holdMs) => KeyPressHold((ushort)key, holdMs);
        public static int KeyPressWhile(Keys key, Keys modifier) => KeyPressWhile((ushort)key, (ushort)modifier);
        public static int KeyPressWhileTwo(Keys key, Keys mod1, Keys mod2) => KeyPressWhileTwo((ushort)key, (ushort)mod1, (ushort)mod2);
        public static int KeyPressAlt(Keys key) => KeyPressAlt((ushort)key);

        #endregion
    }
}
