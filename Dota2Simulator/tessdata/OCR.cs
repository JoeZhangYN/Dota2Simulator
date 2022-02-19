using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Tesseract;

namespace Dota2Simulator;

public class OCR
{
    /// <summary>
    /// </summary>
    /// <param name="bp">识别用的图片</param>
    /// <returns></returns>
    public static string 识别英文文字(Bitmap bitmap)
    {
        Bitmap bp = new Bitmap(bitmap);
        bp = ToGray(bp);
        bp = ConvertTo1Bpp1(bp);
        using var ocr = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"), "eng",
            EngineMode.LstmOnly);
        using var pix = PixConverter.ToPix(bitmap);
        using var page = ocr.Process(pix);
        string str = page.GetText();
        bp.Dispose();
        return str;
    }

    public static string 识别英文数字(Bitmap bitmap)
    {
        Bitmap bp = new Bitmap(bitmap);
        bp = ToGray(bp);
        bp = ConvertTo1Bpp1(bp);
        using var ocr = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"), "eng",
            EngineMode.LstmOnly);
        using var pix = PixConverter.ToPix(bitmap);
        ocr.SetVariable("tessedit_char_whitelist", "1234567890:.s()");
        using var page = ocr.Process(pix);
        string str = page.GetText();
        bp.Dispose();
        return str;
    }

    /// <summary>
    /// </summary>
    /// <param name="bitmap">识别用的图片</param>
    /// <returns></returns>
    public static string 识别中文文字(Bitmap bitmap)
    {
        Bitmap bp = new Bitmap(bitmap);
        bp = ToGray(bp);
        bp = ConvertTo1Bpp1(bp);
        using var ocr = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"), "chi_sim",
            EngineMode.LstmOnly);
        using var pix = PixConverter.ToPix(bitmap);
        using var page = ocr.Process(pix);
        string str = page.GetText();
        bp.Dispose();
        return str;
    }

    /// <summary>
    /// 图像灰度化
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static Bitmap ToGray(Bitmap bmp)
    {
        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < bmp.Height; j++)
            {
                //获取该点的像素的RGB的颜色
                Color color = bmp.GetPixel(i, j);
                //利用公式计算灰度值
                int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                Color newColor = Color.FromArgb(gray, gray, gray);
                bmp.SetPixel(i, j, newColor);
            }
        }
        return bmp;
    }

    /// <summary>
    /// 图像二值化1：取图片的平均灰度作为阈值，低于该值的全都为0，高于该值的全都为255
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static Bitmap ConvertTo1Bpp1(Bitmap bmp)
    {
        int average = 0;
        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < bmp.Height; j++)
            {
                Color color = bmp.GetPixel(i, j);
                average += color.B;
            }
        }
        average = (int)average / (bmp.Width * bmp.Height);

        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < bmp.Height; j++)
            {
                //获取该点的像素的RGB的颜色
                Color color = bmp.GetPixel(i, j);
                int value = 255 - color.B;
                Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                bmp.SetPixel(i, j, newColor);
            }
        }
        return bmp;
    }

    /// <summary>
    /// 图像二值化2
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    public static Bitmap ConvertTo1Bpp2(Bitmap img)
    {
        int w = img.Width;
        int h = img.Height;
        Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
        BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
        for (int y = 0; y < h; y++)
        {
            byte[] scan = new byte[(w + 7) / 8];
            for (int x = 0; x < w; x++)
            {
                Color c = img.GetPixel(x, y);
                if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
            }
            Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
        }
        return bmp;
    }
}