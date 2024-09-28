using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;

namespace Dota2Simulator.PictureProcessing;

/*
 *
 * 安装对应的Nuget包
 Sdcb.PaddleInference
 Sdcb.PaddleOCR
 Sdcb.PaddleOCR.Models.Local
 Sdcb.PaddleInference.runtime.win64.mkl
 Sdcb.PaddleInference.runtime.win64.cu20-sm86-89 选一个，Gpu用下面的，实际上完全没法用。
 OpenCvSharp4.runtime.win
 */
internal abstract class PaddleOcr
{
    private static PaddleOcrAll _PaddleOcrAll;

    private static bool _isStart;

    public static bool 初始化PaddleOcr()
    {
        _PaddleOcrAll = new PaddleOcrAll(LocalFullModels.ChineseV4, PaddleDevice.Mkldnn())
        {
            AllowRotateDetection = false, /* 允许识别有角度的文字 */
            Enable180Classification = false /* 允许识别旋转角度大于90度的文字 */
        };

        _isStart = true;
        return true;
    }

    public static bool 释放PaddleOcr()
    {
        _PaddleOcrAll?.Dispose();
        _PaddleOcrAll = null;
        return true;
    }

    #region 获取图片文字

    public static string 获取图片文字(string path)
    {
        if (!_isStart) _ = 初始化PaddleOcr();

        using Mat src = Cv2.ImRead(path);
        PaddleOcrResult result = _PaddleOcrAll.Run(src);
        return result.Text;
    }

    public static string 获取图片文字(byte[] bts)
    {
        if (!_isStart) _ = 初始化PaddleOcr();

        // Load local file by following code:
        // using (Mat src = Cv2.ImRead(@"J:\Desktop\1.bmp"))
        using Mat src = Cv2.ImDecode(bts, ImreadModes.Color);
        PaddleOcrResult result = _PaddleOcrAll.Run(src);
        return result.Text;
        // Console.WriteLine("Detected all texts: \n" + result.Text);
        //foreach (PaddleOcrResultRegion region in result.Regions)
        //{
        //    Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
        //}
    }

    public static string 获取图片文字(int x, int y, int width, int height)
    {
        return 获取图片文字(PictureProcessing.CaptureScreenAllByte(x, y, width, height));
    }

    #endregion
}