using System.Runtime.InteropServices;

namespace Dota2Simulator.KeyboardMouse
{
    public class kmclassAPI
    {
        private const string DriverFileName = "kmclassdll.dll";

        [DllImport(DriverFileName)]
        public static extern void KeyDown(uint key);

        [DllImport(DriverFileName)]
        public static extern void KeyUp(uint key);

    }
}
