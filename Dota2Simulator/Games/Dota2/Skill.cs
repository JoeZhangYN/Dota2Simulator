//#define 输出技能信息
#if DOTA2

using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.TTS;
using ImageProcessingSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Dota2Simulator.Games.Dota2.MainClass;
using static Dota2Simulator.Games.Dota2.Item;

namespace Dota2Simulator.Games.Dota2
{
    internal class Skill
    {
        #region 模块化技能

        // 模块化之后，都不用注释也能看得懂了。。
        // 技能CD为x为指定 y为灰白色最上方像素 |涉及的颜色| 技能CD颜色实际上是释放技能时变换的颜色 所以容差大 未学习技能CD颜色 
        // 5 6 进入CD颜色渐变 122,129,136 94,100,106 16,19,18 8,11,10 非指向性施法颜色不变
        // 法球位置为灰白色左下角图标 |涉及的颜色| 法球颜色 未学习法球颜色
        // 推荐学习和状态技能取同一个位置 开启技能后 y最底下绿色像素 金黄色下往上数2  5,6技能 为暗黄色最底下 |涉及的颜色| 推荐学习颜色 状态激活颜色
        // QWERDF框x为左下角x向右偏移一个像素 y为特定的y 被动位置x为图标右侧边框左边第一个像素,有的技能亮色影响内侧颜色 y为qwerdf 的某个高度 |涉及的颜色| qwerdf颜色 被动颜色 未学习被动技能颜色 破坏被动颜色
        private static readonly 技能信息 技能4 = new(
            820, 998, 65
            , 27, -52, Color.FromArgb(133, 141, 148), 5, Color.FromArgb(72, 76, 80), 2
            , -1, -45, Color.FromArgb(34, 40, 39), 2
            , 2, -2, Color.FromArgb(54, 62, 70), 2, Color.FromArgb(25, 30, 34), 2
            , 25, -56, Color.FromArgb(0, 129, 0), 0, Color.FromArgb(211, 181, 79), 8
            , 50, -45, Color.FromArgb(49, 51, 47), 3, Color.FromArgb(33, 36, 37), 3
            , 25, -49
            , Color.FromArgb(67, 76, 84), 1 // 没用到
            , [Keys.Q, Keys.W, Keys.E, Keys.R]);

        private static readonly 技能信息 技能5 = new(
            802, 995, 58
            , 30, -49, Color.FromArgb(134, 142, 149), 5, Color.FromArgb(72, 76, 80), 2
            , 1, -41, Color.FromArgb(34, 40, 39), 1
            , 3, -3, Color.FromArgb(54, 61, 69), 2, Color.FromArgb(25, 29, 34), 2
            , 25, -50, Color.FromArgb(0, 129, 0), 0, Color.FromArgb(118, 100, 41), 8
            , 51, -41, Color.FromArgb(49, 51, 47), 3, Color.FromArgb(33, 36, 37), 3
            , 24, -47
            , Color.FromArgb(55, 62, 70), 1  // 没用到
            , [Keys.Q, Keys.W, Keys.E, Keys.D, Keys.R]);

