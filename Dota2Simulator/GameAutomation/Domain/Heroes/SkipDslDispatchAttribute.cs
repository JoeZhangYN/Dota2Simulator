using System;

namespace Dota2Simulator.GameAutomation.Domain.Heroes;

/// <summary>
/// Phase 25A C5: DS0002 escape-hatch — 标记 HeroStrategyBase 子类的 override OnKeyAsync 跳过 DS0002 lint 强制.
/// 用法: 当业务真有 DSL 无法表达的特殊形态 (跨 trigger lock 协议 / 复杂状态机 / 多步异步链), 标该 attribute 显式声明.
/// 触发时机应极少 — 实测 Phase 25A C2 收尾后 92/92 hero 0 override; attribute 仅作未来扩展的安全阀.
/// 标记后 PR review 时应明确说明"为何 DSL 不能表达 + 是否应扩 DSL 容纳" — 不应作绕过 lint 的常规手段.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class SkipDslDispatchAttribute : Attribute
{
}
