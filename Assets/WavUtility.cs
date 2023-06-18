using UnityEngine;
using System;
using System.IO;

public static class WavUtility
{
    public static void Create(string filePath, short[] audioData, int numChannels, int sampleRate)
    {
        // 檢查參數有效性
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

        // 建立檔案流
        using (FileStream fileStream = File.Open(filePath, FileMode.Create))
        {
            // 計算相關參數
            int bitsPerSample = 16; // 16位
            int byteRate = sampleRate * numChannels * (bitsPerSample / 8);
            int dataSize = audioData.Length * (bitsPerSample / 8);

            // 寫入標頭
            WriteHeader(fileStream, dataSize, numChannels, sampleRate, byteRate);

            // 寫入音訊資料
            WriteData(fileStream, audioData);
        }
    }

    private static void WriteHeader(FileStream fileStream, int dataSize, int numChannels, int sampleRate, int byteRate)
    {
        int bitsPerSample = 16; // 位元深度，預設為 16
        // 寫入 Chunk ID
        byte[] chunkId = new byte[] { 0x52, 0x49, 0x46, 0x46 }; // "RIFF"
        fileStream.Write(chunkId, 0, 4);

        // 寫入 Chunk Size (整個檔案大小減8)
        byte[] chunkSize = BitConverter.GetBytes(dataSize + 36);
        fileStream.Write(chunkSize, 0, 4);

        // 寫入 Format
        byte[] format = new byte[] { 0x57, 0x41, 0x56, 0x45 }; // "WAVE"
        fileStream.Write(format, 0, 4);

        // 寫入 Subchunk1 ID
        byte[] subchunk1Id = new byte[] { 0x66, 0x6D, 0x74, 0x20 }; // "fmt "
        fileStream.Write(subchunk1Id, 0, 4);

        // 寫入 Subchunk1 Size (16，固定值)
        byte[] subchunk1Size = new byte[] { 0x10, 0x00, 0x00, 0x00 };
        fileStream.Write(subchunk1Size, 0, 4);

        // 寫入 Audio Format (1，表示 PCM)
        byte[] audioFormat = new byte[] { 0x01, 0x00 };
        fileStream.Write(audioFormat, 0, 2);

        // 寫入 Number of Channels
        byte[] numChannelsBytes = BitConverter.GetBytes((short)numChannels);
        fileStream.Write(numChannelsBytes, 0, 2);

        // 寫入 Sample Rate
        byte[] sampleRateBytes = BitConverter.GetBytes(sampleRate);
        fileStream.Write(sampleRateBytes, 0, 4);

        // 寫入 Byte Rate
        byte[] byteRateBytes = BitConverter.GetBytes(byteRate);
        fileStream.Write(byteRateBytes, 0, 4);

        // 寫入 Block Align
        byte[] blockAlign = BitConverter.GetBytes((short)(numChannels * (bitsPerSample / 8)));
        fileStream.Write(blockAlign, 0, 2);

        // 寫入 Bits Per Sample
        byte[] bitsPerSampleBytes = BitConverter.GetBytes((short)bitsPerSample);
        fileStream.Write(bitsPerSampleBytes, 0, 2);

        // 寫入 Subchunk2 ID
        byte[] subchunk2Id = new byte[] { 0x64, 0x61, 0x74, 0x61 }; // "data"
        fileStream.Write(subchunk2Id, 0, 4);

        // 寫入 Subchunk2 Size
        byte[] subchunk2Size = BitConverter.GetBytes(dataSize);
        fileStream.Write(subchunk2Size, 0, 4);
    }

    private static void WriteData(FileStream fileStream, short[] audioData)
    {
        byte[] byteData = new byte[audioData.Length * 2 ]; // 16位 = 2個byte

        // 將16位整數轉換成byte數組
        for (int i = 0; i < audioData.Length; i++)
        {
            byteData[i * 2] = (byte)(audioData[i] & 0xFF); // 低位元組
            byteData[i* 2 + 1] = (byte)(audioData[i] >> 8); // 高位元組
        }

        // 寫入音訊資料
        fileStream.Write(byteData, 0, byteData.Length);
    }
    public static AudioClip ToAudioClip(byte[] wavData)
    {
        int headerSize = 44;
        int sampleRateOffset = 24;
        int numChannelsOffset = 22;
        int bitsPerSampleOffset = 34;
        int dataSizeOffset = 40;

        // 解析 WAV 標頭資訊
        int sampleRate = BitConverter.ToInt32(wavData, sampleRateOffset);
        int numChannels = BitConverter.ToInt16(wavData, numChannelsOffset);
        int bitsPerSample = BitConverter.ToInt16(wavData, bitsPerSampleOffset);
        int dataSize = BitConverter.ToInt32(wavData, dataSizeOffset);

        Debug.Log(sampleRate);
        // 創建 AudioClip
        AudioClip audioClip = AudioClip.Create("AudioClip", dataSize / (numChannels * (bitsPerSample / 8)), numChannels, sampleRate, false);

        // 複製音頻資料到 AudioClip
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

            samples[sampleIndex] = value / 32768f; // 正規化到 [-1, 1] 範圍
            sampleOffset += bitsPerSample / 8;
            sampleIndex++;
        }

        audioClip.SetData(samples, 0);

        return audioClip;
    }
}
