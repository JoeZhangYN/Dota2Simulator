using System.Drawing;
using System.IO;
using Tesseract;

namespace Dota2Simulator
{
    public class OCR
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="bp">识别用的图片</param>
        /// <returns></returns>
        public static string 识别英文文字(Bitmap bp)
        {
            using var ocr = new TesseractEngine(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tessdata"), "eng", EngineMode.LstmOnly);
            var pix = PixConverter.ToPix(bp);
            using var page = ocr.Process(pix);
            return page.GetText();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="bp">识别用的图片</param>
        /// <returns></returns>
        public static string 识别中文文字(Bitmap bp)
        {
            using var ocr = new TesseractEngine(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tessdata"), "chi_sim", EngineMode.LstmOnly);
            var pix = PixConverter.ToPix(bp);
            using var page = ocr.Process(pix);
            return page.GetText();
        }
    }
}