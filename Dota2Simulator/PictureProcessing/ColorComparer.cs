using System.Drawing;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Dota2Simulator.PictureProcessing
{
    /// <summary>
    /// Windows 11 64位系统下的颜色比较器
    /// 使用AVX2/SSE2指令集优化颜色比较性能
    /// </summary>
    public static class ColorComparer
    {
        public static bool ColorAEqualColorB(Color colorA, Color colorB, byte errorRange)
        {
            Vector128<int> vecA = Vector128.Create(
                colorA.B,
                colorA.G,
                colorA.R,
                colorA.A
            );

            Vector128<int> vecB = Vector128.Create(
                colorB.B,
                colorB.G,
                colorB.R,
                colorB.A
            );

            Vector128<int> errorVec = Vector128.Create((int)errorRange);

            // 计算差值
            Vector128<int> diff = Sse2.Subtract(vecA, vecB);

            Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);  // 获取符号位
            Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);  // 绝对值计算：(x + sign) ^ sign

            // 比较差值是否超过误差范围
            Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

            // 检查所有分量是否都在误差范围内
            return Sse2.MoveMask(cmp.AsSingle()) == 0;
        }

        public static bool ColorAEqualColorB(Color colorA, Color colorB, byte errorR, byte errorG, byte errorB)
        {
            Vector128<int> vecA = Vector128.Create(
                colorA.B,
                colorA.G,
                colorA.R,
                0
            );

            Vector128<int> vecB = Vector128.Create(
                colorB.B,
                colorB.G,
                colorB.R,
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
            Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);  // 绝对值计算：(x + sign) ^ sign

            Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

            // 只检查RGB三个分量（忽略Alpha）
            return (Sse2.MoveMask(cmp.AsSingle()) & 0x7) == 0;
        }
    }

    public static class ColorExtensions
    {
        public static bool EqualsWithError(this Color color, Color other, byte errorRange = 5)
        {
            return ColorComparer.ColorAEqualColorB(color, other, errorRange);
        }

        public static bool EqualsRGBWithError(this Color color, Color other,
            byte errorR = 5, byte errorG = 5, byte errorB = 5)
        {
            return ColorComparer.ColorAEqualColorB(color, other, errorR, errorG, errorB);
        }
    }
}
