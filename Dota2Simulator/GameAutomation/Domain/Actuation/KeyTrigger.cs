namespace Dota2Simulator.GameAutomation.Domain.Actuation;

/// <summary>键盘监听产生的触发事件：用户按下了某个键（含修饰键）。</summary>
public readonly record struct KeyTrigger(VirtualKey Key, KeyModifiers Modifiers)
{
    /// <summary>无修饰键的便捷构造——让旧 <c>new KeyTrigger(key)</c> 调用点不破。</summary>
    public KeyTrigger(VirtualKey key) : this(key, KeyModifiers.None) { }
}
