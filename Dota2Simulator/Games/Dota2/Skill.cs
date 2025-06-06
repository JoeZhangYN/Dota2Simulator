﻿// Games/Dota2/Skill.cs
//#define 输出技能信息
#if DOTA2

using Collections.Pooled;
using Dota2Simulator.ImageProcessingSystem;
using Dota2Simulator.KeyboardMouse;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games.Dota2
{
    internal class Skill
    {
        #region 模块化技能

        // 模块化之后，都不用注释也能看得懂了。。
        // 技能CD为x为指定 y为灰白色最上方像素 |涉及的颜色| 技能CD颜色实际上是释放技能时变换的颜色 所以容差大 456基本通用 4颜色少1rgb 未学习技能CD颜色 
        // 5 6 进入CD颜色渐变 122,129,136 94,100,106 16,19,18 8,11,10 非指向性施法颜色不变
        // 法球位置为灰白色左下角图标 |涉及的颜色| 法球颜色 未学习法球颜色
        // 推荐学习和状态技能取同一个位置 开启技能后 y最底下绿色像素 金黄色下往上数2  5,6技能 为暗黄色最底下 |涉及的颜色| 推荐学习颜色 状态激活颜色
        // QWERDF框x为左下角x向右偏移一个像素 y为特定的y 被动位置x为图标右侧边框左边第一个像素,有的技能亮色影响内侧颜色 y为qwerdf 的某个高度 |涉及的颜色| qwerdf颜色 被动颜色 未学习被动技能颜色 破坏被动颜色
        private static readonly 技能信息 技能4 = new(
            820, 998, 65
            , 27, -52, Color.FromArgb(194, 198, 202), 62, Color.FromArgb(72, 76, 80), 2
            , -1, -45, Color.FromArgb(34, 40, 39), 2
            , 2, -2, Color.FromArgb(54, 62, 70), 2, Color.FromArgb(25, 30, 34), 2
            , 25, -56, Color.FromArgb(0, 129, 0), 0, Color.FromArgb(211, 181, 79), 8
            , 50, -45, Color.FromArgb(49, 51, 47), 3, Color.FromArgb(33, 36, 37), 3
            , 25, -49
            , Color.FromArgb(67, 76, 84), 1 // 没用到
            , [Keys.Q, Keys.W, Keys.E, Keys.R]);

        private static readonly 技能信息 技能5 = new(
            802, 995, 58
            , 30, -49, Color.FromArgb(194, 198, 202), 62, Color.FromArgb(72, 76, 80), 2
            , 1, -41, Color.FromArgb(34, 40, 39), 1
            , 3, -3, Color.FromArgb(54, 61, 69), 2, Color.FromArgb(25, 29, 34), 2
            , 25, -50, Color.FromArgb(0, 129, 0), 0, Color.FromArgb(118, 100, 41), 8
            , 51, -41, Color.FromArgb(49, 51, 47), 3, Color.FromArgb(33, 36, 37), 3
            , 24, -47
            , Color.FromArgb(55, 62, 70), 1  // 没用到
            , [Keys.Q, Keys.W, Keys.E, Keys.D, Keys.R]);

        private static readonly 技能信息 技能6 = new(
            772, 995, 58
            , 30, -49, Color.FromArgb(194, 198, 202), 62, Color.FromArgb(72, 76, 80), 2
            , 1, -41, Color.FromArgb(34, 40, 39), 1
            , 3, -3, Color.FromArgb(54, 61, 69), 2, Color.FromArgb(25, 29, 34), 2
            , 25, -50, Color.FromArgb(0, 129, 0), 0, Color.FromArgb(118, 100, 41), 8
            , 51, -41, Color.FromArgb(49, 51, 47), 3, Color.FromArgb(33, 36, 37), 3
            , 24, -47
            , Color.FromArgb(55, 62, 70), 1  // 没用到
            , [Keys.Q, Keys.W, Keys.E, Keys.D, Keys.F, Keys.R]);

        private class 技能信息(
            int 左下角x,
            int 左下角y,
            int 技能间隔,
            int 技能CD位置偏移x,
            int 技能CD位置偏移y,
            Color 技能CD颜色,
            byte 技能CD颜色容差,
            Color 未学主动技能CD颜色,
            byte 未学主动技能CD颜色容差,
            int QWERDFx,
            int QWERDFy,
            Color QWERDF框颜色,
            byte QWERDF框颜色容差,
            int 法球技能CD位置变化x,
            int 法球技能CD位置变化y,
            Color 法球技能CD颜色,
            byte 法球技能颜色容差,
            Color 未学法球技能CD颜色,
            byte 未学法球技能CD颜色容差,
            int 状态技能位置变化x,
            int 状态技能位置变化y,
            Color 技能状态激活颜色,
            byte 技能状态激活颜色容差,
            Color 推荐学习技能颜色,
            byte 推荐学习技能颜色容差,
            int 被动位置变化x,
            int 被动位置变化y,
            Color 未学被动技能颜色,
            byte 未学被动技能颜色容差,
            Color 破坏被动技能颜色,
            byte 破坏被动技能颜色容差,
            int 释放位置偏移x,
            int 释放位置偏移y,
            Color 技能CD左下颜色,
            byte 技能CD左下颜色容差,
            Keys[] 技能位置)
        {
            public int 技能CD图标x { get; } = 左下角x + 技能CD位置偏移x;
            public int 技能CD图标y { get; } = 左下角y + 技能CD位置偏移y;
            public int 技能间隔 { get; } = 技能间隔;
            public int 释放变色位置x { get; } = 左下角x + 释放位置偏移x - 2; // 向左偏移2个单位，变化更明显
            public int 释放变色位置y { get; } = 左下角y + 释放位置偏移y;
            public int 法球技能CD位置x { get; } = 左下角x + 法球技能CD位置变化x;
            public int 法球技能CD位置y { get; } = 左下角y + 法球技能CD位置变化y;
            public int 状态技能位置x { get; } = 左下角x + 状态技能位置变化x;
            public int 状态技能位置y { get; } = 左下角y + 状态技能位置变化y;
            public int 被动位置x { get; } = 左下角x + 被动位置变化x;
            public int 被动位置y { get; } = 左下角y + 被动位置变化y;
            public int QWERDF位置x { get; } = 左下角x + QWERDFx;
            public int QWERDF位置y { get; } = 左下角y + QWERDFy;
            public Color 技能CD颜色 { get; } = 技能CD颜色;
            public byte 技能CD颜色容差 { get; } = 技能CD颜色容差;
            public Color 技能CD左下颜色 { get; } = 技能CD左下颜色;
            public byte 技能CD左下颜色容差 { get; } = 技能CD左下颜色容差;
            public Color 法球技能CD颜色 { get; } = 法球技能CD颜色;
            public byte 法球技能颜色容差 { get; } = 法球技能颜色容差;
            public Color 技能状态激活颜色 { get; } = 技能状态激活颜色;
            public byte 技能状态激活颜色容差 { get; } = 技能状态激活颜色容差;
            public Color QWERDF框颜色 { get; } = QWERDF框颜色;
            public byte QWERDF框颜色容差 { get; } = QWERDF框颜色容差;
            public Color 未学主动技能CD颜色 { get; } = 未学主动技能CD颜色;
            public byte 未学主动技能CD颜色容差 { get; } = 未学主动技能CD颜色容差;
            public Color 未学法球技能CD颜色 { get; } = 未学法球技能CD颜色;
            public byte 未学法球技能CD颜色容差 { get; } = 未学法球技能CD颜色容差;
            public Color 未学被动技能颜色 { get; } = 未学被动技能颜色;
            public byte 未学被动技能颜色容差 { get; } = 未学被动技能颜色容差;
            public Color 破坏被动技能颜色 { get; } = 破坏被动技能颜色;
            public byte 破坏被动技能颜色容差 { get; } = 破坏被动技能颜色容差;
            public Color 推荐学习技能颜色 { get; } = 推荐学习技能颜色;
            public byte 推荐学习技能颜色容差 { get; } = 推荐学习技能颜色容差;
            public Keys[] 技能位置 { get; } = 技能位置;
        }

        /// <summary>
        ///     技能类型枚举
        /// </summary>
        public enum 技能类型
        {
            图标CD,
            法球,
            状态,
            释放变色,
            QWERDF图标,
            被动技能存在,
            破坏被动技能,
            未学主动技能,
            未学法球技能,
            推荐学习技能
        }

        public static int _技能数量;

        #endregion

        #region 技能相关方法

        private static 技能信息 获取技能信息(int 技能数量 = 4)
        {
            return 技能数量 switch
            {
                4 => 技能4,
                5 => 技能5,
                6 => 技能6,
                _ => throw new ArgumentException("Invalid skill number")
            };
        }


        /// <summary>
        ///     获取技能的位置信息，包括坐标和颜色等。
        /// </summary>
        /// <param name="技能信息">技能信息</param>
        /// <param name="offsetX">X方向的偏移量</param>
        /// <param name="类型">技能类型</param>
        /// <returns>包含坐标和颜色等信息的元组</returns>
        private static (int x, int y, Color 颜色, byte 颜色容差) 获取技能位置信息(技能信息 技能信息, int offsetX, 技能类型 类型)
        {
            int x, y;
            Color 颜色;
            byte 颜色容差;

            switch (类型)
            {
                case 技能类型.释放变色:
                    x = 技能信息.释放变色位置x + offsetX - Main.坐标偏移x;
                    y = 技能信息.释放变色位置y - Main.坐标偏移y;
                    颜色 = default;
                    颜色容差 = 0;
                    break;
                case 技能类型.图标CD:
                    x = 技能信息.技能CD图标x + offsetX - Main.坐标偏移x;
                    y = 技能信息.技能CD图标y - Main.坐标偏移y;
                    颜色 = 技能信息.技能CD颜色;
                    颜色容差 = 技能信息.技能CD颜色容差;
                    break;
                case 技能类型.法球:
                    x = 技能信息.法球技能CD位置x + offsetX - Main.坐标偏移x;
                    y = 技能信息.法球技能CD位置y - Main.坐标偏移y;
                    颜色 = 技能信息.法球技能CD颜色;
                    颜色容差 = 技能信息.法球技能颜色容差;
                    break;
                case 技能类型.状态:
                    x = 技能信息.状态技能位置x + offsetX - Main.坐标偏移x;
                    y = 技能信息.状态技能位置y - Main.坐标偏移y;
                    颜色 = 技能信息.技能状态激活颜色;
                    颜色容差 = 技能信息.技能状态激活颜色容差;
                    break;
                case 技能类型.QWERDF图标:
                    x = 技能信息.QWERDF位置x + offsetX - Main.坐标偏移x;
                    y = 技能信息.QWERDF位置y - Main.坐标偏移y;
                    颜色 = 技能信息.QWERDF框颜色;
                    颜色容差 = 技能信息.QWERDF框颜色容差;
                    break;
                case 技能类型.被动技能存在:
                    x = 技能信息.被动位置x + offsetX - Main.坐标偏移x;
                    y = 技能信息.被动位置y - Main.坐标偏移y;
                    颜色 = 技能信息.未学被动技能颜色;
                    颜色容差 = 技能信息.未学被动技能颜色容差;
                    break;
                case 技能类型.破坏被动技能:
                    x = 技能信息.被动位置x + offsetX - Main.坐标偏移x;
                    y = 技能信息.被动位置y - Main.坐标偏移y;
                    颜色 = 技能信息.破坏被动技能颜色;
                    颜色容差 = 技能信息.破坏被动技能颜色容差;
                    break;
                case 技能类型.未学主动技能:
                    x = 技能信息.技能CD图标x + offsetX - Main.坐标偏移x;
                    y = 技能信息.技能CD图标y - Main.坐标偏移y;
                    颜色 = 技能信息.未学主动技能CD颜色;
                    颜色容差 = 技能信息.未学主动技能CD颜色容差;
                    break;
                case 技能类型.未学法球技能:
                    x = 技能信息.法球技能CD位置x + offsetX - Main.坐标偏移x;
                    y = 技能信息.法球技能CD位置y - Main.坐标偏移y;
                    颜色 = 技能信息.未学法球技能CD颜色;
                    颜色容差 = 技能信息.未学法球技能CD颜色容差;
                    break;
                case 技能类型.推荐学习技能:
                    x = 技能信息.状态技能位置x + offsetX - Main.坐标偏移x;
                    y = 技能信息.状态技能位置y - Main.坐标偏移y;
                    颜色 = 技能信息.推荐学习技能颜色;
                    颜色容差 = 技能信息.推荐学习技能颜色容差;
                    break;
                default:
                    throw new ArgumentException("未知的技能类型");
            }

            return (x, y, 颜色, 颜色容差);
        }

        private static int 获取技能位置偏移(Keys 技能位置, 技能信息 技能信息)
        {
            int index = Array.IndexOf(技能信息.技能位置, 技能位置);
            if (技能位置 == Keys.F && 技能信息 == 技能5)
            {
                index = 3; // 6技能魔晶A杖
            }

            return index == -1 ? throw new ArgumentException("Invalid skill position") : 技能信息.技能间隔 * index;
        }

        #region 获取当前技能数量
        /// <summary>
        ///     100次运行，6ms...
        ///     还要什么自行车，全部添加判断
        /// </summary>
        /// <param name="数组"></param>
        /// <returns></returns>
        public static async Task<bool> 设置当前技能数量()
        {
            int i = 获取当前技能数量(GlobalScreenCapture.GetCurrentHandle());
            if (i != 0)
            {
                // 技能数量改变后，技能判断色全部重置
                // 只有两种情况需要重置，一种是还没学，到学习了
                // 另一种是技能改变了
                if (_技能数量 != i)
                {
                    _技能数量 = i;

                    // 重置所有技能判断
                    重置所有技能判断();

                    // 获取当前技能颜色
                    _ = DOTA2获取所有释放技能前颜色(GlobalScreenCapture.GetCurrentHandle());
                }
            }

            return await Task.FromResult(i != 0).ConfigureAwait(true);
        }
        #endregion

        /// <summary>
        ///     <para>最后一个技能为被动时，出到A杖，光会紊乱颜色</para>
        ///     <para>最后一个不判断，判断前面几个，符合的话+1则为技能数量</para>
        /// </summary>
        /// <param name="数组"></param>
        /// <returns></returns>
        // 预先计算的检测点结构
        private readonly struct 检测点
        {
            public readonly Point 位置;
            public readonly ReadOnlyMemory<(Color 期望颜色, int 容差)> 颜色检查;
            public readonly string 名称;

            public 检测点(Point 位置, ReadOnlyMemory<(Color, int)> 颜色检查, string 名称)
            {
                this.位置 = 位置;
                this.颜色检查 = 颜色检查;
                this.名称 = 名称;
            }
        }

        // 使用缓存的检测配置
        private static readonly ConcurrentDictionary<(技能信息, int), 检测点[]> _检测点缓存 = new();

        // 静态StringBuilder池
        private static class StringBuilderPool_输出点信息
        {
            private static readonly ThreadLocal<StringBuilder> _pool = new ThreadLocal<StringBuilder>(() => new StringBuilder());

            public static StringBuilder Get()
            {
                var sb = _pool.Value;
                sb.Clear();
                return sb;
            }
        }

        private static class StringBuilderPool1
        {
            private static readonly ThreadLocal<StringBuilder> _pool = new(() => new StringBuilder());

            public static StringBuilder Get()
            {
                var sb = _pool.Value;
                sb.Clear();
                return sb;
            }
        }

        public static int 获取当前技能数量(in ImageHandle 句柄)
        {
            List<技能信息> 技能列表 = [技能4, 技能5, 技能6];
            List<int> 技能数量 = [4, 5, 6];

            var 全部文字 = StringBuilderPool_输出点信息.Get();

            for (int j = 0; j < 技能列表.Count; j++)
            {
                技能信息 当前技能 = 技能列表[j];
                int 期望数量 = 技能数量[j];

                var 输出文字 = StringBuilderPool1.Get();

                输出文字.AppendLine($"\r\n当前技能数量{期望数量}");

                var 检测到的数量 = 快速检测技能数量(in 句柄, 当前技能, 期望数量 - 1, 输出文字);

                if (检测到的数量 == 期望数量 - 1)
                {

#if 输出技能信息
                    Common.Main_Logger.Info(输出文字.ToString()); 
#endif

                    return 期望数量;
                }

                全部文字.Append(输出文字.ToString());

#if 输出技能信息
                // 结束循环依旧没匹配到
                if (j == 2) Common.Main_Logger.Error(全部文字.ToString()); 
#endif

            }

            TTS.TTS.Speak("技能数量异常");
            return 0;
        }

        private static int 快速检测技能数量(in ImageHandle 句柄, 技能信息 技能, int 最大检测数量, StringBuilder 调试信息)
        {
            int count_技能 = 0;

            for (int i = 0; i < 最大检测数量; i++)
            {
                var (成功, 单个调试信息) = 快速检测单个技能(in 句柄, 技能, i);

                调试信息.Append(单个调试信息);


                if (成功)
                {
                    count_技能++;
                }
            }

            return count_技能;
        }

        private static (bool 成功, string 调试信息) 快速检测单个技能(in ImageHandle 句柄, 技能信息 技能, int i)
        {
            var 检测点数组 = 获取检测点配置(技能, i);
            var 调试信息 = new StringBuilder();

            // 获取所有颜色和检测结果
            var 检测结果 = new (string 名称, Point 位置, Color 颜色, bool[] 匹配结果)[检测点数组.Length];
            bool 整体有匹配 = false;

            for (int idx = 0; idx < 检测点数组.Length; idx++)
            {
                var 点 = 检测点数组[idx];
                var 实际颜色 = ImageManager.GetColor(in 句柄, 点.位置);
                var 匹配结果 = new bool[点.颜色检查.Length];

                for (int colorIdx = 0; colorIdx < 点.颜色检查.Length; colorIdx++)
                {
                    var (期望颜色, 容差) = 点.颜色检查.Span[colorIdx];
                    匹配结果[colorIdx] = ColorExtensions.ColorAEqualColorB(实际颜色, 期望颜色, (byte)容差);
                }

                bool 当前点有匹配 = 匹配结果.Any(match => match);
                检测结果[idx] = (点.名称, 点.位置, 实际颜色, 匹配结果);
                整体有匹配 |= 当前点有匹配;
            }

            // 根据原逻辑构建调试信息
            if (整体有匹配)
            {
                // 如果有匹配，输出所有检测点的信息
                调试信息.AppendLine($"{i + 1} QWERDF图标 :位置X:{检测结果[0].位置.X + Main.坐标偏移x},位置Y:{检测结果[0].位置.Y + Main.坐标偏移y}，RGB:{检测结果[0].颜色.R}, {检测结果[0].颜色.G}, {检测结果[0].颜色.B}");
                调试信息.AppendLine($"{i + 1} 技能CD图标 :位置X:{检测结果[1].位置.X + Main.坐标偏移x},位置Y:{检测结果[1].位置.Y + Main.坐标偏移y}，RGB:{检测结果[1].颜色.R}, {检测结果[1].颜色.G}, {检测结果[1].颜色.B}。");
                调试信息.AppendLine($"{i + 1} 技能法球 :位置X:{检测结果[2].位置.X + Main.坐标偏移x},位置Y:{检测结果[2].位置.Y + Main.坐标偏移y}，RGB:{检测结果[2].颜色.R}, {检测结果[2].颜色.G}, {检测结果[2].颜色.B}。");
                调试信息.AppendLine($"{i + 1} 被动技能 :位置X:{检测结果[3].位置.X + Main.坐标偏移x},位置Y:{检测结果[3].位置.Y + Main.坐标偏移y}，RGB:{检测结果[3].颜色.R}, {检测结果[3].颜色.G}, {检测结果[3].颜色.B}。");
                调试信息.AppendLine($"{i + 1} 推荐技能 :位置X:{检测结果[4].位置.X + Main.坐标偏移x},位置Y:{检测结果[4].位置.Y + Main.坐标偏移y}，RGB:{检测结果[4].颜色.R}, {检测结果[4].颜色.G}, {检测结果[4].颜色.B}。");
                调试信息.AppendLine();
                return (true, 调试信息.ToString());
            }

            // 如果没有匹配，按原逻辑输出不匹配的项
            // QWERDF图标
            if (!检测结果[0].匹配结果.Any(match => match))
            {
                调试信息.AppendLine($"{i + 1} QWERDF图标 :位置X:{检测结果[0].位置.X + Main.坐标偏移x},位置Y:{检测结果[0].位置.Y + Main.坐标偏移y}，RGB:{检测结果[0].颜色.R}, {检测结果[0].颜色.G}, {检测结果[0].颜色.B}");
            }
            if (!检测结果[1].匹配结果.Any(match => match)) // 主动技能
            {
                调试信息.AppendLine($"{i + 1} 技能CD图标 :位置X:{检测结果[1].位置.X + Main.坐标偏移x},位置Y:{检测结果[1].位置.Y + Main.坐标偏移y}，RGB:{检测结果[1].颜色.R}, {检测结果[1].颜色.G}, {检测结果[1].颜色.B}。");
            }
            if (!检测结果[2].匹配结果.Any(match => match)) // 法球技能
            {
                调试信息.AppendLine($"{i + 1} 技能法球 :位置X:{检测结果[2].位置.X + Main.坐标偏移x},位置Y:{检测结果[2].位置.Y + Main.坐标偏移y}，RGB:{检测结果[2].颜色.R}, {检测结果[2].颜色.G}, {检测结果[2].颜色.B}。");
            }
            if (!检测结果[3].匹配结果.Any(match => match)) // 被动技能
            {
                调试信息.AppendLine($"{i + 1} 被动技能 :位置X:{检测结果[3].位置.X + Main.坐标偏移x},位置Y:{检测结果[3].位置.Y + Main.坐标偏移y}，RGB:{检测结果[3].颜色.R}, {检测结果[3].颜色.G}, {检测结果[3].颜色.B}。");
            }
            if (!检测结果[4].匹配结果.Any(match => match)) // 推荐技能
            {
                调试信息.AppendLine($"{i + 1} 推荐技能 :位置X:{检测结果[4].位置.X + Main.坐标偏移x},位置Y:{检测结果[4].位置.Y + Main.坐标偏移y}，RGB:{检测结果[4].颜色.R}, {检测结果[4].颜色.G}, {检测结果[4].颜色.B}。");
                调试信息.AppendLine();
            }

            return (false, 调试信息.ToString());
        }

        private static 检测点[] 获取检测点配置(技能信息 技能, int i)
        {
            return _检测点缓存.GetOrAdd((技能, i), key => 创建检测点配置_缓存(key.Item1, key.Item2));
        }

        private static 检测点[] 创建检测点配置_缓存(技能信息 技能, int i)
        {
            int 偏移 = 技能.技能间隔 * i;

            Point p_QWERDF = new(技能.QWERDF位置x + 偏移 - Main.坐标偏移x, 技能.QWERDF位置y - Main.坐标偏移y);
            Point p_主动 = new(技能.技能CD图标x + 偏移 - Main.坐标偏移x, 技能.技能CD图标y - Main.坐标偏移y);
            Point p_法球 = new(技能.法球技能CD位置x + 偏移 - Main.坐标偏移x, 技能.法球技能CD位置y - Main.坐标偏移y);
            Point p_被动 = new(技能.被动位置x + 偏移 - Main.坐标偏移x, 技能.被动位置y - Main.坐标偏移y);
            Point p_推荐 = new(技能.状态技能位置x + 偏移 - Main.坐标偏移x, 技能.状态技能位置y - Main.坐标偏移y);

            return
            [
        // QWERDF图标检测点
        new(p_QWERDF, new (Color, int)[]
        {
            (技能.QWERDF框颜色, 技能.QWERDF框颜色容差)
        }, "QWERDF图标"),
        
        // 主动技能检测点
        new(p_主动, new (Color, int)[]
        {
            (技能.技能CD颜色, 技能.技能CD颜色容差),
            (技能.未学主动技能CD颜色, 技能.未学主动技能CD颜色容差)
        }, "主动"),
        
        // 法球技能检测点
        new(p_法球, new (Color, int)[]
        {
            (技能.未学法球技能CD颜色, 技能.未学法球技能CD颜色容差),
            (技能.未学法球技能CD颜色, 技能.未学法球技能CD颜色容差) // 注意：原代码中已学和未学法球用的是同一个颜色和容差
        }, "技能"),
        
        // 被动技能检测点
        new(p_被动, new (Color, int)[]
        {
            (技能.未学被动技能颜色, 技能.未学被动技能颜色容差),
            (技能.破坏被动技能颜色, 技能.破坏被动技能颜色容差)
        }, "被动技能"),
        
        // 推荐技能检测点
        new(p_推荐, new (Color, int)[]
        {
            (技能.推荐学习技能颜色, 技能.推荐学习技能颜色容差)
        }, "推荐技能")
            ];
        }

        /// <summary>
        ///     判断技能状态根据
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <param name="类型">技能类型</param>
        /// <returns>如果技能在CD状态返回真，否则返回假</returns>
        public static bool 判断技能状态(Keys 技能位置, in ImageHandle 句柄, 技能类型 类型 = 技能类型.图标CD)
        {
            if (_技能数量 == 4 && (技能位置 == Keys.D || 技能位置 == Keys.F))
            {
                return false;
            }

            技能信息 技能信息 = 获取技能信息(_技能数量);
            int offsetX = 获取技能位置偏移(技能位置, 技能信息);
            (int x, int y, Color 颜色, byte 颜色容差) = 获取技能位置信息(技能信息, offsetX, 类型);
            return ColorExtensions.ColorAEqualColorB(颜色, ImageManager.GetColor(in 句柄, x, y), 颜色容差);
        }

        /// <summary>
        ///     获取指定技能位置的像素颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <param name="类型">技能类型</param>
        /// <returns>指定位置的像素颜色</returns>
        private static Color 获取技能判断颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4, 技能类型 类型 = 技能类型.释放变色)
        {
            if (技能数量 == 4 && (技能位置 == Keys.D || 技能位置 == Keys.F))
            {
                return Color.Empty;
            }

            技能信息 技能信息 = 获取技能信息(技能数量);
            int offsetX = 获取技能位置偏移(技能位置, 技能信息);
            (int x, int y, Color _, byte _) = 获取技能位置信息(技能信息, offsetX, 类型);

            return ImageManager.GetColor(in 句柄, x, y);
        }

        /// <summary>
        ///     获取用于判断技能释放的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>技能释放判断的像素颜色</returns>
        public static Color 获取技能释放判断颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 句柄, 技能数量, 技能类型.释放变色);
        }

        /// <summary>
        ///     获取用于判断技能进入CD的像素的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>的像素颜色</returns>
        public static Color 获取技能进入CD判断颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 句柄, 技能数量, 技能类型.图标CD);
        }

        /// <summary>
        ///     获取qwerdf的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>技能释放判断的像素颜色</returns>
        public static Color 获取QWERDF颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 句柄, 技能数量, 技能类型.QWERDF图标);
        }

        /// <summary>
        ///     获取法球的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>技能释放判断的像素颜色</returns>
        public static Color 获取法球颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 句柄, 技能数量, 技能类型.法球);
        }

        /// <summary>
        ///     获取用于判断技能释放学习的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>技能释放判断的像素颜色</returns>
        public static Color 获取状态颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 句柄, 技能数量, 技能类型.状态);
        }

        /// <summary>
        ///     获取被动的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>技能释放判断的像素颜色</returns>
        public static Color 获取被动颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 句柄, 技能数量, 技能类型.被动技能存在);
        }

        /// <summary>
        ///     判断右上角技能是否CD，CD好了返回真，没好返回假
        ///     <para>有延时的话建议传入_全局图像句柄</para>
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量</param>
        /// <returns>如果技能CD好了返回真，否则返回假</returns>
        public static bool DOTA2判断技能是否CD(Keys 技能位置, in ImageHandle 句柄)
        {
            return 判断技能状态(技能位置, in 句柄, 技能类型.图标CD);
        }

        public static bool DOTA2释放CD就绪技能(Keys 技能位置, in ImageHandle 句柄)
        {
            if (判断技能状态(技能位置, in 句柄, 技能类型.图标CD))
            {
                SimKeyBoard.KeyPress(技能位置);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     判断法球技能是否CD，CD好了返回真，没好返回假
        ///     <para>有延时的话建议传入_全局图像句柄</para>
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量</param>
        /// <returns>如果法球技能CD好了返回真，否则返回假</returns>
        private static bool DOTA2判断法球技能是否CD(Keys 技能位置, in ImageHandle 句柄)
        {
            return 判断技能状态(技能位置, in 句柄, 技能类型.法球);
        }

        /// <summary>
        ///     判断状态技能是否启动，启动返回真，未启动返回假
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <returns>如果状态技能未启动返回真，否则返回假</returns>
        public static bool DOTA2判断状态技能是否启动(Keys 技能位置, in ImageHandle 句柄)
        {
            return 判断技能状态(技能位置, in 句柄, 技能类型.状态);
        }

        public static bool DOTA2判断是否持续施法(in ImageHandle 句柄)
        {
            // 通过添加步骤来等待完全显示
            // 用于检测持续施法，施法中文字的施字颜色，10秒以内有效
            return ColorExtensions.ColorAEqualColorB(Main.获取指定位置颜色(953, 764, in 句柄), Color.FromArgb(254, 254, 254), 2);
        }

        private static void 记录技能释放信息(Keys s1, string s, bool b1, bool b2, Color c1, Color c2, Color c3)
        {
            Common.Main_Logger.Info($"\r\n{s1}{s}\r\n"
                                + $"判断是否释放{b1}\r\n"
                                + $"释放后变色{b2}\r\n"
                                + $"释放前{c1}\r\n"
                                + $"释放后{c2}\r\n"
                                + $"对比色{c3}\r\n");
        }

        private static void 记录技能释放信息(Keys s1, Keys 技能位置)
        {
            Common.Main_Logger.Info(s1);

            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 获取技能状态))
                {
                    bool 释放中判断 = 获取技能状态.释放中判断;
                    bool 已释放变色判断 = 获取技能状态.已释放变色判断;
                    Color 释放前Color = 获取技能状态.释放前Color;
                    Color 释放后Color = 获取技能状态.释放后Color;
                    Common.Main_Logger.Info($"\r\n获取到{s1}的技能状态\r\n"
                                + $"判断是否释放{释放中判断}\r\n"
                                + $"释放后变色{已释放变色判断}\r\n"
                                + $"释放前{释放前Color}\r\n"
                                + $"释放后{释放后Color}\r\n");
                }
                else
                {
                    Common.Main_Logger.Info($"获取不到{技能位置}的技能状态");
                }
            }
        }

        /// <summary>
        ///     获取CD好了的已学习技能释放前颜色
        /// </summary>
        /// <param name="技能位置"></param>
        /// <param name="数组"></param>
        /// <param name="技能数量"></param>
        /// <returns></returns>
        public static bool DOTA2获取单个释放技能前颜色(Keys 技能位置, in ImageHandle 句柄)
        {
            重置指定技能判断(技能位置);

            // 获取当前技能颜色
            Color 获取当前颜色 = 获取技能释放判断颜色(技能位置, in 句柄, _技能数量);
            if (判断是否更新释放技能前颜色(技能位置, 获取技能信息(_技能数量), in 句柄))
            {
                更新释放前颜色(技能位置, 获取当前颜色);
            }

            return true;
        }

        /// <summary>
        ///     10000次 1-2ms
        /// </summary>
        /// <param name="数组"></param>
        /// <returns></returns>
        public static bool DOTA2获取所有释放技能前颜色(in ImageHandle 句柄)
        {
            static void 更新指定释放色(Keys 技能位置, in ImageHandle 句柄)
            {
                更新释放前颜色(技能位置, 获取技能释放判断颜色(技能位置, in 句柄, _技能数量));
            }

            技能信息 技能信息 = 获取技能信息(_技能数量);

            foreach (Keys 位置 in 技能信息.技能位置)
            {
                if (判断是否更新释放技能前颜色(位置, 技能信息, in 句柄))
                {
                    更新指定释放色(位置, in 句柄);
                }
            }

            return true;
        }

        /// <summary>
        ///     当且仅当该技能为可释放已学习主动时返回真
        /// </summary>
        /// <param name="技能位置"></param>
        /// <param name="技能信息"></param>
        /// <param name="数组"></param>
        /// <returns></returns>
        private static bool 判断是否更新释放技能前颜色(Keys 技能位置, 技能信息 技能信息, in ImageHandle 句柄)
        {
            int 偏移 = 获取技能位置偏移(技能位置, 技能信息);

            Point p_主动 = new(技能信息.技能CD图标x + 偏移 - Main.坐标偏移x, 技能信息.技能CD图标y - Main.坐标偏移y);

            Color 获取的颜色_主动 = ImageManager.GetColor(in 句柄, p_主动);

            // 主动释放CD技能
            bool colorMatch_已学主动 = ColorExtensions.ColorAEqualColorB(获取的颜色_主动, 技能信息.技能CD颜色, 技能信息.技能CD颜色容差);

            return colorMatch_已学主动;
        }

        private static void 更新释放前颜色(Keys 技能位置, Color 当前获取颜色)
        {
            // 释放判断为真时，技能正在释放或者释放完毕，
            // ，前未变色，释放完毕后 释放判断是假，
            // 还有一种情况是船长这类，释放替换型的
            // 
            // 颜色为空，一开始为空，F1更新技能数量后为空

            // CD真：释放前中（充能无效） 排除
            // 就算是充能的主动技能，只要没有学习，还是不管（基本上完美了）

            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 获取技能状态))
                {
                    bool 释放中判断 = 获取技能状态.释放中判断;
                    Color 释放前Color = 获取技能状态.释放前Color;
                    // 如果释放中判断为假并且释放前颜色为空，则更新状态
                    if (!释放中判断 && 释放前Color == Color.Empty)
                    {
                        获取技能状态.释放中判断 = false;
                        获取技能状态.已释放变色判断 = false;
                        获取技能状态.释放前Color = 当前获取颜色;
                        获取技能状态.释放后Color = Color.Empty;
                        // 记录技能释放信息($"更新释放前颜色", false, false, 当前获取颜色, Color.Empty, 当前获取颜色);
                    }
                }
            }
        }

        private static bool DOTA2对比释放技能前后颜色(Keys 技能位置, in ImageHandle 句柄)
        {
            // 指向性技能CD栏基本全白
            Color 技能CD颜色 = 获取技能进入CD判断颜色(技能位置, in 句柄, _技能数量);
            if (!ColorExtensions.ColorAEqualColorB(技能CD颜色, Color.FromArgb(255, 255, 255), 10))
            {
                // 获取当前技能颜色
                Color 当前释放颜色 = 获取技能释放判断颜色(技能位置, in 句柄, _技能数量);

                return 处理技能释放(技能位置, 当前释放颜色);
            }
            return true;
        }

        private static readonly Dictionary<Keys, object> 锁字典 = new()
        {
            { Keys.Q, new object() },
            { Keys.W, new object() },
            { Keys.E, new object() },
            { Keys.R, new object() },
            { Keys.D, new object() },
            { Keys.F, new object() }
        };

        private static readonly Dictionary<Keys, 技能状态> 技能状态字典 = new()
        {
            { Keys.Q, new 技能状态() },
            { Keys.W, new 技能状态() },
            { Keys.E, new 技能状态() },
            { Keys.R, new 技能状态() },
            { Keys.D, new 技能状态() },
            { Keys.F, new 技能状态() }
        };

        private class 技能状态
        {
            public bool 释放中判断 { get; set; }
            public bool 已释放变色判断 { get; set; }
            public Color 释放前Color { get; set; } = Color.Empty;
            public Color 释放后Color { get; set; } = Color.Empty;
        }

        private static bool 获取技能释放状态(Keys 技能位置, out bool 释放中判断, out bool 已释放变色判断, out Color 释放前Color, out Color 释放后Color)
        {
            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 技能状态))
                {
                    释放中判断 = 技能状态.释放中判断;
                    已释放变色判断 = 技能状态.已释放变色判断;
                    释放前Color = 技能状态.释放前Color;
                    释放后Color = 技能状态.释放后Color;
                    return true;
                }

                释放中判断 = false;
                已释放变色判断 = false;
                释放前Color = Color.Empty;
                释放后Color = Color.Empty;
                return false;
            }
        }

        private static void 更新释放判断和颜色(Keys 技能位置, bool 释放中判断, bool 已释放变色判断, Color 释放前Color, Color 释放后Color)
        {
            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 技能状态))
                {
                    技能状态.释放中判断 = 释放中判断;
                    技能状态.已释放变色判断 = 已释放变色判断;
                    技能状态.释放前Color = 释放前Color;
                    技能状态.释放后Color = 释放后Color;
                }
            }
        }
        private static void 重置指定技能判断(Keys key)
        {
            if (技能状态字典.TryGetValue(key, out 技能状态 技能状态))
            {
                技能状态.释放中判断 = false;
                技能状态.已释放变色判断 = false;
                技能状态.释放前Color = Color.Empty;
                技能状态.释放后Color = Color.Empty;
            }
        }

        public static void 重置所有技能判断()
        {
            foreach (Keys key in 技能状态字典.Keys)
            {
                技能状态 技能状态 = 技能状态字典[key];
                技能状态.释放中判断 = false;
                技能状态.已释放变色判断 = false;
                技能状态.释放前Color = Color.Empty;
                技能状态.释放后Color = Color.Empty;
            }
        }
        // 存储每个技能的释放开始时间
        private static readonly Dictionary<Keys, DateTime> _技能释放开始时间 = new Dictionary<Keys, DateTime>();
        private static readonly Lock _时间锁 = new();

        /// <summary>
        /// 获取或设置技能释放开始时间
        /// </summary>
        /// <param name="技能位置">技能按键位置</param>
        /// <param name="当前释放中">当前是否处于释放中状态</param>
        /// <returns>释放开始时间</returns>
        private static DateTime 获取或设置释放开始时间(Keys 技能位置, bool 当前释放中)
        {
            lock (_时间锁)
            {
                if (_技能释放开始时间.TryGetValue(技能位置, out DateTime value))
                {
                    // 如果已存在记录，直接返回
                    return value;
                }
                else
                {
                    // 如果不存在记录，说明是新的释放周期，设置当前时间
                    DateTime 开始时间 = DateTime.Now;
                    _技能释放开始时间[技能位置] = 开始时间;
                    //Common.Main_Logger.Debug($"技能{技能位置}记录释放开始时间: {开始时间:HH:mm:ss.fff}");
                    return 开始时间;
                }
            }
        }

        /// <summary>
        /// 设置技能释放开始时间
        /// </summary>
        /// <param name="技能位置">技能按键位置</param>
        /// <param name="开始时间">释放开始时间</param>
        private static void 设置释放开始时间(Keys 技能位置, DateTime 开始时间)
        {
            lock (_时间锁)
            {
                _技能释放开始时间[技能位置] = 开始时间;
                //Common.Main_Logger.Debug($"技能{技能位置}设置释放开始时间: {开始时间:HH:mm:ss.fff}");
            }
        }

        /// <summary>
        /// 清除技能释放开始时间记录（在释放结束时调用）
        /// </summary>
        /// <param name="技能位置">技能按键位置</param>
        private static void 清除释放开始时间(Keys 技能位置)
        {
            lock (_时间锁)
            {
                if (_技能释放开始时间.ContainsKey(技能位置))
                {
                    _技能释放开始时间.Remove(技能位置);
                    //Common.Main_Logger.Debug($"技能{技能位置}清除释放开始时间记录");
                }
            }
        }

        /// <summary>
        /// 获取技能释放持续时间（毫秒）
        /// </summary>
        /// <param name="技能位置">技能按键位置</param>
        /// <returns>持续时间（毫秒），如果未找到记录返回0</returns>
        private static double 获取释放持续时间(Keys 技能位置)
        {
            lock (_时间锁)
            {
                if (_技能释放开始时间.TryGetValue(技能位置, out DateTime value))
                {
                    return (DateTime.Now - value).TotalMilliseconds;
                }
                return 0;
            }
        }

        private static bool 处理技能释放(Keys 技能位置, Color 当前释放颜色)
        {
            if (!获取技能释放状态(技能位置, out bool 释放中判断, out bool 已释放变色判断, out Color 释放前Color, out Color 释放后Color))
            {
                Common.Main_Logger.Info("字典读取失败");
                return false;
            }

            const int COLOR_TOLERANCE = 8;
            const int MIN_RELEASE_DURATION_MS = 100; // 最短释放时间

            // 获取释放开始时间
            DateTime 释放开始时间 = 获取或设置释放开始时间(技能位置, 释放中判断);

            //Common.Main_Logger.Debug($"技能{技能位置}: 当前RGB({当前释放颜色.R},{当前释放颜色.G},{当前释放颜色.B}) " +
            //            $"释放前RGB({释放前Color.R},{释放前Color.G},{释放前Color.B}) " +
            //            $"释放后RGB({释放后Color.R},{释放后Color.G},{释放后Color.B}) " +
            //            $"状态: 释放中={释放中判断}, 已释放={已释放变色判断}");

            if (释放中判断)
            {
                if (!已释放变色判断)
                {
                    double 与释放前距离 = CalculateColorDistance(当前释放颜色, 释放前Color);
                    double 与释放后距离 = CalculateColorDistance(当前释放颜色, 释放后Color);
                    double 释放已持续时间 = (DateTime.Now - 释放开始时间).TotalMilliseconds;

                    //Common.Main_Logger.Debug($"技能{技能位置}释放中: 与释放前距离={与释放前距离:F2}, 与释放后距离={与释放后距离:F2}, 持续时间={释放已持续时间:F0}ms");

                    // 智能判断：优先考虑距离比较，然后考虑时间
                    if (与释放前距离 <= COLOR_TOLERANCE)
                    {
                        // 回到释放前颜色
                        if (释放已持续时间 >= MIN_RELEASE_DURATION_MS)
                        {
                            更新释放判断和颜色(技能位置, false, true, 释放前Color, 释放后Color);
                            //Common.Main_Logger.Info($"技能{技能位置}释放完毕 - 颜色回到初始状态 (距离={与释放前距离:F2}, 持续={释放已持续时间:F0}ms)");
                            return false;
                        }
                        else
                        {
                            //Common.Main_Logger.Debug($"技能{技能位置}颜色回到初始，但时间太短，继续等待");
                            return true;
                        }
                    }
                    else if (与释放后距离 <= Math.Max(20, 与释放前距离 * 0.7)) // 动态阈值：更接近释放后颜色
                    {
                        //Common.Main_Logger.Debug($"技能{技能位置}仍接近释放后颜色，继续释放状态");
                        return true;
                    }
                    else if (IsColorInTransition(当前释放颜色, 释放前Color, 释放后Color, 与释放前距离, 与释放后距离))
                    {
                        //Common.Main_Logger.Debug($"技能{技能位置}检测到渐变色，继续释放状态");
                        return true;
                    }
                    else
                    {
                        // 智能判断是否应该结束
                        bool 应该结束释放 = ShouldEndRelease(与释放前距离, 与释放后距离, 释放已持续时间);

                        if (应该结束释放)
                        {
                            更新释放判断和颜色(技能位置, false, true, 释放前Color, 释放后Color);
                            //Common.Main_Logger.Info($"技能{技能位置}释放结束 - 智能判断 (前距离={与释放前距离:F2}, 后距离={与释放后距离:F2}, 持续={释放已持续时间:F0}ms)");
                            return false;
                        }
                        else
                        {
                            //Common.Main_Logger.Debug($"技能{技能位置}颜色异常但继续等待");
                            return true;
                        }
                    }
                }
                else
                {
                    // 已释放完毕状态处理...
                    if (ColorExtensions.ColorAEqualColorB(释放前Color, 当前释放颜色, COLOR_TOLERANCE))
                    {
                        更新释放判断和颜色(技能位置, false, false, 释放前Color, 释放后Color);
                        //Common.Main_Logger.Info($"技能{技能位置}重置为可用状态");
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                // 未释放状态逻辑...
                if (已释放变色判断)
                {
                    if (ColorExtensions.ColorAEqualColorB(释放前Color, 当前释放颜色, COLOR_TOLERANCE))
                    {
                        更新释放判断和颜色(技能位置, false, false, 释放前Color, 释放后Color);
                        //Common.Main_Logger.Info($"技能{技能位置}从已释放状态重置为可用");
                        return true;
                    }
                    return false;
                }
                else
                {
                    if (ColorExtensions.ColorAEqualColorB(释放前Color, 当前释放颜色, COLOR_TOLERANCE))
                    {
                        return true;
                    }
                    else
                    {
                        // 统一的释放检测逻辑
                        bool 开始释放 = IsSkillReleasing(当前释放颜色, 释放前Color, 释放后Color);

                        if (开始释放)
                        {
                            更新释放判断和颜色(技能位置, true, false, 释放前Color, 当前释放颜色);
                            设置释放开始时间(技能位置, DateTime.Now);
                            //Common.Main_Logger.Info($"技能{技能位置}开始释放 - 统一检测逻辑");
                        }

                        return true;
                    }
                }
            }
        }

        // 智能判断是否应该结束释放
        private static bool ShouldEndRelease(double 前距离, double 后距离, double 持续时间)
        {
            //// 如果持续时间太短，不要轻易结束
            //if (持续时间 < MIN_RELEASE_DURATION_MS)
            //    return false;

            // 如果两个距离都很大，且持续时间够长，可以结束
            if (前距离 > 50 && 后距离 > 50 && 持续时间 > 500)
                return true;

            // 如果距离差异很大（明显偏离两种预期颜色），结束
            if (Math.Min(前距离, 后距离) > 100)
                return true;

            return false;
        }

        // 改进的渐变色检测
        private static bool IsColorInTransition(Color current, Color start, Color end, double 前距离, double 后距离)
        {
            // 如果当前颜色在起始和结束颜色的"中间位置"
            double 总距离 = CalculateColorDistance(start, end);
            double 距离比例 = Math.Min(前距离, 后距离) / 总距离;

            return 距离比例 < 0.8; // 在合理范围内
        }

        // 统一的释放检测逻辑
        private static bool IsSkillReleasing(Color current, Color before, Color after)
        {
            // 方法1：直接匹配释放后颜色
            if (after != Color.Empty && ColorExtensions.ColorAEqualColorB(after, current, 15))
            {
                return true;
            }

            // 方法2：使用颜色变换公式
            if (DOTA2释放颜色前后对比(before, current))
            {
                return true;
            }

            // 方法3：颜色距离判断（作为补充）
            double 与前距离 = CalculateColorDistance(current, before);
            double 与后距离 = CalculateColorDistance(current, after);

            // 如果明显更接近释放后颜色
            return 与后距离 < 与前距离 * 0.6 && 与后距离 < 30;
        }

        // 辅助方法：计算颜色距离
        private static double CalculateColorDistance(Color c1, Color c2)
        {
            return Math.Sqrt(Math.Pow(c1.R - c2.R, 2) + Math.Pow(c1.G - c2.G, 2) + Math.Pow(c1.B - c2.B, 2));
        }

        // 辅助方法：检查颜色是否在渐变过程中
        private static bool IsColorInTransition(Color current, Color start, Color end)
        {
            // 检查当前颜色是否在起始色和结束色之间的合理范围内
            bool rInRange = (current.R >= Math.Min(start.R, end.R) - 10) && (current.R <= Math.Max(start.R, end.R) + 10);
            bool gInRange = (current.G >= Math.Min(start.G, end.G) - 10) && (current.G <= Math.Max(start.G, end.G) + 10);
            bool bInRange = (current.B >= Math.Min(start.B, end.B) - 10) && (current.B <= Math.Max(start.B, end.B) + 10);

            return rInRange && gInRange && bInRange;
        }

        /// <summary>
        /// 高性能颜色匹配器 - 使用SIMD和Span优化
        /// </summary>
        public static class ColorMatcher
        {
            // 预计算的颜色转换表
            private static readonly byte[] RSquaredLookup = new byte[256];
            private static readonly byte[] GSquaredLookup = new byte[256];
            private static readonly byte[] BSquaredLookup = new byte[256];

            // 空间换时间 预先计算所有结果,到时候直接匹配
            static ColorMatcher()
            {
                // 预计算查找表
                for (int i = 0; i < 256; i++)
                {
                    RSquaredLookup[i] = (byte)(i * i * 0.0001 + 0.7629 * i);
                    GSquaredLookup[i] = (byte)(i * i * 0.0014 + 0.0219 * i + 147);
                    BSquaredLookup[i] = (byte)(i * i * 0.0002 + 0.7586 * i);
                }
            }

            /// <summary>
            /// 批量颜色匹配 - 使用SIMD加速
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool BatchColorMatch(ReadOnlySpan<Color> colors, ReadOnlySpan<Color> targets, byte tolerance)
            {
                if (colors.Length != targets.Length)
                    return false;

                //// 使用SIMD进行批量比较
                //if (Avx2.IsSupported && colors.Length >= 8)
                //{
                //    return BatchColorMatchAvx2(colors, targets, tolerance);
                //}

                // 回退到标量实现
                for (int i = 0; i < colors.Length; i++)
                {
                    if (!ColorExtensions.ColorAEqualColorB(colors[i], targets[i], tolerance))
                        return false;
                }

                return true;
            }

            ///     变色运算匹配，符合的返回真，不符合返回假  未使用
            ///     <para>10000次 1ms..</para>
            ///     <para><paramref name="beforColor" /> 释放前颜色</para>
            ///     <para><paramref name="afterColor" /> 释放后颜色</para>
            /// </summary>
            /// <param name="befor"></param>
            /// <param name="after"></param>
            /// <returns></returns>
            private static bool MatchAvx2(Color befor, Color after)
            {
                if (!Avx2.IsSupported)
                {
                    return Math.Abs(befor.R * befor.R * 0.0001 + befor.R * 0.7629 - after.R) <= 4
                           && Math.Abs(befor.G * befor.G * 0.0014 + befor.G * 0.0219 + 147 - after.G) <= 4
                           && Math.Abs(befor.B * befor.B * 0.0002 + befor.B * 0.7586 - after.B) <= 4;
                }

                // 一次性加载所有数据
                Vector256<float> beforeVec = Vector256.Create(befor.R, befor.G, befor.B, 0f, 0f, 0f, 0f, 0f);
                Vector256<float> afterVec = Vector256.Create(after.R, after.G, after.B, 0f, 0f, 0f, 0f, 0f);
                Vector256<float> squareCoeff = Vector256.Create(0.0001f, 0.0014f, 0.0002f, 0f, 0f, 0f, 0f, 0f);
                Vector256<float> linearCoeff = Vector256.Create(0.7629f, 0.0219f, 0.7586f, 0f, 0f, 0f, 0f, 0f);
                Vector256<float> constant = Vector256.Create(0f, 147f, 0f, 0f, 0f, 0f, 0f, 0f);

                // x²
                Vector256<float> squared = Avx.Multiply(beforeVec, beforeVec);

                Vector256<float> result = Avx.Add(
                    Avx.Add(
                        // +x²*a
                        Avx.Multiply(squared, squareCoeff)
                        // +x*b
                        , Avx.Multiply(beforeVec, linearCoeff)
                        )
                    // +c
                    , constant);

                // 计算结果和原始的差值
                Vector256<float> diff = Avx.Subtract(result, afterVec);
                // 取绝对值
                Vector256<float> absDiff = Avx.And(diff, Vector256.Create(0x7FFFFFFF).AsSingle());

                // 检查阈值
                return absDiff.GetElement(0) <= 4f
                    && absDiff.GetElement(1) <= 4f
                    && absDiff.GetElement(2) <= 4f;
            }

            /// <summary>
            /// 使用查找表的快速颜色转换
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool FastReleaseColorMatch(Color before, Color after)
            {
                var expectedR = RSquaredLookup[before.R];
                var expectedG = GSquaredLookup[before.G];
                var expectedB = BSquaredLookup[before.B];

                return Math.Abs(expectedR - after.R) <= 4 &&
                       Math.Abs(expectedG - after.G) <= 4 &&
                       Math.Abs(expectedB - after.B) <= 4;
            }
        }

        /// <summary>
        ///     运用算发表查找匹配,减少运算量
        ///     <para><paramref name="befor" /> 释放前颜色</para>
        ///     <para><paramref name="after" /> 释放后颜色</para>
        /// </summary>
        /// <param name="befor"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        private static bool DOTA2释放颜色前后对比(Color befor, Color after)
        {
            return ColorMatcher.FastReleaseColorMatch(befor, after);
        }

        #endregion

        #region 技能具体实现

        /// <summary>
        ///     技能释放完毕后处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为技能释放完那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为技能释放完那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>技能未释放、释放中返回真，释放完毕执行逻辑返回假</returns>
        public static async Task<bool> 主动技能释放后续(Keys skill, Action afterAction)
        {
            if (DOTA2对比释放技能前后颜色(skill, GlobalScreenCapture.GetCurrentHandle()))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            _ = Task.Run(afterAction).ConfigureAwait(true);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     主动技能进入CD后处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为技能进入CD那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为技能进入CD那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>主动技能CD就绪返回真，进入CD执行逻辑后返回假</returns>
        public static async Task<bool> 主动技能进入CD后续(Keys skill, Action afterAction)
        {
            if (DOTA2判断技能是否CD(skill, GlobalScreenCapture.GetCurrentHandle()))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            _ = Task.Run(afterAction).ConfigureAwait(false);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     主动技能CD完毕处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为技能转好CD那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为技能转好CD那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>主动技能CD未就绪返回真，CD就绪执行逻辑后返回假</returns>
        public static async Task<bool> 主动技能已就绪后续(Keys skill, Action afterAction)
        {
            // 检查是否在时间间隔内
            if (技能执行时间.TryGetValue(skill, out DateTime 上次执行时间))
            {
                if ((DateTime.Now - 上次执行时间).TotalMilliseconds < 重复按键执行间隔阈值)
                {
                    // 时间间隔过短，跳过执行
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
            }

            if (!DOTA2判断技能是否CD(skill, GlobalScreenCapture.GetCurrentHandle()))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            技能执行时间.AddOrUpdate(skill, DateTime.Now, (k, v) => DateTime.Now);
            _ = Task.Run(afterAction).ConfigureAwait(false);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     法球技能进入CD处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为法球进入CD那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为法球进入CD那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>法球技能CD就绪返回真，进入CD执行逻辑后返回假</returns>
        public static async Task<bool> 法球技能进入CD后续(Keys skill, Action afterAction)
        {
            if (DOTA2判断法球技能是否CD(skill, GlobalScreenCapture.GetCurrentHandle()))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            _ = Task.Run(afterAction).ConfigureAwait(false);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     状态技能启动后处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为状态技能启动那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为状态技能启动那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>状态技能未启动返回真，启动后逻辑后返回假</returns>
        public static async Task<bool> 状态技能启动后续(Keys skill, Action afterAction)
        {
            if (!DOTA2判断状态技能是否启动(skill, GlobalScreenCapture.GetCurrentHandle()))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            _ = Task.Run(afterAction).ConfigureAwait(false);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     懒加载字典
        /// </summary>
        private static readonly Lazy<ConcurrentDictionary<Keys, DateTime>> _技能执行时间 =
        new Lazy<ConcurrentDictionary<Keys, DateTime>>(() => new ConcurrentDictionary<Keys, DateTime>());

        /// <summary>
        ///     获取对应Key的时间
        /// </summary>
        private static ConcurrentDictionary<Keys, DateTime> 技能执行时间 => _技能执行时间.Value;

        /// <summary>
        ///     循环使用技能等待时间
        /// </summary>
        public static int 重复按键执行间隔阈值 = 100;

        public static void 清除技能时间记录(Keys skill)
        {
            if (_技能执行时间.IsValueCreated)
            {
                技能执行时间.TryRemove(skill, out _);
            }
        }

        // 可选：清除所有技能的时间记录
        public static void 清除所有时间记录()
        {
            if (_技能执行时间.IsValueCreated)
            {
                技能执行时间.Clear();
            }
        }

        /// <summary>
        ///     使用技能后通用后续
        ///     <para><paramref name="判断模式" /> 0 主动技能进入CD 1 释放技能有抬手 2 技能准备好就释放</para>
        ///     <para> 10 进入CD仅切回假腿 11 释放技能仅切回假腿</para>
        /// </summary>
        /// <param name="技能键"></param>
        /// <param name="数组"></param>
        /// <param name="判断模式">0 无充能进入CD 1 释放技能有抬手</param>
        /// <param name="是否接A"></param>
        /// <returns></returns>
        public static async Task<bool> 技能通用判断(Keys 技能键, int 判断模式, bool 是否接按键 = true, Keys 要接的按键 = Keys.A,
            int 判断成功后延时 = 0)
        {
            void 技能后续动作()
            {
                通用技能后续动作(是否接按键, 要接的按键, 判断成功后延时);
            }

            void 切回假腿()
            {
                后续切回假腿(是否接按键, 要接的按键, 判断成功后延时);
            }

            Func<Task<bool>> 使用技能 = 判断模式 switch
            {
                0 => () => 主动技能进入CD后续(技能键, 技能后续动作),
                1 => () => 主动技能释放后续(技能键, 技能后续动作),
                2 => () => 主动技能已就绪后续(技能键, () => { SimKeyBoard.KeyPress(技能键); }),
                10 => () => 主动技能进入CD后续(技能键, 切回假腿),
                11 => () => 主动技能释放后续(技能键, 切回假腿),
                _ => throw new ArgumentException("无效的判断模式")
            };

            return await 使用技能().ConfigureAwait(true);
        }

        /// <summary>
        ///     释放技能后替换图标技能后续
        /// </summary>
        /// <param name="key"></param>
        /// <param name="数组"></param>
        /// <param name="获取全局步骤"></param>
        /// <param name="设置全局步骤"></param>
        /// <returns></returns>
        public static async Task<bool> 释放技能后替换图标技能后续(Keys key, Func<int> 获取全局步骤, Action<int> 设置全局步骤)
        {
            int 全局步骤 = 获取全局步骤();

            switch (全局步骤)
            {
                case 1:
                    return await 主动技能进入CD后续(key, () =>
                    {
                        设置全局步骤(0);
                        Item._切假腿配置.修改配置(key, true);
                    }).ConfigureAwait(true);
                default:
                    _ = await 主动技能释放后续(key, () =>
                    {
                        设置全局步骤(1);
                        通用技能后续动作();
                        Item._切假腿配置.修改配置(key, false);
                    }).ConfigureAwait(true);

                    return await Task.FromResult(true).ConfigureAwait(true);
            }
        }


        /// <summary>
        ///     传入参数是移动接A，否A接移动
        ///     <para><paramref name="是否接A" /> 是移动接A，否A接移动 默认是</para>
        /// </summary>
        /// <param name="是否接A"></param>
        public static void 通用技能后续动作(bool 是否接按键 = true, Keys 要接的按键 = Keys.A, int 等待的延迟 = 0, bool 是否保持假腿 = true)
        {
            _ = Task.Run(() =>
            {
                Common.Delay(等待的延迟);

                if (是否保持假腿)
                {
                    Item.要求保持假腿();
                }

                if (是否接按键)
                {
                    SimKeyBoard.MouseRightClick();
                    SimKeyBoard.KeyPress(要接的按键);
                }
                else
                {
                    SimKeyBoard.MouseRightClick();
                }
            });
        }

        public static void 后续切回假腿(bool 是否接按键 = true, Keys 要接的按键 = Keys.A, int 等待的延迟 = 0)
        {
            _ = Task.Run(() =>
            {
                Common.Delay(等待的延迟);
                Item.要求保持假腿();
            });
        }

        #endregion

        #region 捕捉颜色

        #region 定制测试方法（按需求更改）

        public static Task 测试方法(int x, int y)
        {
            if (Common.Main_Form == null) return Task.CompletedTask;

            long startTime = Common.获取当前时间毫秒();

            // 使用值元组记录颜色变化，减少GC压力
            using var colors = new PooledList<Color>();
            using var longs = new PooledList<long>();

            // 使用取消令牌控制循环寿命
            using var cts = new CancellationTokenSource();
            int 延迟 = 0;
            Common.Main_Form.Invoke(() => { 延迟 = int.Parse(Common.Main_Form?.tb_状态抗性.Text.Trim(), CultureInfo.InvariantCulture); });
            cts.CancelAfter(延迟);

            try
            {
                // 主循环：定期发送时间戳触发屏幕捕获
                while (!cts.IsCancellationRequested)
                {
                    _ = Main.获取图片_2();

                    var color = GlobalScreenCapture.GetColor(x, y);

                    if (colors.Count == 0)
                    {
                        colors.Add(color);
                        longs.Add(0);
                    }
                    else
                    {
                        if (!colors[^1].Equals(color))
                        {
                            colors.Add(color);
                            longs.Add(Common.获取当前时间毫秒() - startTime);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 预期的取消，静默处理
            }
            Common.Main_Form.Invoke(() =>
            {
                // 将结果转换为所需格式
                Common.Main_Form.tb_x.Text = string.Join("|", colors.Select(c => $"{c.R},{c.G},{c.B}"));
                Common.Main_Form.tb_y.Text = string.Join("|", longs.Select((c, index) => index == 0 ? c : c - longs[index - 1]));
            });

            TTS.TTS.Speak("完成");
            return Task.CompletedTask;
        }

        #endregion

        public static async Task 捕捉颜色()
        {
            if (Common.Main_Form == null) return;

            long startTime = Common.获取当前时间毫秒();

            // 使用值元组记录颜色变化，减少GC压力
            using var colors = new PooledList<Color>();
            using var timestamps = new PooledList<long>();

            // 预先定义所有要采集的技能位置和类型配置
            var colorConfigs = GetColorConfigurations();

            // 使用取消令牌控制循环寿命
            var timeout = 0;

            // 缓存当前选中的颜色键
            var selectedKey = "";

            Common.Main_Form.Invoke(() =>
            {
                selectedKey = Common.Main_Form.tb_阵营.Text.Trim();
                timeout = int.Parse(Common.Main_Form.tb_状态抗性.Text.Trim(), CultureInfo.InvariantCulture);
            });

            using var cts = new CancellationTokenSource(timeout);

            try
            {
                // 缓存最近的颜色结果避免频繁的重复计算
                var colorCache = new Dictionary<string, Color>(colorConfigs.Count);

                while (!cts.IsCancellationRequested)
                {
                    // 截图并赋值数组
                    Main.获取图片_2();

                    // 并行获取所有颜色并缓存结果
                    await Task.Run(() =>
                    {
                        Parallel.ForEach(
                            colorConfigs,
                            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                            config =>
                            {
                                var color = 获取技能颜色(config.SkillKey, GlobalScreenCapture.GetCurrentHandle(), config.SkillCount, config.ColorType);
                                lock (colorCache)
                                {
                                    colorCache[config.Key] = color;
                                }
                            });
                    });

                    // 获取当前选择的颜色
                    Color currentColor;
                    if (colorCache.TryGetValue(selectedKey, out var cachedColor))
                    {
                        currentColor = cachedColor;
                    }
                    else
                    {
                        // 默认返回q4
                        currentColor = colorCache.TryGetValue("q4", out var defaultColor) ? defaultColor : Color.Empty;
                    }

                    // 记录颜色变化
                    if (colors.Count == 0)
                    {
                        colors.Add(currentColor);
                        timestamps.Add(0);
                    }
                    else if (!colors[^1].Equals(currentColor))
                    {
                        colors.Add(currentColor);
                        timestamps.Add(Common.获取当前时间毫秒() - startTime);
                    }

                    // 添加短暂休眠，减轻CPU负担
                    // Task.Delay(1, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // 预期的取消，静默处理
            }

            Common.Main_Form.Invoke(() =>
            {
                // 将结果转换为所需格式
                Common.Main_Form.tb_x.Text = string.Join("|", colors.Select(c => $"{c.R},{c.G},{c.B}"));
                Common.Main_Form.tb_y.Text = string.Join("|", timestamps.Select((c, index) => index == 0 ? c : c - timestamps[index - 1]));
            });

            TTS.TTS.Speak("完成");
        }

        // 辅助方法：获取颜色配置
        private static List<ColorConfig> GetColorConfigurations()
        {
            // 预定义所有需要捕获的颜色配置
            var configs = new List<ColorConfig>(150);
            foreach (var key in new[] { Keys.Q, Keys.W, Keys.E, Keys.R })
            {
                foreach (var count in new[] { 4, 5, 6 })
                {
                    // 添加所有颜色类型
                    configs.Add(new ColorConfig($"{key.ToString().ToLower(CultureInfo.CurrentCulture)}{count}", key, count, 技能类型.释放变色));
                    configs.Add(new ColorConfig($"{key.ToString().ToLower(CultureInfo.CurrentCulture)}{count}1", key, count, 技能类型.图标CD));
                    configs.Add(new ColorConfig($"{key.ToString().ToLower(CultureInfo.CurrentCulture)}{count}2", key, count, 技能类型.QWERDF图标));
                    configs.Add(new ColorConfig($"{key.ToString().ToLower(CultureInfo.CurrentCulture)}{count}3", key, count, 技能类型.法球));
                    configs.Add(new ColorConfig($"{key.ToString().ToLower(CultureInfo.CurrentCulture)}{count}4", key, count, 技能类型.状态));
                    configs.Add(new ColorConfig($"{key.ToString().ToLower(CultureInfo.CurrentCulture)}{count}5", key, count, 技能类型.被动技能存在));
                }
            }

            // D键只有5和6
            foreach (var count in new[] { 5, 6 })
            {
                configs.Add(new ColorConfig($"d{count}", Keys.D, count, 技能类型.释放变色));
                configs.Add(new ColorConfig($"d{count}1", Keys.D, count, 技能类型.图标CD));
                configs.Add(new ColorConfig($"d{count}2", Keys.D, count, 技能类型.QWERDF图标));
                configs.Add(new ColorConfig($"d{count}3", Keys.D, count, 技能类型.法球));
                configs.Add(new ColorConfig($"d{count}4", Keys.D, count, 技能类型.状态));
                configs.Add(new ColorConfig($"d{count}5", Keys.D, count, 技能类型.被动技能存在));
            }

            // F键只有6
            configs.Add(new ColorConfig("f6", Keys.F, 6, 技能类型.释放变色));
            configs.Add(new ColorConfig("f61", Keys.F, 6, 技能类型.图标CD));
            configs.Add(new ColorConfig("f62", Keys.F, 6, 技能类型.QWERDF图标));
            configs.Add(new ColorConfig("f63", Keys.F, 6, 技能类型.法球));
            configs.Add(new ColorConfig("f64", Keys.F, 6, 技能类型.状态));
            configs.Add(new ColorConfig("f65", Keys.F, 6, 技能类型.被动技能存在));

            return configs;
        }

        // 统一获取技能颜色的方法
        private static Color 获取技能颜色(Keys skillKey, in ImageHandle 句柄, int skillCount, 技能类型 colorType)
        {
            return colorType switch
            {
                技能类型.释放变色 => 获取技能释放判断颜色(skillKey, in 句柄, skillCount),
                技能类型.图标CD => 获取技能进入CD判断颜色(skillKey, in 句柄, skillCount),
                技能类型.QWERDF图标 => 获取QWERDF颜色(skillKey, in 句柄, skillCount),
                技能类型.法球 => 获取法球颜色(skillKey, in 句柄, skillCount),
                技能类型.状态 => 获取状态颜色(skillKey, in 句柄, skillCount),
                技能类型.被动技能存在 => 获取被动颜色(skillKey, in 句柄, skillCount),
                _ => Color.Empty,
            };
        }

        // 颜色配置记录类
        private readonly struct ColorConfig
        {
            public string Key { get; }
            public Keys SkillKey { get; }
            public int SkillCount { get; }
            public 技能类型 ColorType { get; }

            public ColorConfig(string key, Keys skillKey, int skillCount, 技能类型 colorType)
            {
                Key = key;
                SkillKey = skillKey;
                SkillCount = skillCount;
                ColorType = colorType;
            }
        }

        #endregion
    }
}

#endif