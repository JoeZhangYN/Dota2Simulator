using System.Speech.Synthesis;

namespace Dota2Simulator;

/// <summary>
///     <see cref="TTS" /> TTS 类
/// </summary>
public class TTS
{
    #region 语音播放

    /// <summary>
    ///     TTSs the speak.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="rate">The rate. -10 to 10</param>
    /// <param name="volume">The volume. 0 to 100</param>
    /// <param name="index">The index.</param>
    /// <param name="limitTime">The limit time.</param>
    public static void Speak(string str, int rate = 4, int volume = 100)
    {
        var synthesizer = new SpeechSynthesizer
        {
            Rate = rate,
            Volume = volume
        };
        synthesizer.SpeakAsync(str);
    }

    #endregion 语音播放
}