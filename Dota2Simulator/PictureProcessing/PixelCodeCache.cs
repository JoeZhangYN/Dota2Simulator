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
    /// 像素代码缓存 - 用于缓存特定位置的颜色信息
    /// </summary>
    public static class PixelCodeCache
    {
        private static readonly ConcurrentDictionary<string, PixelInfo> _pixelCache = new();
        private const int MAX_CACHE_SIZE = 100000;

        public struct PixelInfo
        {
            public Point Position { get; set; }
            public uint ColorCode { get; set; }
            public string Description { get; set; }
            public DateTime CachedTime { get; set; }
        }

        public static int CacheSize => _pixelCache.Count;

        /// <summary>
        /// 缓存特定位置的颜色信息
        /// </summary>
        public static void CachePixel(string key, Point position, uint colorCode, string description = "")
        {
            if (_pixelCache.Count > MAX_CACHE_SIZE)
            {
                ClearOldEntries();
            }

            _pixelCache[key] = new PixelInfo
            {
                Position = position,
                ColorCode = colorCode,
                Description = description,
                CachedTime = DateTime.Now
            };
        }

        /// <summary>
        /// 获取缓存的像素信息
        /// </summary>
        public static PixelInfo? GetPixelInfo(string key)
        {
            return _pixelCache.TryGetValue(key, out var info) ? info : null;
        }

        /// <summary>
        /// 验证缓存的像素是否仍然匹配
        /// </summary>
        public static bool ValidatePixel(string key, in ImageHandle handle, byte tolerance = 0)
        {
            if (!_pixelCache.TryGetValue(key, out var info))
                return false;

            uint currentColor = ImageManager.GetPixel(handle, info.Position.X, info.Position.Y);
            return PixelCodeCache.ComparePixelCodes(currentColor, info.ColorCode, tolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ColorToCode(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color CodeToColor(uint code)
        {
            return Color.FromArgb(
                (int)((code >> 24) & 0xFF),
                (int)((code >> 16) & 0xFF),
                (int)((code >> 8) & 0xFF),
                (int)(code & 0xFF)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ComparePixelCodes(uint code1, uint code2, byte tolerance)
        {
            if (tolerance == 0)
                return code1 == code2;

            byte r1 = (byte)((code1 >> 16) & 0xFF);
            byte g1 = (byte)((code1 >> 8) & 0xFF);
            byte b1 = (byte)(code1 & 0xFF);

            byte r2 = (byte)((code2 >> 16) & 0xFF);
            byte g2 = (byte)((code2 >> 8) & 0xFF);
            byte b2 = (byte)(code2 & 0xFF);

            return Math.Abs(r1 - r2) <= tolerance &&
                   Math.Abs(g1 - g2) <= tolerance &&
                   Math.Abs(b1 - b2) <= tolerance;
        }

        public static void ClearOldEntries()
        {
            var now = DateTime.Now;
            var toRemove = _pixelCache
                .Where(kvp => (now - kvp.Value.CachedTime).TotalMinutes > 30)
                .Select(kvp => kvp.Key)
                .Take(_pixelCache.Count / 3)
                .ToList();

            foreach (var key in toRemove)
            {
                _pixelCache.TryRemove(key, out _);
            }
        }

        public static void Clear()
        {
            _pixelCache.Clear();
        }
    }
}