        private static readonly 技能信息 技能6 = new(
            772, 995, 58
            , 30, -49, Color.FromArgb(134, 142, 149), 5, Color.FromArgb(72, 76, 80), 2
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
                case 技能类型.图标CD:
                    x = 技能信息.技能CD图标x + offsetX - 坐标偏移x;
                    y = 技能信息.技能CD图标y - 坐标偏移y;
                    颜色 = 技能信息.技能CD颜色;
                    颜色容差 = 技能信息.技能CD颜色容差;
                    break;
                case 技能类型.法球:
                    x = 技能信息.法球技能CD位置x + offsetX - 坐标偏移x;
                    y = 技能信息.法球技能CD位置y - 坐标偏移y;
                    颜色 = 技能信息.法球技能CD颜色;
                    颜色容差 = 技能信息.法球技能颜色容差;
                    break;
                case 技能类型.状态:
                    x = 技能信息.状态技能位置x + offsetX - 坐标偏移x;
                    y = 技能信息.状态技能位置y - 坐标偏移y;
                    颜色 = 技能信息.技能状态激活颜色;
                    颜色容差 = 技能信息.技能状态激活颜色容差;
                    break;
                case 技能类型.释放变色:
                    x = 技能信息.释放变色位置x + offsetX - 坐标偏移x;
                    y = 技能信息.释放变色位置y - 坐标偏移y;
                    颜色 = default;
                    颜色容差 = 0;
                    break;
                case 技能类型.QWERDF图标:
                    x = 技能信息.QWERDF位置x + offsetX - 坐标偏移x;
                    y = 技能信息.QWERDF位置y - 坐标偏移y;
                    颜色 = 技能信息.QWERDF框颜色;
                    颜色容差 = 技能信息.QWERDF框颜色容差;
                    break;
                case 技能类型.被动技能存在:
                    x = 技能信息.被动位置x + offsetX - 坐标偏移x;
                    y = 技能信息.被动位置y - 坐标偏移y;
                    颜色 = 技能信息.未学被动技能颜色;
                    颜色容差 = 技能信息.未学被动技能颜色容差;
                    break;
                case 技能类型.破坏被动技能:
                    x = 技能信息.被动位置x + offsetX - 坐标偏移x;
                    y = 技能信息.被动位置y - 坐标偏移y;
                    颜色 = 技能信息.破坏被动技能颜色;
                    颜色容差 = 技能信息.破坏被动技能颜色容差;
                    break;
                case 技能类型.未学主动技能:
                    x = 技能信息.技能CD图标x + offsetX - 坐标偏移x;
                    y = 技能信息.技能CD图标y - 坐标偏移y;
                    颜色 = 技能信息.未学主动技能CD颜色;
                    颜色容差 = 技能信息.未学主动技能CD颜色容差;
                    break;
                case 技能类型.未学法球技能:
                    x = 技能信息.法球技能CD位置x + offsetX - 坐标偏移x;
                    y = 技能信息.法球技能CD位置y - 坐标偏移y;
                    颜色 = 技能信息.未学法球技能CD颜色;
                    颜色容差 = 技能信息.未学法球技能CD颜色容差;
                    break;
                case 技能类型.推荐学习技能:
                    x = 技能信息.状态技能位置x + offsetX - 坐标偏移x;
                    y = 技能信息.状态技能位置y - 坐标偏移y;
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
                    Logger.Info(输出文字.ToString()); 
#endif

                    return 期望数量;
                }

                全部文字.Append(输出文字.ToString());

#if 输出技能信息
                // 结束循环依旧没匹配到
                if (j == 2) Logger.Error(全部文字.ToString()); 
#endif

            }

