using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Diagnostics;

/// <summary>
/// 六边形重写回归探针。在端口边界（<see cref="IInputExecutor"/> / <see cref="IScreenVision"/>）
/// 以装饰器拦截调用，把调用序列录为文本日志。重构前后对同一操作序列各录一次、
/// diff 两份日志即回归判据——本项目无自动化测试，这是 Phase 4 的回归安全网。
///
/// 默认关闭（<see cref="Enabled"/>=false）。装饰器常驻输入路径，关闭时每次调用仅多一次
/// volatile bool 读、不构造参数字符串，开销可忽略。冒烟测试前调 <see cref="BeginSession"/>
/// 开录，结束调 <see cref="EndSession"/> 落盘。
/// </summary>
public static class RecordReplayProbe
{
    private static readonly ConcurrentQueue<ProbeEvent> _events = new();
    private static readonly Stopwatch _clock = Stopwatch.StartNew();
    private static long _seq;
    private static volatile bool _enabled;
    private static string _sessionName = "default";

    /// <summary>探针总开关。false 时 <see cref="Record"/> 直接返回。</summary>
    public static bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    /// <summary>日志输出目录，默认 程序目录\探针日志\。</summary>
    public static string OutputDirectory { get; set; }
        = Path.Combine(AppContext.BaseDirectory, "探针日志");

    /// <summary>开始一次录制会话：清空缓冲、重置时钟与序号、开启探针。</summary>
    public static void BeginSession(string name)
    {
        _events.Clear();
        Interlocked.Exchange(ref _seq, 0);
        _sessionName = string.IsNullOrWhiteSpace(name) ? "default" : name.Trim();
        _clock.Restart();
        _enabled = true;
    }

    /// <summary>
    /// 结束会话、关闭探针并把缓冲 dump 到文本文件。
    /// 返回日志文件路径；未录到任何事件则返回 null。
    /// </summary>
    public static string? EndSession()
    {
        _enabled = false;
        if (_events.IsEmpty) return null;

        Directory.CreateDirectory(OutputDirectory);
        string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string path = Path.Combine(OutputDirectory, $"{_sessionName}_{stamp}.log");

        var sb = new StringBuilder();
        sb.Append("# 探针会话: ").AppendLine(_sessionName);
        sb.Append("# 录制时间: ").AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.Append("# 事件数: ").AppendLine(_events.Count.ToString());
        sb.AppendLine();
        foreach (ProbeEvent e in _events)
            sb.AppendLine(e.ToString());

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        return path;
    }

    /// <summary>记录一条端口调用。探针关闭时直接返回。</summary>
    public static void Record(string port, string method, string args)
    {
        if (!_enabled) return;
        long seq = Interlocked.Increment(ref _seq);
        _events.Enqueue(new ProbeEvent(seq, _clock.ElapsedMilliseconds, port, method, args));
    }

    /// <summary>
    /// 用探针装饰器包裹 <see cref="IInputExecutor"/>。总是返回装饰器——
    /// 因调用方通常把结果存为 static readonly 字段，不能依赖包裹时刻的开关状态。
    /// </summary>
    public static IInputExecutor Wrap(IInputExecutor inner) => new ProbeInputExecutor(inner);

    /// <summary>
    /// 用探针装饰器包裹 <see cref="IScreenVision"/>。语义同 <see cref="Wrap(IInputExecutor)"/>。
    /// </summary>
    public static IScreenVision Wrap(IScreenVision inner) => new ProbeScreenVision(inner);
}
