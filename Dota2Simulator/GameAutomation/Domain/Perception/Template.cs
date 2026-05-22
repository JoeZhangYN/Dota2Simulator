namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// 待匹配的图片模板（needle）。Phase 1 仅持标识；Phase 3 Vision 端口化时
/// 关联实际图像数据，统一取代散落的 ImageHandle。
/// </summary>
public readonly record struct Template(string Name);
