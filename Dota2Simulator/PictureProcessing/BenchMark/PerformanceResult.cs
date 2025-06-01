using ImageProcessingSystem;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OptimizedGraphics;

namespace Dota2Simulator.PictureProcessing.BenchMark
{
    #region 测试结果
    /*
    结论 DesktopDuplication 缓存句柄最优,其次全局捕获,其次局部捕获
     === 全屏捕获策略 ===
    样本数: 50
    平均耗时: 4.19ms
    最小/最大: 2.50ms / 8.79ms
    中位数: 4.37ms
    标准差: 1.24ms
    P95/P99: 6.90ms / 8.79ms
    吞吐量: 238.6/秒
    内存使用: 229.47MB
    总耗时: 209ms


    === 多次局部捕获策略 ===
    样本数: 50
    平均耗时: 5.23ms
    最小/最大: 4.72ms / 6.36ms
    中位数: 5.14ms
    标准差: 0.34ms
    P95/P99: 5.99ms / 6.36ms
    吞吐量: 191.0/秒
    内存使用: 135.77MB
    总耗时: 261ms


    === 缓存句柄策略 ===
    样本数: 50
    平均耗时: 1.77ms
    最小/最大: 1.64ms / 2.26ms
    中位数: 1.72ms
    标准差: 0.14ms
    P95/P99: 2.15ms / 2.26ms
    吞吐量: 565.9/秒
    内存使用: 0.08MB
    总耗时: 88ms
     */

    /*
     2160p 缓存
     === Desktop Duplication 方法 ===
        样本数: 100
        平均耗时: 2.76ms
        最小/最大: 2.57ms / 3.20ms
        中位数: 2.74ms
        标准差: 0.13ms
        P95/P99: 3.06ms / 3.13ms
        吞吐量: 362.0/秒
        内存使用: 0.10MB
        总耗时: 276ms
    1080p 缓存
    === Desktop Duplication 方法 ===
        样本数: 100
        平均耗时: 1.31ms
        最小/最大: 1.20ms / 2.98ms
        中位数: 1.28ms
        标准差: 0.18ms
        P95/P99: 1.40ms / 1.55ms
        吞吐量: 765.0/秒
        内存使用: 0.09MB
        总耗时: 131ms

    2160p 截图
    === BitBlt 方法 ===
    样本数: 100
    平均耗时: 43.25ms
    最小/最大: 39.63ms / 56.98ms
    中位数: 42.03ms
    标准差: 3.15ms
    P95/P99: 49.60ms / 51.10ms
    吞吐量: 23.1/秒
    内存使用: 379.71MB
    总耗时: 4325ms


    === Desktop Duplication 方法 ===
    样本数: 100
    平均耗时: 8.65ms
    最小/最大: 6.43ms / 16.99ms
    中位数: 7.57ms
    标准差: 2.20ms
    P95/P99: 12.93ms / 15.92ms
    吞吐量: 115.6/秒
    内存使用: 379.73MB
    总耗时: 865ms


    === GDI+ 方法 ===
    样本数: 100
    平均耗时: 57.77ms
    最小/最大: 49.69ms / 73.22ms
    中位数: 56.05ms
    标准差: 4.22ms
    P95/P99: 65.54ms / 71.93ms
    吞吐量: 17.3/秒
    内存使用: 379.71MB
    总耗时: 5777ms

    1080p截图
    === BitBlt 方法 ===
    样本数: 100
    平均耗时: 14.97ms
    最小/最大: 11.30ms / 25.56ms
    中位数: 14.04ms
    标准差: 2.53ms
    P95/P99: 20.31ms / 22.11ms
    吞吐量: 66.8/秒
    内存使用: 261.06MB
    总耗时: 1498ms


    === Desktop Duplication 方法 ===
    样本数: 100
    平均耗时: 2.90ms
    最小/最大: 2.01ms / 8.08ms
    中位数: 2.41ms
    标准差: 1.08ms
    P95/P99: 4.42ms / 7.45ms
    吞吐量: 345.1/秒
    内存使用: 261.10MB
    总耗时: 289ms


    === GDI+ 方法 ===
    样本数: 100
    平均耗时: 16.64ms
    最小/最大: 13.25ms / 21.75ms
    中位数: 14.39ms
    标准差: 3.26ms
    P95/P99: 21.14ms / 21.32ms
    吞吐量: 60.1/秒
    内存使用: 261.06MB
    总耗时: 1664ms
    */
    #endregion

