using System;
using System.Runtime.InteropServices;

namespace TestKeyboard.DriverStageHelper;

public class SendInputHelper
{
    private const int KeyEvent_InputType = 1; // INPUT 键盘类型 1

    private const int KeyEvent_KeyDown = 0; // Key Down
    private const int KeyEvent_KeyUp = 0x0002; // Key Up

    private const int KeyEvent_EXTENDEDKEY = 0x0001;
    private const int KeyEvent_UNICODE = 0x0004;
    private const int KeyEvent_SCANCODE = 0x0008; // SCANCODE

    private const int MouseEvent_InputType = 0; // INPUT 鼠标类型 0

    private const int MouseEvent_Absolute = 0x8000;
    private const int MouserEvent_Hwheel = 0x01000;
    private const int MouseEvent_Move = 0x0001;
    private const int MouseEvent_Move_noCoalesce = 0x2000;
    private const int MouseEvent_LeftDown = 0x0002;
    private const int MouseEvent_LeftUp = 0x0004;
    private const int MouseEvent_MiddleDown = 0x0020;
    private const int MouseEvent_MiddleUp = 0x0040;
    private const int MouseEvent_RightDown = 0x0008;
    private const int MouseEvent_RightUp = 0x0010;
    private const int MouseEvent_Wheel = 0x0800;
    private const int MousseEvent_XUp = 0x0100;
    private const int MousseEvent_XDown = 0x0080;

    /// <summary>
    ///     模拟键盘或鼠标
    /// </summary>
    /// <param name="nInput"> 一个 1u 两个 2u </param>
    /// <param name="pInputs"> Input 构造体数组 </param>
    /// <param name="cbSize"> Input自身所占字节 Marshal.SizeOf(default(INPUT)) </param>
    /// <returns></returns>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint SendInput(uint nInput, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    internal static extern int MapVirtualKey(uint Ucode, uint uMapType);

    [DllImport("user32.dll")]
    internal static extern short VkKeyScan(char ch);

    [DllImport("user32.dll")]
    internal static extern short GetKeyState(int nVirtKey);


    public static void SimulateInputString(string sText)
    {
        var cText = sText.ToCharArray();
        foreach (var c in cText)
        {
            var input = new INPUT[2];
            if (c >= 0 && c < 256) //a-z A-Z
            {
                var num = VkKeyScan(c); //获取虚拟键码值
                if (num != -1)
                {
                    var shift = ((num >> 8) & 1) !=
                                0; //num >>8表示 高位字节上当状态，如果为1则按下Shift，否则没有按下Shift，即大写键CapsLk没有开启时，是否需要按下Shift。
                    if ((GetKeyState(20) & 1) != 0 &&
                        ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))) //Win32API.GetKeyState(20)获取CapsLk大写键状态
                        shift = !shift;

                    if (shift)
                    {
                        input[0].type = 1; //模拟键盘
                        input[0].ki.WVk = 16; //Shift键
                        input[0].ki.DwFlags = 0; //按下
                        SendInput(1u, input, Marshal.SizeOf(default(INPUT)));
                    }

                    input[0].type = 1;
                    input[0].ki.WVk = (short) (num & 0xFF);
                    input[0].ki.DwFlags = 0;

                    input[1].type = 1;
                    input[1].ki.WVk = (short) (num & 0xFF);
                    input[1].ki.DwFlags = 2;
                    SendInput(2u, input, Marshal.SizeOf(default(INPUT)));

                    if (shift)
                    {
                        input[0].type = 1;
                        input[0].ki.WVk = 16;
                        input[0].ki.DwFlags = 2; //抬起
                        SendInput(1u, input, Marshal.SizeOf(default(INPUT)));
                    }

                    continue;
                }
            }

            input[0].type = 1;
            input[0].ki.WVk = 0; //dwFlags 为KEYEVENTF_UNICODE 即4时，wVk必须为0
            input[0].ki.WScan = (short) c;
            input[0].ki.DwFlags = 4; //输入UNICODE字符
            input[0].ki.Time = 0;
            input[0].ki.DwExtraInfo = IntPtr.Zero;
            input[1].type = 1;
            input[1].ki.WVk = 0;
            input[1].ki.WScan = (short) c;
            input[1].ki.DwFlags = 6;
            input[1].ki.Time = 0;
            input[1].ki.DwExtraInfo = IntPtr.Zero;
            SendInput(2u, input, Marshal.SizeOf(default(INPUT)));
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        /// <summary>
        ///     0 为模拟鼠标 1 为模拟键盘
        /// </summary>
        [FieldOffset(0)] public int type;

        /// <summary>
        ///     模拟键盘用到的构造体
        /// </summary>
        [FieldOffset(4)] public Keybdinput ki;

        /// <summary>
        ///     模拟鼠标用到的构造体
        /// </summary>
        [FieldOffset(4)] public MOUSEINPUT mi;

        [FieldOffset(4)] public Hardwareinput hi;
    }

    public struct MOUSEINPUT
    {
        /// <summary>
        ///     <para>坐标x</para>
        /// </summary>
        public int dx;

        /// <summary>
        ///     <para>坐标y</para>
        /// </summary>
        public int dy;

        /// <summary>
        ///     <para>按键数据</para>
        /// </summary>
        public int mouseData;

        /// <summary>
        ///     操作标志位
        /// </summary>
        public int dwFlags;

        /// <summary>
        ///     持续时间
        /// </summary>
        public int time;

        /// <summary>
        ///     指针
        /// </summary>
        public IntPtr dwExtraInfo;
    }

    public struct Keybdinput
    {
        /// <summary>
        ///     <para>按键对应short码</para>
        ///     <para>dwFlags 为KEYEVENTF_UNICODE 即4时，wVk必须为0</para>
        /// </summary>
        public short WVk;

        /// <summary>
        ///     <para>直接按按键不用设置</para>
        ///     <para>dwFlags 为KEYEVENTF_UNICODE 即4时，(short)c</para>
        /// </summary>
        public short WScan;

        /// <summary>
        ///     <para>标志位 0 为按下 2 为弹起</para>
        /// </summary>
        public int DwFlags;

        /// <summary>
        ///     <para>按键时长 单纯按按键不用设置</para>
        /// </summary>
        public int Time;

        /// <summary>
        ///     <para> 单纯按按键不用设置</para>
        ///     <para> 输入UNICODE字符为 IntPtr.Zero</para>
        /// </summary>
        public IntPtr DwExtraInfo;
    }

    public struct Hardwareinput
    {
        public int UMsg;

        public short WParamL;

        public short WParamH;
    }
}