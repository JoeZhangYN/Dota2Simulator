using System;
using NAudio.Wave;

namespace Dota2Simulator.Audio
{
    internal class AudioAttack
    {
        private static float[] attackSoundSample;
        private static WaveInEvent waveIn;

        private static void MainVoid(string[] args)
        {
            // 加载攻击音效
            LoadAttackSound("attack_sound.wav");

            waveIn = new WaveInEvent();
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.WaveFormat = new WaveFormat(44100, 1);
            waveIn.StartRecording();

            Console.WriteLine("Listening for attack sound...");
            _ = Console.ReadLine(); // 保持程序运行
            waveIn.StopRecording();
        }

        private static void LoadAttackSound(string filePath)
        {
            using AudioFileReader reader = new(filePath);
            ISampleProvider sampleProvider = reader.ToSampleProvider();
            float[] buffer = new float[reader.Length / sizeof(float)];
            _ = sampleProvider.Read(buffer, 0, buffer.Length);
            attackSoundSample = buffer;
        }

        private static void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            // 将音频数据转换为浮点样本
            float[] buffer = new float[e.BytesRecorded / sizeof(float)];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            // 比较样本
            if (IsAttackSound(buffer))
            {
                Console.WriteLine("Attack sound detected");
            }
            // 处理检测到攻击音效后的逻辑
        }

        private static bool IsAttackSound(float[] buffer)
        {
            // 简单的波形对比示例
            // 可以使用更复杂的算法进行特征提取和匹配
            if (buffer.Length < attackSoundSample.Length)
            {
                return false;
            }

            for (int i = 0; i <= buffer.Length - attackSoundSample.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < attackSoundSample.Length; j++)
                {
                    if (Math.Abs(buffer[i + j] - attackSoundSample[j]) > 0.01f)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return true;
                }
            }

            return false;
        }
    }
}