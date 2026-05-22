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
    /// <summary>
    ///     颜色对比
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        ///     比较颜色
        /// </summary>
        /// <param name="colorA"></param>
        /// <param name="colorB"></param>
        /// <param name="errorRange">默认不传的话容差为10</param>
        /// <returns></returns>
        public static bool ColorAEqualColorB(in Color colorA, in Color colorB, byte errorRange = 10)
        {
            return colorA.EqualsWithError(colorB, errorRange);
        }

        /// <summary>
        ///     比较颜色
        /// </summary>
        /// <param name="colorA"></param>
        /// <param name="colorB"></param>
        /// <param name="errorR">R容差</param>
        /// <param name="errorG">G容差</param>
        /// <param name="errorB">B容差</param>
        /// <returns></returns>
        public static bool ColorAEqualColorB(in Color colorA, in Color colorB, byte errorR, byte errorG, byte errorB)
        {
            return colorA.EqualsRGBWithError(in colorB, errorR, errorG, errorB);
        }

        /// <summary>
        /// 带误差范围的颜色相等比较（所有RGBA通道使用相同误差值）
        /// </summary>
        /// <param name="color">要比较的颜色</param>
        /// <param name="other">目标颜色</param>
        /// <param name="errorRange">允许的误差范围（默认5）</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        private static bool EqualsWithError(this in Color color, in Color other, byte errorRange = 5)
        {
            // 使用SIMD指令实现RGBA值快速比较
            if (Sse2.IsSupported)
            {
                Vector128<int> vecA = Vector128.Create(
                    color.B,
                    color.G,
                    color.R,
                    color.A
                );
                Vector128<int> vecB = Vector128.Create(
                    other.B,
                    other.G,
                    other.R,
                    other.A
                );
                Vector128<int> errorVec = Vector128.Create((int)errorRange);

                // 计算差值
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);  // 获取符号位
                Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);  // 绝对值计算

                // 比较差值是否超过误差范围
                Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

                // 检查所有分量是否都在误差范围内
                return Sse2.MoveMask(cmp.AsSingle()) == 0;
            }

            // 不支持SIMD时的回退方案
            return Math.Abs(color.R - other.R) <= errorRange &&
                   Math.Abs(color.G - other.G) <= errorRange &&
                   Math.Abs(color.B - other.B) <= errorRange &&
                   Math.Abs(color.A - other.A) <= errorRange;
        }

        /// <summary>
        /// 带误差范围的RGB颜色相等比较（各通道可以指定不同误差值）
        /// </summary>
        /// <param name="color">要比较的颜色</param>
        /// <param name="other">目标颜色</param>
        /// <param name="errorR">R通道允许的误差范围</param>
        /// <param name="errorG">G通道允许的误差范围</param>
        /// <param name="errorB">B通道允许的误差范围</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        private static bool EqualsRGBWithError(this in Color color, in Color other,
            byte errorR = 5, byte errorG = 5, byte errorB = 5)
        {
            // 使用SIMD指令实现RGB值快速比较
            if (Sse2.IsSupported)
            {
                Vector128<int> vecA = Vector128.Create(
                    color.B,
                    color.G,
                    color.R,
                    0
                );
                Vector128<int> vecB = Vector128.Create(
                    other.B,
                    other.G,
                    other.R,
                    0
                );
                Vector128<int> errorVec = Vector128.Create(
                    errorB,
                    errorG,
                    errorR,
                    0
                );

                // 计算差值
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);  // 获取符号位
                Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);  // 绝对值计算

                // 比较差值是否超过误差范围
                Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

                // 只检查RGB三个分量（忽略Alpha）
                return (Sse2.MoveMask(cmp.AsSingle()) & 0x7) == 0;
            }

            // 不支持SIMD时的回退方案
            return Math.Abs(color.R - other.R) <= errorR &&
                   Math.Abs(color.G - other.G) <= errorG &&
                   Math.Abs(color.B - other.B) <= errorB;
        }

        /// <summary>
        ///     基于uint指针的颜色比较（ARGB格式，统一误差范围）
        /// </summary>
        /// <param name="pixelA">指向第一个像素的指针</param>
        /// <param name="pixelB">指向第二个像素的指针</param>
        /// <param name="errorRange">误差范围（默认10）</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        public static unsafe bool ComparePixels(uint* pixelA, uint* pixelB, byte errorRange = 10)
        {
            return ComparePixelValues(*pixelA, *pixelB, errorRange);
        }

        /// <summary>
        ///     基于uint值的颜色比较（ARGB格式，统一误差范围）
        /// </summary>
        /// <param name="pixelA">第一个像素值（ARGB格式）</param>
        /// <param name="pixelB">第二个像素值（ARGB格式）</param>
        /// <param name="errorRange">误差范围（默认10）</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        public static bool ComparePixelValues(uint pixelA, uint pixelB, byte errorRange = 10)
        {
            // 使用SIMD指令实现ARGB值快速比较
            if (Sse2.IsSupported)
            {
                // 解析ARGB格式 (0xAARRGGBB)
                Vector128<int> vecA = Vector128.Create(
                    (int)(pixelA & 0xFF),        // B
                    (int)((pixelA >> 8) & 0xFF), // G
                    (int)((pixelA >> 16) & 0xFF),// R
                    (int)((pixelA >> 24) & 0xFF) // A
                );
                Vector128<int> vecB = Vector128.Create(
                    (int)(pixelB & 0xFF),        // B
                    (int)((pixelB >> 8) & 0xFF), // G
                    (int)((pixelB >> 16) & 0xFF),// R
                    (int)((pixelB >> 24) & 0xFF) // A
                );
                Vector128<int> errorVec = Vector128.Create((int)errorRange);

                // 计算差值
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);
                Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);

                // 比较差值是否超过误差范围
                Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

                // 检查所有分量是否都在误差范围内
                return Sse2.MoveMask(cmp.AsSingle()) == 0;
            }

            // 不支持SIMD时的回退方案
            byte aR = (byte)((pixelA >> 16) & 0xFF);
            byte aG = (byte)((pixelA >> 8) & 0xFF);
            byte aB = (byte)(pixelA & 0xFF);
            byte aA = (byte)((pixelA >> 24) & 0xFF);

            byte bR = (byte)((pixelB >> 16) & 0xFF);
            byte bG = (byte)((pixelB >> 8) & 0xFF);
            byte bB = (byte)(pixelB & 0xFF);
            byte bA = (byte)((pixelB >> 24) & 0xFF);

            return Math.Abs(aR - bR) <= errorRange &&
                   Math.Abs(aG - bG) <= errorRange &&
                   Math.Abs(aB - bB) <= errorRange &&
                   Math.Abs(aA - bA) <= errorRange;
        }

        /// <summary>
        ///     基于uint指针的RGB颜色比较（各通道独立误差范围）
        /// </summary>
        /// <param name="pixelA">指向第一个像素的指针</param>
        /// <param name="pixelB">指向第二个像素的指针</param>
        /// <param name="errorR">R通道误差范围</param>
        /// <param name="errorG">G通道误差范围</param>
        /// <param name="errorB">B通道误差范围</param>
        /// <returns>如果RGB颜色在误差范围内相等则返回true</returns>
        public static unsafe bool ComparePixelsRGB(uint* pixelA, uint* pixelB,
            byte errorR, byte errorG, byte errorB)
        {
            return ComparePixelValuesRGB(*pixelA, *pixelB, errorR, errorG, errorB);
        }

        /// <summary>
        ///     基于uint值的RGB颜色比较（各通道独立误差范围）
        /// </summary>
        /// <param name="pixelA">第一个像素值（ARGB格式）</param>
        /// <param name="pixelB">第二个像素值（ARGB格式）</param>
        /// <param name="errorR">R通道误差范围</param>
        /// <param name="errorG">G通道误差范围</param>
        /// <param name="errorB">B通道误差范围</param>
        /// <returns>如果RGB颜色在误差范围内相等则返回true</returns>
        public static bool ComparePixelValuesRGB(uint pixelA, uint pixelB,
            byte errorR, byte errorG, byte errorB)
        {
            // 使用SIMD指令实现RGB值快速比较
            if (Sse2.IsSupported)
            {
                // 解析ARGB格式，忽略Alpha通道
                Vector128<int> vecA = Vector128.Create(
                    (int)(pixelA & 0xFF),        // B
                    (int)((pixelA >> 8) & 0xFF), // G
                    (int)((pixelA >> 16) & 0xFF),// R
                    0                            // 忽略A
                );
                Vector128<int> vecB = Vector128.Create(
                    (int)(pixelB & 0xFF),        // B
                    (int)((pixelB >> 8) & 0xFF), // G
                    (int)((pixelB >> 16) & 0xFF),// R
                    0                            // 忽略A
                );
                Vector128<int> errorVec = Vector128.Create(
                    errorB,
                    errorG,
                    errorR,
                    0
                );

                // 计算差值
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);
                Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);

                // 比较差值是否超过误差范围
                Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

                // 只检查RGB三个分量（忽略Alpha）
                return (Sse2.MoveMask(cmp.AsSingle()) & 0x7) == 0;
            }

            // 不支持SIMD时的回退方案
            byte aR = (byte)((pixelA >> 16) & 0xFF);
            byte aG = (byte)((pixelA >> 8) & 0xFF);
            byte aB = (byte)(pixelA & 0xFF);

            byte bR = (byte)((pixelB >> 16) & 0xFF);
            byte bG = (byte)((pixelB >> 8) & 0xFF);
            byte bB = (byte)(pixelB & 0xFF);

            return Math.Abs(aR - bR) <= errorR &&
                   Math.Abs(aG - bG) <= errorG &&
                   Math.Abs(aB - bB) <= errorB;
        }

        /// <summary>
        ///     批量像素比较（优化性能）
        /// </summary>
        /// <param name="pixelsA">第一组像素数据指针</param>
        /// <param name="pixelsB">第二组像素数据指针</param>
        /// <param name="count">要比较的像素数量</param>
        /// <param name="errorRange">误差范围</param>
        /// <returns>返回相等的像素数量</returns>
        public static unsafe int CompareBatchPixels(uint* pixelsA, uint* pixelsB,
            int count, byte errorRange = 10)
        {
            int equalCount = 0;

            for (int i = 0; i < count; i++)
            {
                if (ComparePixelValues(pixelsA[i], pixelsB[i], errorRange))
                {
                    equalCount++;
                }
            }

            return equalCount;
        }

        /// <summary>
        ///     从uint值获取各颜色分量
        /// </summary>
        /// <param name="pixel">像素值（ARGB格式）</param>
        /// <returns>返回(A, R, G, B)元组</returns>
        public static (byte A, byte R, byte G, byte B) GetColorComponents(uint pixel)
        {
            return (
                (byte)((pixel >> 24) & 0xFF), // A
                (byte)((pixel >> 16) & 0xFF), // R
                (byte)((pixel >> 8) & 0xFF),  // G
                (byte)(pixel & 0xFF)          // B
            );
        }

        /// <summary>
        ///     从各颜色分量构建uint像素值
        /// </summary>
        /// <param name="a">Alpha通道</param>
        /// <param name="r">Red通道</param>
        /// <param name="g">Green通道</param>
        /// <param name="b">Blue通道</param>
        /// <returns>ARGB格式的像素值</returns>
        public static uint CreatePixelValue(byte a, byte r, byte g, byte b)
        {
            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }
    }
}
