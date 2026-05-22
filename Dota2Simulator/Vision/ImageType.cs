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
using Tuple = Dota2Simulator.Vision.ImageFinder.Tuple;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// 图像类型枚举
    /// </summary>
    public enum ImageType : byte
    {
        Dynamic = 0,    // 动态图像（截图等会变化的）
        Static = 1      // 静态图像（特征图片等不变的）
    }

    ///// <summary>
    ///// 内联数组缓冲区 - 用于减少内存碎片
    ///// </summary>
    //[InlineArray(1024 * 1024)]  // 1MB 内联数组
    //public struct InlineBuffer
    //{
    //    private byte _element0;
    //}
}
