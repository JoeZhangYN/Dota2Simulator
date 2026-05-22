namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「时间源」的需求。抽象出来便于测试与回放探针。
/// </summary>
public interface IClock
{
    /// <summary>当前时间，Unix 毫秒。</summary>
    long NowMs { get; }
}