            Tts.Speak("技能数量异常");
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
                调试信息.AppendLine($"{i + 1} QWERDF图标 :位置X:{检测结果[0].位置.X + 坐标偏移x},位置Y:{检测结果[0].位置.Y + 坐标偏移y}，RGB:{检测结果[0].颜色.R}, {检测结果[0].颜色.G}, {检测结果[0].颜色.B}");
                调试信息.AppendLine($"{i + 1} 技能CD图标 :位置X:{检测结果[1].位置.X + 坐标偏移x},位置Y:{检测结果[1].位置.Y + 坐标偏移y}，RGB:{检测结果[1].颜色.R}, {检测结果[1].颜色.G}, {检测结果[1].颜色.B}。");
                调试信息.AppendLine($"{i + 1} 技能法球 :位置X:{检测结果[2].位置.X + 坐标偏移x},位置Y:{检测结果[2].位置.Y + 坐标偏移y}，RGB:{检测结果[2].颜色.R}, {检测结果[2].颜色.G}, {检测结果[2].颜色.B}。");
                调试信息.AppendLine($"{i + 1} 被动技能 :位置X:{检测结果[3].位置.X + 坐标偏移x},位置Y:{检测结果[3].位置.Y + 坐标偏移y}，RGB:{检测结果[3].颜色.R}, {检测结果[3].颜色.G}, {检测结果[3].颜色.B}。");
                调试信息.AppendLine($"{i + 1} 推荐技能 :位置X:{检测结果[4].位置.X + 坐标偏移x},位置Y:{检测结果[4].位置.Y + 坐标偏移y}，RGB:{检测结果[4].颜色.R}, {检测结果[4].颜色.G}, {检测结果[4].颜色.B}。");
                调试信息.AppendLine();
                return (true, 调试信息.ToString());
            }

            // 如果没有匹配，按原逻辑输出不匹配的项
            // QWERDF图标
            if (!检测结果[0].匹配结果.Any(match => match))
            {
                调试信息.AppendLine($"{i + 1} QWERDF图标 :位置X:{检测结果[0].位置.X + 坐标偏移x},位置Y:{检测结果[0].位置.Y + 坐标偏移y}，RGB:{检测结果[0].颜色.R}, {检测结果[0].颜色.G}, {检测结果[0].颜色.B}");
            }
            if (!检测结果[1].匹配结果.Any(match => match)) // 主动技能
            {
                调试信息.AppendLine($"{i + 1} 技能CD图标 :位置X:{检测结果[1].位置.X + 坐标偏移x},位置Y:{检测结果[1].位置.Y + 坐标偏移y}，RGB:{检测结果[1].颜色.R}, {检测结果[1].颜色.G}, {检测结果[1].颜色.B}。");
            }
            if (!检测结果[2].匹配结果.Any(match => match)) // 法球技能
            {
                调试信息.AppendLine($"{i + 1} 技能法球 :位置X:{检测结果[2].位置.X + 坐标偏移x},位置Y:{检测结果[2].位置.Y + 坐标偏移y}，RGB:{检测结果[2].颜色.R}, {检测结果[2].颜色.G}, {检测结果[2].颜色.B}。");
            }
            if (!检测结果[3].匹配结果.Any(match => match)) // 被动技能
            {
                调试信息.AppendLine($"{i + 1} 被动技能 :位置X:{检测结果[3].位置.X + 坐标偏移x},位置Y:{检测结果[3].位置.Y + 坐标偏移y}，RGB:{检测结果[3].颜色.R}, {检测结果[3].颜色.G}, {检测结果[3].颜色.B}。");
            }
            if (!检测结果[4].匹配结果.Any(match => match)) // 推荐技能
            {
                调试信息.AppendLine($"{i + 1} 推荐技能 :位置X:{检测结果[4].位置.X + 坐标偏移x},位置Y:{检测结果[4].位置.Y + 坐标偏移y}，RGB:{检测结果[4].颜色.R}, {检测结果[4].颜色.G}, {检测结果[4].颜色.B}。");
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

            Point p_QWERDF = new(技能.QWERDF位置x + 偏移 - 坐标偏移x, 技能.QWERDF位置y - 坐标偏移y);
            Point p_主动 = new(技能.技能CD图标x + 偏移 - 坐标偏移x, 技能.技能CD图标y - 坐标偏移y);
            Point p_法球 = new(技能.法球技能CD位置x + 偏移 - 坐标偏移x, 技能.法球技能CD位置y - 坐标偏移y);
            Point p_被动 = new(技能.被动位置x + 偏移 - 坐标偏移x, 技能.被动位置y - 坐标偏移y);
            Point p_推荐 = new(技能.状态技能位置x + 偏移 - 坐标偏移x, 技能.状态技能位置y - 坐标偏移y);

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
            return ColorExtensions.ColorAEqualColorB(MainClass.获取指定位置颜色(953, 764, in 句柄), Color.FromArgb(254, 254, 254), 2);
        }

        private static void 记录技能释放信息(Keys s1, string s, bool b1, bool b2, Color c1, Color c2, Color c3)
        {
            Logger.Info($"\r\n{s1}{s}\r\n"
                                + $"判断是否释放{b1}\r\n"
                                + $"释放后变色{b2}\r\n"
                                + $"释放前{c1}\r\n"
                                + $"释放后{c2}\r\n"
                                + $"对比色{c3}\r\n");
        }

        private static void 记录技能释放信息(Keys s1, Keys 技能位置)
        {
            Logger.Info(s1);

            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 获取技能状态))
                {
                    bool 释放中判断 = 获取技能状态.释放中判断;
                    bool 已释放变色判断 = 获取技能状态.已释放变色判断;
                    Color 释放前Color = 获取技能状态.释放前Color;
                    Color 释放后Color = 获取技能状态.释放后Color;
                    Logger.Info($"\r\n获取到{s1}的技能状态\r\n"
                                + $"判断是否释放{释放中判断}\r\n"
                                + $"释放后变色{已释放变色判断}\r\n"
                                + $"释放前{释放前Color}\r\n"
                                + $"释放后{释放后Color}\r\n");
                }
                else
                {
                    Logger.Info($"获取不到{技能位置}的技能状态");
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

            Point p_主动 = new(技能信息.技能CD图标x + 偏移 - 坐标偏移x, 技能信息.技能CD图标y - 坐标偏移y);

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

        private static bool 处理技能释放(Keys 技能位置, Color 当前释放颜色)
        {
            if (!获取技能释放状态(技能位置, out bool 释放中判断, out bool 已释放变色判断, out Color 释放前Color, out Color 释放后Color))
            {
                Logger.Info("字典读取失败");
                return false;
            }
            if (释放中判断)
            {
                // 技能释放中
                if (!已释放变色判断)
                {
                    // 技能未释放完毕
                    if (ColorExtensions.ColorAEqualColorB(释放后Color, 当前释放颜色, 0))
                    {
                        更新释放判断和颜色(技能位置, true, false, 释放前Color, 释放后Color);
                        return true;
                    }
                    else if (ColorExtensions.ColorAEqualColorB(释放前Color, 当前释放颜色, 0))  // 新增：判断是否回到初始颜色
                    {
                        更新释放判断和颜色(技能位置, false, true, 释放前Color, 释放后Color);
                        // 记录技能释放信息(技能位置, "已释放完毕", false, true, 释放前Color, 释放后Color, 当前释放颜色);
                        return false;
                    }
                    else
                    {
                        更新释放判断和颜色(技能位置, false, true, 释放前Color, 释放后Color);
                        // 记录技能释放信息(技能位置, "已释放完毕", false, true, 释放前Color, 释放后Color, 当前释放颜色);
                        return false;
                    }
                }
                return false;
            }
            else
            {
                // 未释放中 颜色相同时
                if (ColorExtensions.ColorAEqualColorB(释放前Color, 当前释放颜色, 0))
                {
                    更新释放判断和颜色(技能位置, false, false, 释放前Color, 释放后Color);
                    return true;
                }
                // 未释放中 颜色不同时
                else
                {
                    // 释放后不为空 且颜色相同 说明正在释放 或者释放前颜色变绿 说明正在释放
                    if (释放后Color != Color.Empty && ColorExtensions.ColorAEqualColorB(释放后Color, 当前释放颜色, 0)
                        || DOTA2释放颜色前后对比(释放前Color, 当前释放颜色))
                    {
                        更新释放判断和颜色(技能位置, true, false, 释放前Color, 当前释放颜色);
                        // 记录技能释放信息(技能位置, "释放中", true, false, 释放前Color, 当前释放颜色, 当前释放颜色);
                    }
                    return true;
                }
            }
        }

        /// <summary>
        ///     变色运算匹配，符合的返回真，不符合返回假
        ///     <para>10000次 1ms..</para>
        ///     <para><paramref name="beforColor" /> 释放前颜色</para>
        ///     <para><paramref name="afterColor" /> 释放后颜色</para>
        /// </summary>
        /// <param name="beforColor"></param>
        /// <param name="afterColor"></param>
        /// <returns></returns>
        private static bool DOTA2释放颜色前后对比(Color beforColor, Color afterColor)
        {
            if (!Avx2.IsSupported)
            {
                return Math.Abs(beforColor.R * beforColor.R * 0.0001 + beforColor.R * 0.7656 - afterColor.R) <= 3
                       && Math.Abs(beforColor.G * beforColor.G * 0.0014 + beforColor.G * 0.0251 + 147 - afterColor.G) <= 3
                       && Math.Abs(beforColor.B * beforColor.B * 0.0002 + beforColor.B * 0.751 - afterColor.B) <= 3;
            }

            // 修改2处，上面数值

            // Load color components into Vector256<float>
            Vector256<float> beforColorVec = Vector256.Create(
                beforColor.R, beforColor.G, beforColor.B, 0f,
                beforColor.R, beforColor.G, beforColor.B, 0f
            );

            // 这里的数值之一

            // Compute R channel comparison
            Vector256<float> squaredBeforColorVecR = Avx.Multiply(beforColorVec, beforColorVec);
            Vector256<float> tempR = Avx.Add(Avx.Multiply(squaredBeforColorVecR, Vector256.Create(0.0001f)),
                Avx.Multiply(beforColorVec, Vector256.Create(0.7656f)));
            Vector256<float> diffR = Avx.Subtract(tempR, Vector256.Create((float)afterColor.R));
            Vector256<float> absDiffR = Avx.And(diffR, Vector256.Create(0x7FFFFFFF).AsSingle());
            float rResultValue = absDiffR.GetElement(0);
            bool rResult = rResultValue <= 3f;

            // 这里的数值之一

            // Compute G channel comparison
            Vector256<float> squaredBeforColorVecG = Avx.Multiply(beforColorVec, beforColorVec);
            Vector256<float> tempG = Avx.Add(Avx.Multiply(squaredBeforColorVecG, Vector256.Create(0.0014f)),
                Avx.Multiply(beforColorVec, Vector256.Create(0.0251f)));
            tempG = Avx.Add(tempG, Vector256.Create(147f));
            Vector256<float> diffG = Avx.Subtract(tempG, Vector256.Create((float)afterColor.G));
            Vector256<float> absDiffG = Avx.And(diffG, Vector256.Create(0x7FFFFFFF).AsSingle());
            float gResultValue = absDiffG.GetElement(1);
            bool gResult = gResultValue <= 3f;

            // 这里的数值之一

            // Compute B channel comparison
            Vector256<float> squaredBeforColorVecB = Avx.Multiply(beforColorVec, beforColorVec);
            Vector256<float> tempB = Avx.Add(Avx.Multiply(squaredBeforColorVecB, Vector256.Create(0.0002f)),
                Avx.Multiply(beforColorVec, Vector256.Create(0.751f)));
            Vector256<float> diffB = Avx.Subtract(tempB, Vector256.Create((float)afterColor.B));
            Vector256<float> absDiffB = Avx.And(diffB, Vector256.Create(0x7FFFFFFF).AsSingle());
            float bResultValue = absDiffB.GetElement(2);
            bool bResult = bResultValue <= 3f;

            return rResult && gResult && bResult;
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
            if (DOTA2对比释放技能前后颜色(skill, in MainClass._全局图像句柄))
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
            if (DOTA2判断技能是否CD(skill, in MainClass._全局图像句柄))
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
            if (!DOTA2判断技能是否CD(skill, in MainClass._全局图像句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

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
            if (DOTA2判断法球技能是否CD(skill, in MainClass._全局图像句柄))
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
            if (!DOTA2判断状态技能是否启动(skill, MainClass._全局图像句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            _ = Task.Run(afterAction).ConfigureAwait(false);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     使用技能后通用后续
        ///     <para><paramref name="判断模式" /> 0 主动技能进入CD 1 释放技能有抬手</para>
        ///     <para> 10 进入CD仅切回假腿 11 释放技能仅切回假腿</para>
        /// </summary>
        /// <param name="技能键"></param>
        /// <param name="数组"></param>
        /// <param name="判断模式">0 无充能进入CD 1 释放技能有抬手</param>
        /// <param name="是否接A"></param>
        /// <returns></returns>
        public static async Task<bool> 使用技能后通用后续(Keys 技能键, int 判断模式, bool 是否接按键 = true, Keys 要接的按键 = Keys.A,
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
                        _切假腿配置.修改配置(key, false);
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
                MainClass.Delay(等待的延迟);

                if (是否保持假腿)
                {
                    要求保持假腿();
                }

                if (是否接按键)
                {
                    SimKeyBoard.MouseRightClick();
                    SimKeyBoard.KeyPress(要接的按键);
                }
                else
                {
                    SimKeyBoard.KeyPress(Keys.A);
                    SimKeyBoard.MouseRightClick();
                }
            });
        }

        public static void 后续切回假腿(bool 是否接按键 = true, Keys 要接的按键 = Keys.A, int 等待的延迟 = 0)
        {
            _ = Task.Run(() =>
            {
                MainClass.Delay(等待的延迟);
                要求保持假腿();
            });
        }

        #endregion
    }
}

#endif