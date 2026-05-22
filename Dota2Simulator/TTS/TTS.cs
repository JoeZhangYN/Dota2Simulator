using Dota2Simulator.Audio.Adapters;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.TTS
{
    /// <summary>
    /// 语音播报门面。Strangler Fig 过渡形态：保留原静态 API 供 ~50 处业务调用，
    /// 内部转发到 ISpeaker 端口。Phase 4 业务层直连端口后，本门面于 Phase 6 删除。
    /// </summary>
    internal class TTS
    {
        private static readonly ISpeaker _speaker = new SystemSpeechAdapter();

        /// <summary>
        ///     语音播报。rate / volume 参数保留以兼容既有签名，
        ///     当前所有调用均使用默认值。
        /// </summary>
        /// <param name="str">播报文本。</param>
        /// <param name="rate">语速 -10~10。</param>
        /// <param name="volume">音量 0~100。</param>
        public static void Speak(string str, int rate = 4, int volume = 100) => _speaker.Speak(str);
    }
}
