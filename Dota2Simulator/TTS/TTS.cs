using System.Speech.Synthesis;

namespace Dota2Simulator.TTS;

/// <summary>
///     <see cref="Tts" /> TTS 类
/// </summary>
internal class Tts
{
    #region 语音播放

    /// <summary>
    ///     TTSs the speak.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="rate">The rate. -10 to 10</param>
    /// <param name="volume">The volume. 0 to 100</param>
    public static void Speak(string str, int rate = 4, int volume = 100)
    {
        SpeechSynthesizer synthesizer = new()
        {
            Rate = rate,
            Volume = volume
        };
        _ = synthesizer.SpeakAsync(str);
    }

    #endregion 语音播放
}