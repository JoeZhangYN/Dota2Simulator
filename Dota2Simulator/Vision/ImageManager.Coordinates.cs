using System.Drawing;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// ImageManager 拆分 partial：坐标有效性校验。
    /// </summary>
    public static partial class ImageManager
    {
        #region 检测位置是否有效

        private static readonly Point 无效坐标 = new(245760, 143640);

        public static bool 是否无效位置(Point? 位置)
        {
            return 位置 == null ||
           位置 == 无效坐标 ||
           位置.Value.X <= 0 ||
           位置.Value.Y <= 0;
        }

        #endregion
    }
}
