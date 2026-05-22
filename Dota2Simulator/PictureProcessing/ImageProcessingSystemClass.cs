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
    // 内存预算管理器
    public static class ImageProcessingSystem
    {
        private static long _totalBudget = 512 * 1024 * 1024; // 512mb 默认预算 1920*1080*4 = 6.4mb
        private static long _currentUsage;
        private static ILogger _logger = new ConsoleLogger();

        public static long TotalBudget
        {
            get => _totalBudget;
            set => Interlocked.Exchange(ref _totalBudget, value);
        }

        public static long CurrentUsage => Interlocked.Read(ref _currentUsage);
        public static long AvailableMemory => _totalBudget - CurrentUsage;
        public static double UsagePercentage => (double)CurrentUsage / _totalBudget * 100;

        public static void SetLogger(ILogger logger)
        {
            _logger = logger ?? new ConsoleLogger();
        }

        public static bool TryAllocate(long size)
        {
            long currentUsage, newUsage;
            do
            {
                currentUsage = _currentUsage;
                newUsage = currentUsage + size;
                if (newUsage > _totalBudget)
                    return false;
            } while (Interlocked.CompareExchange(ref _currentUsage, newUsage, currentUsage) != currentUsage);

            return true;
        }

        public static void Deallocate(long size)
        {
            Interlocked.Add(ref _currentUsage, -size);
        }

        public static void ReportUsage()
        {
            _logger.LogInfo($"内存使用: {CurrentUsage / (1024 * 1024):F2}MB / {TotalBudget / (1024 * 1024):F2}MB ({UsagePercentage:F1}%)");
        }
    }
}
