namespace Dota2Simulator.Diagnostics;

/// <summary>
/// 一条端口调用记录。<see cref="SeqNo"/> 单调递增用于稳定排序，
/// <see cref="TimestampMs"/> 是会话开始后的相对毫秒、用于时序分析。
/// </summary>
public readonly record struct ProbeEvent(
    long SeqNo,
    long TimestampMs,
    string Port,
    string Method,
    string Args)
{
    /// <summary>定长前缀 + 端口调用，便于两份日志逐行 diff。</summary>
    public override string ToString()
        => $"{SeqNo:D6} +{TimestampMs,8}ms  {Port}.{Method}({Args})";
}
