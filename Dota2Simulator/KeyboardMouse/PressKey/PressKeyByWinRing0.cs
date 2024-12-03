using System.Threading;
using TestKeyboard.DriverStageHelper;
using TestKeyboard.PressKey;

namespace Dota2Simulator.KeyboardMouse.PressKey
{
    internal class PressKeyByWinRing0 : IPressKey
    {
        public bool Initialize(EnumWindowsType winType)
        {
            return WinRing0.init();
        }

        public void KeyDown(char key)
        {
            WinRing0.KeyDown(key); //按下
        }

        public void KeyPress(char key)
        {
            KeyDown(key); //按下
            Thread.Sleep(100);
            KeyUp(key); //松开
        }

        public void KeyUp(char key)
        {
            WinRing0.KeyUp(key); //松开
        }
    }
}