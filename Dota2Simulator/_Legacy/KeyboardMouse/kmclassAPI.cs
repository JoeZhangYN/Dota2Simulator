// TODO-dead-Phase7-remove
// Reason: csproj Compile Remove (_Legacy/**), no live ref
// Re-enable: delete this header + restore .csproj Compile line (if any).
using System.Runtime.InteropServices;

namespace Dota2Simulator.KeyboardMouse
{
    internal class kmclassAPI
    {
        private const string DriverFileName = "kmclassdll.dll";

        [DllImport(DriverFileName)]
        public static extern void KeyDown(uint key);

        [DllImport(DriverFileName)]
        public static extern void KeyUp(uint key);
    }
}