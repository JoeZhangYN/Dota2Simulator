using Dota2Simulator.GameAutomation.Domain.Perception;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「图片模板获取」的需求。
/// 由 AssetCatalog BC 的 adapter 实现（嵌入资源懒加载）。
/// </summary>
public interface ITemplateRepository
{
    /// <summary>按名称获取图片模板。</summary>
    Template Get(string name);
}
