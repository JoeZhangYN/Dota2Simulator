using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Vision.Adapters;

/// <summary>
/// <see cref="ITemplateRepository"/> 的实现：按名称取图片模板。
/// 模板名形如 "物品.以太"，对应嵌入资源 Dota2Simulator.Picture_Dota2.物品.以太.bmp。
///
/// 取模板时经 <see cref="LazyImageLoader"/> 预热图像句柄——首次取用即把嵌入资源解码并缓存，
/// 确保 <see cref="RustVisionAdapter"/> 找图时数据就绪，并尽早暴露缺失资源。
/// </summary>
public sealed class EmbeddedResourceTemplateRepository : ITemplateRepository
{
    /// <summary>取模板。Template 当前仅持标识名，图像数据由 LazyImageLoader 按名缓存。</summary>
    public Template Get(string name)
    {
        // 预热懒加载：把模板对应的 .bmp 嵌入资源解码为图像句柄并缓存。
        LazyImageLoader.GetImage(name);
        return new Template(name);
    }
}
