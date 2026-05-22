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
    /// 图像句柄
    /// </summary>
    public readonly struct ImageHandle : IEquatable<ImageHandle>
    {
        public int Id { get; }
        public Size Size { get; }
        public long Offset { get; }
        public ImageType Type { get; }

        internal ImageHandle(int id, in Size size, long offset, ImageType type)
        {
            Id = id;
            Size = size;
            Offset = offset;
            Type = type;
        }

        public bool IsValid => Id > 0;
        public bool IsStatic => Type == ImageType.Static;
        public bool IsDynamic => Type == ImageType.Dynamic;

        public static ImageHandle Invalid => new ImageHandle(0, Size.Empty, 0, ImageType.Dynamic);

        public bool Equals(ImageHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is ImageHandle other && Equals(other);
        public override int GetHashCode() => Id;
    }
}
