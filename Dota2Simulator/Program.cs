using System;
using System.Windows.Forms;

namespace Dota2Simulator
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // 输入后端 ABI 对账：部署了旧版/漂移 DLL 时启动即报错，而非运行时静默失败。
            Input.NativeAbiCheck.EnsureContractSurface();

            _ = Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DOTA2
            CompositionRoot.AppContainer container = new();
            Application.Run(new Form2(container));
#else
            Application.Run(new Form2());
#endif
        }
    }
}