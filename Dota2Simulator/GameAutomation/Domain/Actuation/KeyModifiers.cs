namespace Dota2Simulator.GameAutomation.Domain.Actuation;

/// <summary>按键修饰键标志，领域中性，取代 System.Windows.Forms.Keys 的 Alt/Control/Shift。</summary>
[System.Flags]
public enum KeyModifiers { None = 0, Alt = 1, Control = 2, Shift = 4 }
