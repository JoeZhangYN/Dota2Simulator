namespace Dota2Simulator.GameAutomation.Domain.Actuation;

/// <summary>键盘监听产生的触发事件：用户按下了某个键。</summary>
public readonly record struct KeyTrigger(VirtualKey Key);
