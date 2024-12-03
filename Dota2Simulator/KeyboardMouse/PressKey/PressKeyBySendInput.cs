using System.Runtime.InteropServices;
using System.Windows.Forms;
using static TestKeyboard.DriverStageHelper.SendInputHelper;

namespace TestKeyboard.PressKey
{
    internal class PressKeyBySendInput
    {
        public static bool Initialize(EnumWindowsType winType)
        {
            return true;
        }

        public static void KeyDown(short key)
        {
            INPUT[] input = new INPUT[1];
            input[0].type = 1;
            input[0].ki.WVk = key;
            input[0].ki.DwFlags = 0;

            _ = SendInput((uint)input.Length, input, Marshal.SizeOf(default(INPUT)));
        }

        public static void KeyPress(short key)
        {
            KeyDown(key);
            Application.DoEvents();
            KeyUp(key);
        }

        public static void KeyUp(short key)
        {
            INPUT[] input = new INPUT[1];
            input[0].type = 1;
            input[0].ki.WVk = key;
            input[0].ki.DwFlags = 2;

            _ = SendInput((uint)input.Length, input, Marshal.SizeOf(default(INPUT)));
        }
    }
}