    /// <summary>
    /// 性能测试结果
    /// </summary>
    public class PerformanceResult
    {
        public string Method { get; set; }
        public double AverageTimeMs { get; set; }
        public double MinTimeMs { get; set; }
        public double MaxTimeMs { get; set; }
        public double MedianTimeMs { get; set; }
        public double StdDeviation { get; set; }
        public double P95TimeMs { get; set; }  // 95分位数
        public double P99TimeMs { get; set; }  // 99分位数
        public long TotalTimeMs { get; set; }
        public int SampleCount { get; set; }
        public double ThroughputPerSecond { get; set; }
        public long MemoryUsedBytes { get; set; }
        public Dictionary<string, object> ExtraMetrics { get; set; } = new Dictionary<string, object>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {Method} ===");
            sb.AppendLine($"样本数: {SampleCount}");
            sb.AppendLine($"平均耗时: {AverageTimeMs:F2}ms");
            sb.AppendLine($"最小/最大: {MinTimeMs:F2}ms / {MaxTimeMs:F2}ms");
            sb.AppendLine($"中位数: {MedianTimeMs:F2}ms");
            sb.AppendLine($"标准差: {StdDeviation:F2}ms");
            sb.AppendLine($"P95/P99: {P95TimeMs:F2}ms / {P99TimeMs:F2}ms");
            sb.AppendLine($"吞吐量: {ThroughputPerSecond:F1}/秒");
            sb.AppendLine($"内存使用: {MemoryUsedBytes / 1024.0 / 1024.0:F2}MB");
            sb.AppendLine($"总耗时: {TotalTimeMs}ms");

