using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Dota2Simulator.PictureProcessing.Capture
{
    internal class ScreenCaptureGDI
    {
        private const int SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(nint hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
            nint hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll")]
        private static extern nint GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern nint GetWindowDC(nint hWnd);

        [DllImport("user32.dll")]
        private static extern nint ReleaseDC(nint hWnd, nint hDC);

        public static Bitmap CaptureScreen(int width, int height, int x, int y)
        {
            Size screenSize = new(width, height);
            Bitmap screenBitmap = new(screenSize.Width, screenSize.Height);

            using (Graphics g = Graphics.FromImage(screenBitmap))
            {
                nint hdcDest = g.GetHdc();
                nint hdcSrc = GetWindowDC(GetDesktopWindow());

                _ = BitBlt(hdcDest, 0, 0, screenSize.Width, screenSize.Height, hdcSrc, x, y, SRCCOPY);

                g.ReleaseHdc(hdcDest);
                _ = ReleaseDC(GetDesktopWindow(), hdcSrc);
            }

            return screenBitmap;
        }
    }
}