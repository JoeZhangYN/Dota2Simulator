#if DOTA2
#if Silt

using Dota2Simulator.ImageProcessingSystem;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.PictureProcessing.OCR;
using Dota2Simulator.PictureProcessing.RustImageProcessingSystem;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Dota2Simulator.Games.Dota2.Item;
using Color = System.Drawing.Color;
using ImageHandle = Dota2Simulator.ImageProcessingSystem.ImageHandle;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Dota2Simulator.Games.Dota2.Silt
{
    internal class Main
    {
        private static bool 已吃书;

        public static async Task<bool> 有书吃书(ImageHandle 句柄)
        {
            if (已吃书
                && 根据图片使用物品(Dota2_Pictrue.物品.书) == 0)
            {
                SimKeyBoard.KeyPress(Keys.D2);
                已吃书 = false;
            }
            else if (根据图片使用物品(Dota2_Pictrue.物品.书) == 1)
            {
                Common.Delay(50);
                已吃书 = true;
            }
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        private readonly static Rectangle RPG选择技能范围 = new(379, 115, 1140, 153);
        private readonly static Rectangle RPG第一技能金 = new(631, 290, 60, 400);

        private static int 获取碎片循环计数;
        private static Point 之前位置;

        private static long _循环时间;

        public static void 跳过循环获取金碎片(in ImageHandle 句柄)
        {
            if (获取碎片循环计数 == 0) 之前位置 = Control.MousePosition;

            _循环时间 = Common.获取当前时间毫秒();

            if (获取碎片循环计数 > 12)
            {
                获取碎片循环计数 = 0;
                SimKeyBoard.MouseMove(之前位置);
                之前位置 = new();
                TTS.TTS.Speak("12次退出");
                var 当前英雄 = "";
                Common.Main_Form?.Invoke(() =>
                {
                    当前英雄 = Common.Main_Form.tb_name.Text.Trim();
                });
                if (当前英雄 == "飞机")
                {
                    // 需要循环需加速buff
                    SimKeyBoard.MouseMove(1332, 92);
                    Common.Delay(50);
                    SimKeyBoard.MouseLeftClick();
                }
            }
            else
            {
                var 描述 = PaddleOCR.获取图片文字(GlobalScreenCapture.GetCurrentHandle(), new Rectangle(730, 503, 71, 39)).Trim();
                // Common.Main_Form.Invoke(() => { Common.Main_Form.tb_阵营.Text = 描述; });
                if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Silt.选择天赋, GlobalScreenCapture.GetCurrentHandle(), RPG选择技能范围)
                    && ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Silt.普通天赋, GlobalScreenCapture.GetCurrentHandle(), RPG第一技能金))
                {
                    跳过重新选择();
                }
                else if (描述 != "+45")
                {
                    跳过重新选择();
                }
                else
                {
                    获取碎片循环计数 = 0;
                }
            }

            static void 跳过重新选择()
            {
                SimKeyBoard.MouseMove(768, 674);
                Common.Delay(10);
                SimKeyBoard.MouseLeftClick();
                Common.Delay(10);
                SimKeyBoard.MouseLeftClick();

                获取碎片循环计数++;
                Task.Run(() =>
                {
                    Common.Delay(325, _循环时间);
                    // 自我调用
                    SimKeyBoard.KeyPress(Keys.NumPad1);
                }).ConfigureAwait(true);
            }
        }


        public static void 自动屏蔽3个选项(in ImageHandle 句柄)
        {
            if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Silt.选择天赋, GlobalScreenCapture.GetCurrentHandle(), RPG选择技能范围))
            {
                var p = Control.MousePosition;
                // 第三个技能右上角颜色金
                if (ColorExtensions.ColorAEqualColorB(GlobalScreenCapture.GetColor(1318, 296),
                    Color.FromArgb(188, 134, 1), 10))
                {
                    SimKeyBoard.MouseMove(690, 673);
                    Common.Delay(25);
                    SimKeyBoard.MouseLeftClick();
                    Common.Delay(25);
                    SimKeyBoard.MouseMove(953, 673);
                    Common.Delay(25);
                    SimKeyBoard.MouseLeftClick();
                    Common.Delay(25);
                    SimKeyBoard.MouseMove(1205, 673);
                    Common.Delay(25);
                    SimKeyBoard.MouseLeftClick();
                    Common.Delay(25);
                    SimKeyBoard.MouseMove(540, 146);
                    Common.Delay(25);
                    SimKeyBoard.MouseLeftClick();
                    Common.Delay(25);
                }
                SimKeyBoard.MouseMove(p);
            }
        }
        public static void 点击黑皇(in ImageHandle 句柄)
        {
            var p = Control.MousePosition;
            SimKeyBoard.MouseMove(40, 500);
            Common.Delay(25);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(25);
            SimKeyBoard.MouseMove(p);
        }

        public static void 点击暴击(in ImageHandle 句柄)
        {
            //var p = Control.MousePosition;
            //SimKeyBoard.MouseMove(40, 580);
            //Common.Delay(25);
            //SimKeyBoard.MouseLeftClick();
            //Common.Delay(25);
            //SimKeyBoard.MouseMove(p);
            // Example 1: Basic screen capture and image finding
            BasicImageFinding();

            // Example 2: Continuous screen monitoring
            ContinuousMonitoring();

            // Example 3: Performance testing
            PerformanceTest();
        }

        static void BasicImageFinding()
        {
            Console.WriteLine("=== Basic Image Finding Example ===");

            using (var processor = new PictureProcessing.RustImageProcessingSystem.ImageProcessor())
            {
                // Initialize with screen dimensions
                if (!processor.Initialize(1920, 1080))
                {
                    Console.WriteLine("Failed to initialize image processor");
                    return;
                }

                // Load template image
                var needleHandle = processor.CreateImage("template.png");

                try
                {
                    // Capture full screen
                    if (processor.CaptureScreen(0, 0, 1920, 1080))
                    {
                        // Find single occurrence
                        var point = processor.FindImage(needleHandle, matchRate: 0.95, tolerance: 5);
                        if (point.HasValue)
                        {
                            Console.WriteLine($"Found image at: ({point.Value.X}, {point.Value.Y})");

                            // Get pixel color at found location
                            var color = processor.GetPixel(point.Value.X, point.Value.Y);
                            Console.WriteLine($"Pixel color: R={color.R}, G={color.G}, B={color.B}, A={color.A}");
                        }
                        else
                        {
                            Console.WriteLine("Image not found");
                        }

                        // Find all occurrences
                        var allPoints = processor.FindAllImages(needleHandle, matchRate: 0.90, tolerance: 10, maxResults: 50);
                        Console.WriteLine($"Found {allPoints.Count} occurrences");

                        foreach (var p in allPoints)
                        {
                            Console.WriteLine($"  - At ({p.X}, {p.Y})");
                        }
                    }
                }
                finally
                {
                    processor.ReleaseImage(needleHandle);
                }

                // Show memory stats
                var stats = processor.GetMemoryStats();
                Console.WriteLine($"Memory - Total: {stats.totalSize / 1024 / 1024}MB, " +
                                $"Allocated: {stats.allocated / 1024 / 1024}MB, " +
                                $"Free: {stats.free / 1024 / 1024}MB");
            }
        }

        static void ContinuousMonitoring()
        {
            Console.WriteLine("\n=== Continuous Monitoring Example ===");

            using (var processor = new PictureProcessing.RustImageProcessingSystem.ImageProcessor())
            {
                processor.Initialize(1920, 1080);

                var targetImage = processor.CreateImage("target_button.png");
                var stopwatch = new Stopwatch();

                try
                {
                    Console.WriteLine("Monitoring for target image... Press any key to stop.");

                    while (!Console.KeyAvailable)
                    {
                        stopwatch.Restart();

                        // Capture specific region
                        processor.CaptureScreen(500, 300, 920, 480);

                        var found = processor.FindImage(targetImage, 0.95, 0);

                        stopwatch.Stop();

                        if (found.HasValue)
                        {
                            Console.WriteLine($"Target found at ({found.Value.X}, {found.Value.Y}) " +
                                            $"in {stopwatch.ElapsedMilliseconds}ms");

                            // Simulate click or other action
                            ClickAt(found.Value.X + 500, found.Value.Y + 300);

                            Thread.Sleep(1000); // Wait before next check
                        }
                        else
                        {
                            Thread.Sleep(100); // Small delay when not found
                        }
                    }

                    Console.ReadKey(true); // Clear the key press
                }
                finally
                {
                    processor.ReleaseImage(targetImage);
                }
            }
        }

        static void PerformanceTest()
        {
            Console.WriteLine("\n=== Performance Test ===");

            using (var processor = new PictureProcessing.RustImageProcessingSystem.ImageProcessor())
            {
                processor.Initialize(1920, 1080);

                var testImage = processor.CreateImage("test_pattern.png");
                var stopwatch = new Stopwatch();

                try
                {
                    // Test single image finding
                    const int iterations = 100;
                    double totalTime = 0;

                    for (int i = 0; i < iterations; i++)
                    {
                        processor.CaptureScreen(0, 0, 1920, 1080);

                        stopwatch.Restart();
                        var result = processor.FindImage(testImage, 0.95, 0);
                        stopwatch.Stop();

                        totalTime += stopwatch.ElapsedMilliseconds;
                    }

                    Console.WriteLine($"Average time for single find: {totalTime / iterations:F2}ms");

                    // Test multiple image finding
                    totalTime = 0;
                    for (int i = 0; i < iterations; i++)
                    {
                        processor.CaptureScreen(0, 0, 1920, 1080);

                        stopwatch.Restart();
                        var results = processor.FindAllImages(testImage, 0.90, 5, 100);
                        stopwatch.Stop();

                        totalTime += stopwatch.ElapsedMilliseconds;
                    }

                    Console.WriteLine($"Average time for find all: {totalTime / iterations:F2}ms");
                }
                finally
                {
                    processor.ReleaseImage(testImage);
                }
            }
        }

        // Helper method to simulate mouse click
        static void ClickAt(int x, int y)
        {
            // Implementation depends on your needs
            Console.WriteLine($"Simulating click at ({x}, {y})");
        }

        // Advanced usage with managed resources
        class AdvancedExample
        {
            public static void ManagedResourceExample()
            {
                using (var processor = new PictureProcessing.RustImageProcessingSystem.ImageProcessor())
                {
                    processor.Initialize(1920, 1080);

                    // Using ManagedImage for automatic cleanup
                    using (var managedImage = new ManagedImage(processor, processor.CreateImage("icon.png")))
                    {
                        processor.CaptureScreen(0, 0, 1920, 1080);
                        var result = processor.FindImage(managedImage.Handle, 0.95, 0);

                        if (result.HasValue)
                        {
                            Console.WriteLine($"Found at: {result.Value.X}, {result.Value.Y}");
                        }
                    } // Image automatically released here
                }
            }
        }

        public static void 测试识别(in ImageHandle 句柄)
        {
            var rustResults = GlobalScreenCapture.FindAllImages(Dota2_Pictrue.Silt.钢毛后背, 0.9, 100, 10);
            var string1 = "";
            foreach (var point in rustResults)
            {
                var rege文字 = new Rectangle(point.X - 66, point.Y + 114, 212, 42);
                var rege效果 = new Rectangle(point.X - 66, point.Y + 183, 212, 42);
                var b_pick = new Point(point.X + 41, point.Y + 306);
                var b_skip = new Point(point.X + 41, point.Y + 306);
                var 描述 = PaddleOCR.获取图片文字(in 句柄, rege文字);
                var 效果 = PaddleOCR.获取图片文字(in 句柄, rege效果);
                string1 += 描述 + "\r\n" + 效果;
                SimKeyBoard.MouseMove(b_pick);
                Common.Delay(500);
                SimKeyBoard.MouseMove(b_skip);
                Common.Delay(500);
            }
            Common.Main_Form.Invoke(() =>
            {
                Common.Main_Form.tb_阵营.Text = string1;
            });
            TTS.TTS.Speak("完成");
            // Tts.Speak(PaddleOCR.获取图片文字(in 句柄, new Rectangle(620, 437, 120, 40)));
        }

        public static void 沙王自动选择(in ImageHandle gameHandle)
        {
            var p = Control.MousePosition;
            TalentSelectionExamples.ExecuteHeroSelection("沙王", gameHandle);
            SimKeyBoard.MouseMove(p);
        }

        public static void 初始化天赋选择(in ImageHandle gameHandle)
        {
        }
    }
}




#endif
#endif