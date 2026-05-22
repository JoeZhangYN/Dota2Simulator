namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 出站端口：领域对「语音播报」的需求。由 Audio BC 的 adapter 实现（System.Speech）。
/// </summary>
public interface ISpeaker
{
    void Speak(string text);
}
