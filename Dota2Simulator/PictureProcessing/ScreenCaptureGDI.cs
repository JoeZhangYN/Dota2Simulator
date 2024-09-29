using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Dota2Simulator.PictureProcessing;

internal class ScreenCaptureGDI
{
    private const int SRCCOPY = 0x00CC0020;

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

    public static Bitmap CaptureScreen(int width, int height, int x, int y)
    {
        Size screenSize = new(width, height);
        Bitmap screenBitmap = new(screenSize.Width, screenSize.Height);

        using (Graphics g = Graphics.FromImage(screenBitmap))
        {
            IntPtr hdcDest = g.GetHdc();
            IntPtr hdcSrc = GetWindowDC(GetDesktopWindow());

            _ = BitBlt(hdcDest, 0, 0, screenSize.Width, screenSize.Height, hdcSrc, x, y, SRCCOPY);

            g.ReleaseHdc(hdcDest);
            _ = ReleaseDC(GetDesktopWindow(), hdcSrc);
        }

        return screenBitmap;
    }
}