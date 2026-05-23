using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// ImageManager 拆分 partial：Bitmap 互转 + 保存为文件。
    /// </summary>
    public static partial class ImageManager
    {
        /// <summary>
        /// 从Bitmap获取数据
        /// </summary>
        public static unsafe byte[] GetBitmapData(Bitmap bitmap)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                var data = new byte[bitmap.Width * bitmap.Height * 4];
                var scan0 = (byte*)bitmapData.Scan0;

                // 使用 SIMD 优化复制
                if (bitmapData.Stride == bitmap.Width * 4)
                {
                    // 连续内存，直接复制
                    Buffer.MemoryCopy(scan0, (byte*)Unsafe.AsPointer(ref data[0]),
                        data.Length, data.Length);
                }
                else
                {
                    // 有 padding，逐行复制
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Buffer.MemoryCopy(scan0 + y * bitmapData.Stride,
                            (byte*)Unsafe.AsPointer(ref data[y * bitmap.Width * 4]),
                            bitmap.Width * 4, bitmap.Width * 4);
                    }
                }

                return data;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// 优化的保存图像方法
        /// </summary>
        public static unsafe bool SaveImage(in ImageHandle handle, string filePath, Rectangle? region = null)
        {
            if (!_images.TryGetValue(handle.Id, out var metadata))
                return false;

            try
            {
                // 直接使用 GetImageData 方法获取所有需要的信息
                var (ptr, length, size) = GetImageData(in handle);

                // 确定保存区域
                Rectangle saveRegion;
                if (region.HasValue)
                {
                    // 验证并调整区域边界
                    var rect = region.Value;
                    int maxWidth = (int)size.x;
                    int maxHeight = (int)size.y;

                    saveRegion = new Rectangle(
                        Math.Max(0, Math.Min(rect.X, maxWidth)),
                        Math.Max(0, Math.Min(rect.Y, maxHeight)),
                        Math.Max(1, Math.Min(rect.Width, maxWidth - Math.Max(0, rect.X))),
                        Math.Max(1, Math.Min(rect.Height, maxHeight - Math.Max(0, rect.Y)))
                    );

                    // 如果调整后的区域无效，返回false
                    if (saveRegion.Width <= 0 || saveRegion.Height <= 0)
                    {
                        _logger.Error($"无效的保存区域: {rect}，图像尺寸: {size.x}x{size.y}");
                        return false;
                    }
                }
                else
                {
                    // 保存整个图像
                    saveRegion = new Rectangle(0, 0, (int)size.x, (int)size.y);
                }

                using (var bitmap = new Bitmap(saveRegion.Width, saveRegion.Height, PixelFormat.Format32bppArgb))
                {
                    var rect = new Rectangle(0, 0, saveRegion.Width, saveRegion.Height);
                    var bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    try
                    {
                        byte* src = (byte*)ptr;
                        byte* dst = (byte*)bitmapData.Scan0;
                        int srcWidth = (int)size.x;
                        int dstWidth = saveRegion.Width;
                        int dstHeight = saveRegion.Height;
                        int bytesPerPixel = 4; // ARGB

                        // 计算源图像中区域起始位置
                        int srcStartX = saveRegion.X;
                        int srcStartY = saveRegion.Y;

                        // 使用并行复制提高大图像区域的保存速度
                        if (dstHeight > 100)
                        {
                            // 大区域使用并行复制
                            Parallel.For(0, dstHeight, y =>
                            {
                                byte* srcRow = src + ((srcStartY + y) * srcWidth + srcStartX) * bytesPerPixel;
                                byte* dstRow = dst + y * bitmapData.Stride;
                                Buffer.MemoryCopy(srcRow, dstRow, dstWidth * bytesPerPixel, dstWidth * bytesPerPixel);
                            });
                        }
                        else
                        {
                            // 小区域使用顺序复制
                            for (int y = 0; y < dstHeight; y++)
                            {
                                byte* srcRow = src + ((srcStartY + y) * srcWidth + srcStartX) * bytesPerPixel;
                                byte* dstRow = dst + y * bitmapData.Stride;
                                Buffer.MemoryCopy(srcRow, dstRow, dstWidth * bytesPerPixel, dstWidth * bytesPerPixel);
                            }
                        }
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    // 根据扩展名保存
                    var extension = Path.GetExtension(filePath).ToLower(System.Globalization.CultureInfo.CurrentCulture);
                    var format = extension switch
                    {
                        ".bmp" => ImageFormat.Bmp,
                        ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                        ".png" => ImageFormat.Png,
                        ".gif" => ImageFormat.Gif,
                        ".tiff" or ".tif" => ImageFormat.Tiff,
                        ".ico" => ImageFormat.Icon,
                        _ => ImageFormat.Bmp
                    };

                    // 对于JPEG格式，可以设置质量
                    if (format == ImageFormat.Jpeg)
                    {
                        var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                        if (jpegEncoder == null)
                        {
                            bitmap.Save(filePath, format);
                        }
                        else
                        {
                            var encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 90L); // 90% 质量
                            bitmap.Save(filePath, jpegEncoder, encoderParams);
                        }
                    }
                    else
                    {
                        bitmap.Save(filePath, format);
                    }
                }

                // 更新访问时间
                metadata.LastAccessTime = DateTime.Now;
                _images[handle.Id] = metadata;

                string regionInfo = region.HasValue ? $"区域: {saveRegion}" : "完整图像";
                _logger.Info($"图像已保存: {filePath} (类型: {handle.Type}, {regionInfo}, 保存尺寸: {saveRegion.Width}x{saveRegion.Height})");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"保存图像失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取图像编码器
        /// </summary>
        private static ImageCodecInfo? GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
