using System;
using System.Drawing;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Dota2Simulator.PictureProcessing
{
    public static class ColorExtensions
    {
        /// <summary>
        /// 带误差范围的颜色相等比较（所有RGBA通道使用相同误差值）
        /// </summary>
        /// <param name="color">要比较的颜色</param>
        /// <param name="other">目标颜色</param>
        /// <param name="errorRange">允许的误差范围（默认5）</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        public static bool EqualsWithError(this Color color, Color other, byte errorRange = 5)
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
        public static bool EqualsRGBWithError(this Color color, Color other,
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
        /// 检查颜色是否在指定颜色范围内
        /// </summary>
        /// <param name="color">要检查的颜色</param>
        /// <param name="minColor">最小颜色值</param>
        /// <param name="maxColor">最大颜色值</param>
        /// <returns>如果颜色在指定范围内则返回true</returns>
        public static bool IsInRange(this Color color, Color minColor, Color maxColor)
        {
            if (Sse2.IsSupported)
            {
                Vector128<int> vecColor = Vector128.Create(
                    color.B, color.G, color.R, 0);

                Vector128<int> vecMin = Vector128.Create(
                    minColor.B, minColor.G, minColor.R, 0);

                Vector128<int> vecMax = Vector128.Create(
                    maxColor.B, maxColor.G, maxColor.R, 0);

                // 检查是否大于等于最小值
                Vector128<int> cmpMin = Sse2.CompareGreaterThan(vecMin, vecColor);

                // 检查是否小于等于最大值
                Vector128<int> cmpMax = Sse2.CompareGreaterThan(vecColor, vecMax);

                // 合并结果，只检查RGB通道（低3位）
                int mask = Sse2.MoveMask(cmpMin.AsSingle()) | Sse2.MoveMask(cmpMax.AsSingle());
                return (mask & 0x7) == 0;
            }

            return color.R >= minColor.R && color.R <= maxColor.R &&
                   color.G >= minColor.G && color.G <= maxColor.G &&
                   color.B >= minColor.B && color.B <= maxColor.B;
        }

        /// <summary>
        /// 获取两个颜色之间的欧几里得距离
        /// </summary>
        /// <param name="color">第一个颜色</param>
        /// <param name="other">第二个颜色</param>
        /// <param name="includeAlpha">是否包含Alpha通道的比较</param>
        /// <returns>两个颜色在RGB(A)空间中的欧几里得距离</returns>
        public static float GetColorDistance(this Color color, Color other, bool includeAlpha = false)
        {
            if (Sse2.IsSupported)
            {
                Vector128<int> vecA = Vector128.Create(
                    color.B, color.G, color.R, includeAlpha ? color.A : 0);

                Vector128<int> vecB = Vector128.Create(
                    other.B, other.G, other.R, includeAlpha ? other.A : 0);

                // 计算差值的平方
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> squared = Sse41.MultiplyLow(diff, diff);

                // 计算平方和
                int sum = squared.GetElement(0) + squared.GetElement(1) +
                          squared.GetElement(2) + (includeAlpha ? squared.GetElement(3) : 0);

                return (float)Math.Sqrt(sum);
            }

            // 标准算法回退
            float diffR = color.R - other.R;
            float diffG = color.G - other.G;
            float diffB = color.B - other.B;

            if (includeAlpha)
            {
                float diffA = color.A - other.A;
                return (float)Math.Sqrt(diffR * diffR + diffG * diffG + diffB * diffB + diffA * diffA);
            }

            return (float)Math.Sqrt(diffR * diffR + diffG * diffG + diffB * diffB);
        }
    }
}
