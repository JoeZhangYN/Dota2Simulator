using System.Threading.Tasks;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 条件委托：返回「条件是否保持触发」。
/// Phase 18 V6a：删除原 (ImageHandle 句柄) 参数 (Phase 6 临时妥协)。
/// 委托方法都是 instance method, 直接走 this._vision.PixelAt / this._vision.Find 端口；
/// 多数旧实现体内是「忽略 句柄 入参 + 重新调 GlobalScreenCapture.GetCurrentHandle()」, 入参冗余。
/// </summary>
public delegate Task<bool> ConditionDelegateBitmap();

/// <summary>
/// 一个条件槽：Active 触发标志 + 关联的图像委托 Probe。
/// 取代 Main.cs 散落的成对字段 _条件N（bool）+ _条件根据图片委托N（委托）。
/// </summary>
public sealed class ConditionSlot
{
    /// <summary>条件是否触发（原 _条件N）。</summary>
    public bool Active { get; set; }

    /// <summary>条件关联的图像委托（原 _条件根据图片委托N）。</summary>
    public ConditionDelegateBitmap? Probe { get; set; }

    /// <summary>复位：清触发标志 + 清委托。</summary>
    public void Clear()
    {
        Active = false;
        Probe = null;
    }
}
