using UnityEngine;
using System;
using System.IO;

public static class WavUtility
{
    public static void Create(string filePath, short[] audioData, int numChannels, int sampleRate)
    {
        // �ˬd�ѼƦ��ĩ�
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.");
        }

        if (audioData == null || audioData.Length == 0)
        {
            throw new ArgumentException("Audio data cannot be null or empty.");
        }

        if (numChannels <= 0)
        {
            throw new ArgumentException("Number of channels must be greater than 0.");
        }

        if (sampleRate <= 0)
        {
            throw new ArgumentException("Sample rate must be greater than 0.");
        }

        // �إ��ɮ׬y
        using (FileStream fileStream = File.Open(filePath, FileMode.Create))
        {
            // �p������Ѽ�
            int bitsPerSample = 16; // 16��
            int byteRate = sampleRate * numChannels * (bitsPerSample / 8);
            int dataSize = audioData.Length * (bitsPerSample / 8);

            // �g�J���Y
            WriteHeader(fileStream, dataSize, numChannels, sampleRate, byteRate);

            // �g�J���T���
            WriteData(fileStream, audioData);
        }
    }

    private static void WriteHeader(FileStream fileStream, int dataSize, int numChannels, int sampleRate, int byteRate)
    {
        int bitsPerSample = 16; // �줸�`�סA�w�]�� 16
        // �g�J Chunk ID
        byte[] chunkId = new byte[] { 0x52, 0x49, 0x46, 0x46 }; // "RIFF"
        fileStream.Write(chunkId, 0, 4);

        // �g�J Chunk Size (����ɮפj�p��8)
        byte[] chunkSize = BitConverter.GetBytes(dataSize + 36);
        fileStream.Write(chunkSize, 0, 4);

        // �g�J Format
        byte[] format = new byte[] { 0x57, 0x41, 0x56, 0x45 }; // "WAVE"
        fileStream.Write(format, 0, 4);

        // �g�J Subchunk1 ID
        byte[] subchunk1Id = new byte[] { 0x66, 0x6D, 0x74, 0x20 }; // "fmt "
        fileStream.Write(subchunk1Id, 0, 4);

        // �g�J Subchunk1 Size (16�A�T�w��)
        byte[] subchunk1Size = new byte[] { 0x10, 0x00, 0x00, 0x00 };
        fileStream.Write(subchunk1Size, 0, 4);

        // �g�J Audio Format (1�A��� PCM)
        byte[] audioFormat = new byte[] { 0x01, 0x00 };
        fileStream.Write(audioFormat, 0, 2);

        // �g�J Number of Channels
        byte[] numChannelsBytes = BitConverter.GetBytes((short)numChannels);
        fileStream.Write(numChannelsBytes, 0, 2);

        // �g�J Sample Rate
        byte[] sampleRateBytes = BitConverter.GetBytes(sampleRate);
        fileStream.Write(sampleRateBytes, 0, 4);

        // �g�J Byte Rate
        byte[] byteRateBytes = BitConverter.GetBytes(byteRate);
        fileStream.Write(byteRateBytes, 0, 4);

        // �g�J Block Align
        byte[] blockAlign = BitConverter.GetBytes((short)(numChannels * (bitsPerSample / 8)));
        fileStream.Write(blockAlign, 0, 2);

        // �g�J Bits Per Sample
        byte[] bitsPerSampleBytes = BitConverter.GetBytes((short)bitsPerSample);
        fileStream.Write(bitsPerSampleBytes, 0, 2);

        // �g�J Subchunk2 ID
        byte[] subchunk2Id = new byte[] { 0x64, 0x61, 0x74, 0x61 }; // "data"
        fileStream.Write(subchunk2Id, 0, 4);

        // �g�J Subchunk2 Size
        byte[] subchunk2Size = BitConverter.GetBytes(dataSize);
        fileStream.Write(subchunk2Size, 0, 4);
    }

    private static void WriteData(FileStream fileStream, short[] audioData)
    {
        byte[] byteData = new byte[audioData.Length * 2 ]; // 16�� = 2��byte

        // �N16�����ഫ��byte�Ʋ�
        for (int i = 0; i < audioData.Length; i++)
        {
            byteData[i * 2] = (byte)(audioData[i] & 0xFF); // �C�줸��
            byteData[i* 2 + 1] = (byte)(audioData[i] >> 8); // ���줸��
        }

        // �g�J���T���
        fileStream.Write(byteData, 0, byteData.Length);
    }
    public static AudioClip ToAudioClip(byte[] wavData)
    {
        int headerSize = 44;
        int sampleRateOffset = 24;
        int numChannelsOffset = 22;
        int bitsPerSampleOffset = 34;
        int dataSizeOffset = 40;

        // �ѪR WAV ���Y��T
        int sampleRate = BitConverter.ToInt32(wavData, sampleRateOffset);
        int numChannels = BitConverter.ToInt16(wavData, numChannelsOffset);
        int bitsPerSample = BitConverter.ToInt16(wavData, bitsPerSampleOffset);
        int dataSize = BitConverter.ToInt32(wavData, dataSizeOffset);

        Debug.Log(sampleRate);
        // �Ы� AudioClip
        AudioClip audioClip = AudioClip.Create("AudioClip", dataSize / (numChannels * (bitsPerSample / 8)), numChannels, sampleRate, false);

        // �ƻs���W��ƨ� AudioClip
        float[] samples = new float[dataSize / (bitsPerSample / 8)];
        int sampleOffset = headerSize;
        int sampleIndex = 0;

        while (sampleOffset < dataSize + headerSize)
        {
            int value = 0;

            if (bitsPerSample == 16)
            {
                value = (short)(wavData[sampleOffset + 1] << 8 | wavData[sampleOffset]);
            }
            else if (bitsPerSample == 8)
            {
                value = wavData[sampleOffset];
            }

            samples[sampleIndex] = value / 32768f; // ���W�ƨ� [-1, 1] �d��
            sampleOffset += bitsPerSample / 8;
            sampleIndex++;
        }

        audioClip.SetData(samples, 0);

        return audioClip;
    }
}
