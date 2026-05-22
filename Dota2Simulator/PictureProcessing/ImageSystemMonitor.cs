using Collections.Pooled;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Threading.Timer;
using Tuple = Dota2Simulator.ImageProcessingSystem.ImageFinder.Tuple;

namespace Dota2Simulator.ImageProcessingSystem
{
    // 系统监控维护
    public static class ImageSystemMonitor
    {
        private static readonly Timer _maintenanceTimer;
        private static bool _autoMaintenanceEnabled = true;
        private static ILogger _logger = new ConsoleLogger();

        public static void SetLogger(ILogger logger)
        {
            _logger = logger ?? new ConsoleLogger();
        }

        public static void PrintStatus()
        {
            _logger.LogInfo("=== 图像系统状态 ===");
            ImageProcessingSystem.ReportUsage();
            _logger.LogInfo($"静态缓存数:{StaticImageCache.Count}");
            _logger.LogInfo($"像素缓存数: {PixelCodeCache.CacheSize}");
            _logger.LogInfo("==================");
        }

        public static bool AutoMaintenanceEnabled
        {
            get => _autoMaintenanceEnabled;
            set
            {
                _autoMaintenanceEnabled = value;
                if (value)
                {
                    _maintenanceTimer.Change(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
                }
                else
                {
                    _maintenanceTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        // 手动触发完整清理
        public static void PerformFullCleanup()
        {
            _logger.LogInfo("执行完整清理...");

            // 清理缓存
            StaticImageCache.Clear();
            DynamicImageBuffer.Cleanup();
            PixelCodeCache.Clear();

            // 强制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            _logger.LogInfo("清理完成");
            PrintStatus();
        }
    }
}
