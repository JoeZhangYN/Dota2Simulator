using System.Runtime.InteropServices;

namespace Dota2Simulator.KeyboardMouse
{
    public class SimEnigo
    {

        [DllImport("simengio.dll")]
        public static extern void KeyPress(char s);

        [DllImport("simengio.dll")]
        public static extern void KeyPressControl(char s);

        [DllImport("simengio.dll")]
        public static extern void KeyPressAlt(char s);

        [DllImport("simengio.dll")]
        public static extern void KeyPressShift(char s);
    }
}
