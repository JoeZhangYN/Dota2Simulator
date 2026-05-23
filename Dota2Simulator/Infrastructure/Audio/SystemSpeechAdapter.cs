using System.Speech.Synthesis;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Audio.Adapters;

/// <summary>ISpeaker 的 System.Speech 实现。</summary>
public sealed class SystemSpeechAdapter : ISpeaker
{
    public void Speak(string text)
    {
        SpeechSynthesizer synthesizer = new()
        {
            Rate = 4,
            Volume = 100,
        };
        _ = synthesizer.SpeakAsync(text);
    }
}
