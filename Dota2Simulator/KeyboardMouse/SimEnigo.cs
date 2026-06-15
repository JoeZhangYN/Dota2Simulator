using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Dota2Simulator.Infrastructure.Input;

namespace Dota2Simulator.KeyboardMouse
{
    /// <summary>
    /// simengio.dll（enigo 用户态后端）绑定。
    /// FFI 面 = input_abi 契约 42 符号 + <c>simengio_text</c>（契约外专属），
    /// 与 <see cref="InterceptionInput"/> 同面——任何调用点换类名即换后端。
    /// 返回值约定：0 = 成功，负值 = simengio 错误码（诊断文本走 <see cref="LastError"/>）。
    /// </summary>
    /// <remarks>
    /// 契约 41 个标量签名 extern 由 <c>InputAbiExternGenerator</c> 从 SSOT manifest
    /// （Infrastructure/Input/input_abi.contract.txt）生成（见 <see cref="InputAbiBindingsAttribute"/>）。
    /// 本文件只手写：LastError（缓冲协议）、Keys 重载、simengio_text（契约外）。
    /// </remarks>
    [InputAbiBindings("simengio.dll")]
    internal static partial class SimEnigo
    {
        private const string DllName = "simengio.dll";

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
        public static int KeyPressWhile(Keys key, Keys modifier) => KeyPressWhile((ushort)key, (ushort)modifier);
        public static int KeyPressWhileTwo(Keys key, Keys mod1, Keys mod2) => KeyPressWhileTwo((ushort)key, (ushort)mod1, (ushort)mod2);
        public static int KeyPressAlt(Keys key) => KeyPressAlt((ushort)key);

        #endregion

        #region 契约外专属（interception 后端无 Unicode 通道，simengio 独有；手写）

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