            if (ExtraMetrics.Count > 0)
            {
                sb.AppendLine("额外指标:");
                foreach (var metric in ExtraMetrics)
                {
                    sb.AppendLine($"  {metric.Key}: {metric.Value}");
                }
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// 性能测试运行器
    /// </summary>
    public class PerformanceTestRunner
    {
        private readonly List<double> _timings = new List<double>();
        private readonly Stopwatch _totalStopwatch = new Stopwatch();
        private long _initialMemory;
        private long _peakMemory;

        public string TestName { get; }
        /// <summary>
        ///     预测次数用于缓存
        /// </summary>
        public int WarmupIterations { get; set; } = 10;
        /// <summary>
        ///     测试次数
        /// </summary>
        public int TestIterations { get; set; } = 100;

        public PerformanceTestRunner(string testName)
        {
            TestName = testName;
        }

        public async Task<PerformanceResult> RunAsync(Func<Task> testAction)
        {
            // 预热
            for (int i = 0; i < WarmupIterations; i++)
            {
                await testAction();
            }

            // 强制GC，获取基准内存
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            _initialMemory = GC.GetTotalMemory(false);

            // 开始测试
            _totalStopwatch.Start();

            for (int i = 0; i < TestIterations; i++)
            {
                var sw = Stopwatch.StartNew();
                await testAction();
                sw.Stop();

                _timings.Add(sw.Elapsed.TotalMilliseconds);

                // 更新峰值内存
                var currentMemory = GC.GetTotalMemory(false);
                if (currentMemory > _peakMemory)
                    _peakMemory = currentMemory;
            }

            _totalStopwatch.Stop();

            return CalculateResults();
        }

        public PerformanceResult Run(Action testAction)
        {
            // 预热
            for (int i = 0; i < WarmupIterations; i++)
            {
                testAction();
            }

            // 强制GC，获取基准内存
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            _initialMemory = GC.GetTotalMemory(false);

            // 开始测试
            _totalStopwatch.Start();

            for (int i = 0; i < TestIterations; i++)
            {
                var sw = Stopwatch.StartNew();
                testAction();
                sw.Stop();

                _timings.Add(sw.Elapsed.TotalMilliseconds);

                // 更新峰值内存
                var currentMemory = GC.GetTotalMemory(false);
                if (currentMemory > _peakMemory)
                    _peakMemory = currentMemory;
            }

            _totalStopwatch.Stop();

            return CalculateResults();
        }

        private PerformanceResult CalculateResults()
        {
            _timings.Sort();

            var result = new PerformanceResult
            {
                Method = TestName,
                SampleCount = _timings.Count,
                AverageTimeMs = _timings.Average(),
                MinTimeMs = _timings.Min(),
                MaxTimeMs = _timings.Max(),
                MedianTimeMs = GetPercentile(_timings, 50),
                P95TimeMs = GetPercentile(_timings, 95),
                P99TimeMs = GetPercentile(_timings, 99),
                TotalTimeMs = _totalStopwatch.ElapsedMilliseconds,
                MemoryUsedBytes = _peakMemory - _initialMemory
            };

            // 计算标准差
            double mean = result.AverageTimeMs;
            double sumOfSquares = _timings.Sum(t => Math.Pow(t - mean, 2));
            result.StdDeviation = Math.Sqrt(sumOfSquares / _timings.Count);

            // 计算吞吐量
            result.ThroughputPerSecond = 1000.0 / result.AverageTimeMs;

            return result;
        }

        private static double GetPercentile(List<double> sortedValues, int percentile)
        {
            int index = (int)Math.Ceiling(percentile / 100.0 * sortedValues.Count) - 1;
            return sortedValues[Math.Max(0, Math.Min(index, sortedValues.Count - 1))];
        }
    }

    /// <summary>
    /// 性能比较器
    /// </summary>
    public class PerformanceComparator
    {
        private readonly List<PerformanceResult> _results = new List<PerformanceResult>();

        public void AddResult(PerformanceResult result)
        {
            _results.Add(result);
        }

        public string GetComparisonReport()
        {
            if (_results.Count == 0) return "没有测试结果";

            var sb = new StringBuilder();
            sb.AppendLine("=== 性能对比报告 ===\n");

            // 找出最快的方法
            var fastest = _results.OrderBy(r => r.AverageTimeMs).First();

            // 表头
            sb.AppendLine($"{"方法",-20} {"平均耗时",-12} {"相对性能",-10} {"吞吐量/秒",-12} {"内存使用MB",-10}");
            sb.AppendLine(new string('-', 70));

            foreach (var result in _results.OrderBy(r => r.AverageTimeMs))
            {
                double relativePeformance = fastest.AverageTimeMs / result.AverageTimeMs;
                string performanceText = result == fastest ? $"最快{relativePeformance:F2}x" : $"{relativePeformance:F2}x";

                sb.AppendLine($"{result.Method,-20} {result.AverageTimeMs,-12:F2}ms {performanceText,-10} " +
                            $"{result.ThroughputPerSecond,-12:F1} {result.MemoryUsedBytes / 1024.0 / 1024.0,-10:F2}");
            }

            sb.AppendLine("\n详细统计:");
            foreach (var result in _results)
            {
                sb.AppendLine($"\n{result}");
            }

            return sb.ToString();
        }

        public PerformanceResult GetBest(Func<PerformanceResult, double> metric = null)
        {
            metric = metric ?? (r => r.AverageTimeMs);
            return _results.OrderBy(metric).FirstOrDefault();
        }
    }

    /// <summary>
    /// 完整的性能测试套件
    /// </summary>
    public static class ScreenCapturePerformanceTestSuite
    {
        // 创建一个静态的 Logger 实例
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     DesktopDuplication 测试
        /// </summary>
        /// <returns></returns>
        public static async Task RunCompleteTestSuite()
        {
            var comparator = new PerformanceComparator();

            // 测试配置
            var testRegions = new[]
            {
                new System.Drawing.Rectangle(100, 100, 200, 200),
                new System.Drawing.Rectangle(500, 300, 400, 300),
                new System.Drawing.Rectangle(1000, 600, 800, 600),
                new System.Drawing.Rectangle(50, 50, 100, 100),
                new System.Drawing.Rectangle(1500, 800, 300, 200),
            };

            _logger.Info("开始性能测试...\n");

            // 测试1: 全屏捕获策略
            var fullScreenTest = new PerformanceTestRunner("全屏捕获策略")
            {
                WarmupIterations = 5,
                TestIterations = 100
            };

            var fullScreenResult = await fullScreenTest.RunAsync(async () =>
            {
                var handle = CaptureScreen(0, 0, 1920, 1080);

                // 处理各个区域
                foreach (var region in testRegions)
                {
                    for (int y = 0; y < region.Height; y += 10)
                    {
                        for (int x = 0; x < region.Width; x += 10)
                        {
                            var pixel = ImageManager.GetPixel(handle, region.X + x, region.Y + y);
                        }
                    }
                }

                ImageManager.ReleaseImage(handle);
                await Task.Yield();
            });

            comparator.AddResult(fullScreenResult);

            // 测试2: 多次局部捕获策略
            var multiCaptureTest = new PerformanceTestRunner("多次局部捕获策略")
            {
                WarmupIterations = 5,
                TestIterations = 100
            };

            var multiCaptureResult = await multiCaptureTest.RunAsync(async () =>
            {
                var handles = new List<ImageHandle>();

                foreach (var region in testRegions)
                {
                    var handle = CaptureScreen(
                        region.X, region.Y, region.Width, region.Height);
                    handles.Add(handle);

                    // 处理区域
                    for (int y = 0; y < region.Height; y += 10)
                    {
                        for (int x = 0; x < region.Width; x += 10)
                        {
                            var pixel = ImageManager.GetPixel(handle, x, y);
                        }
                    }
                }

                foreach (var handle in handles)
                {
                    ImageManager.ReleaseImage(handle);
                }

                await Task.Yield();
            });

            comparator.AddResult(multiCaptureResult);

            // 测试3: 缓存策略
            var cachedHandle = GetOrCreateScreenCaptureHandle(
                "CachedScreen", 1920, 1080);

            var cachedTest = new PerformanceTestRunner("缓存句柄策略")
            {
                WarmupIterations = 5,
                TestIterations = 50
            };

            var cachedResult = await cachedTest.RunAsync(async () =>
            {
                // 更新缓存的句柄
                CaptureScreenToHandle(cachedHandle, 0, 0);

                // 处理各个区域
                foreach (var region in testRegions)
                {
                    for (int y = 0; y < region.Height; y += 10)
                    {
                        for (int x = 0; x < region.Width; x += 10)
                        {
                            var pixel = ImageManager.GetPixel(cachedHandle,
                                region.X + x, region.Y + y);
                        }
                    }
                }

                await Task.Yield();
            });

            comparator.AddResult(cachedResult);

            // 输出比较报告
            _logger.Info(comparator.GetComparisonReport());

            // 清理
            ReleaseScreenCaptureHandle("CachedScreen");
        }

        /// <summary>
        ///     专门测试不同捕获方法的性能
        /// </summary>
        public static void TestCaptureMethodsPerformance()
        {
            var comparator = new PerformanceComparator();
            var testRegion = new System.Drawing.Rectangle(0, 0, 1920, 1080);

            var cachedHandle = GetOrCreateScreenCaptureHandle(
                "CachedScreen", 1920, 1080);

            var 使用缓存 = false;

            // 测试各种捕获方法
            var methods = new[]
            {
                ("BitBlt", CaptureMethod.BitBlt),
                ("Desktop Duplication", CaptureMethod.DesktopDuplication),
                ("GDI+", CaptureMethod.GdiPlus)
            };

            foreach (var (name, method) in methods)
            {
                try
                {
                    // 临时设置捕获方法
                    SetPreferredCaptureMethod(method);

                    var test = new PerformanceTestRunner($"{name} 方法")
                    {
                        WarmupIterations = 5,
                        TestIterations = 100
                    };

                    if (使用缓存 && method == CaptureMethod.DesktopDuplication)
                    {
                        var result = test.Run(() =>
                        {
                            // 更新缓存的句柄
                            CaptureScreenToHandle(cachedHandle, 0, 0);
                        });

                        comparator.AddResult(result);
                    }
                    else
                    {
                        var result = test.Run(() =>
                        {
                            var handle = CaptureScreen(
                                testRegion.X, testRegion.Y,
                                testRegion.Width, testRegion.Height);
                            ImageManager.ReleaseImage(handle);
                        });

                        comparator.AddResult(result);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info($"{name} 测试失败: {ex.Message}");
                }
            }

            _logger.Info(comparator.GetComparisonReport());
        }

        // 辅助方法：设置首选捕获方法（需要在OptimizedGraphics中添加）
        public static void SetPreferredCaptureMethod(CaptureMethod method)
        {
            // 需要在OptimizedGraphics类中添加此方法
            SetPreferredMethod(method);
        }
    }
}