using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using NAudio.Wave;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public int recordingTime = 5;  // 录制音频的时长（单位：秒）
    public AudioMixerGroup outputMixerGroup;
    public string savePath = "Assets/Audio/recordedAudio.wav";
    public string[] availableDevices;
    public string selectedFirstDeviceProperty;
    public string selectedSecDeviceProperty;
    public AudioSource audioSource;
    public AudioSource SFX;
    private string device;
    public AudioClip recordedClip1;
    public AudioClip recordedClip2;
    public DateTime recordingStartTime;
    public DateTime recordingEndTing;
    private bool isRecording;
    private bool isSaved;
    private WaveInEvent waveIn;
    private WaveFileWriter writer;
    private BufferedWaveProvider bufferedWaveProvider;
    private WaveFormat waveFormat;
    private Coroutine RecordCoroutine1, RecordCoroutine2;
    private int outputSampleRate;
    private AudioClip clip;

    private void Start()
    {
        outputSampleRate = AudioSettings.outputSampleRate;
        audioSource = GetComponent<AudioSource>();
    }
    public void StartRecording()
    {

        Debug.Log("Start Recording!");
        recordingStartTime = DateTime.Now;
        // 開始錄製第一個裝置的音頻
        RecordCoroutine1 = StartCoroutine(RecordMicrophone(selectedFirstDeviceProperty, 0));

        // 開始錄製第二個裝置的音頻
        RecordCoroutine2 = StartCoroutine(RecordMicrophone(selectedSecDeviceProperty, 1));
    }


    IEnumerator RecordMicrophone(string deviceName, int deviceIndex)
    {
        // 在主線程中啟動錄製
        yield return StartCoroutine(StartMicrophoneRecording(deviceName, deviceIndex));

        isRecording = true;

        // 等待錄製停止
        while (isRecording)
        {
            yield return null;
        }

        // 在主線程中停止錄製
        yield return StartCoroutine(StopMicrophoneRecording(deviceName, deviceIndex));

        // 將音訊保存成 WAV 檔案
        //if (deviceIndex == 0)
        //{
        //    SaveAudioClipAsWav(recordedClip1, "Microphone1.wav");
        //}
        //else if (deviceIndex == 1)
        //{
        //    SaveAudioClipAsWav(recordedClip2, "Microphone2.wav");
        //}
    }

    IEnumerator StartMicrophoneRecording(string deviceName, int deviceIndex)
    {
        if (deviceIndex == 0)
        {
            Debug.Log(deviceName + " is start recording! OutputSampleRate: " + AudioSettings.outputSampleRate);
            recordedClip1 = Microphone.Start(deviceName, false, 2000, outputSampleRate);
        }
        else if (deviceIndex == 1)
        {
            Debug.Log(deviceName + " is start recording!" + AudioSettings.outputSampleRate);
            recordedClip2 = Microphone.Start(deviceName, false, 2000, outputSampleRate);
        }

        // 等待一幀時間，以確保錄製已經開始
        yield return null;
    }

    IEnumerator StopMicrophoneRecording(string deviceName, int deviceIndex)
    {

        Debug.Log(deviceName + " is end recording!");
        Microphone.End(deviceName);

        // 等待一幀時間，以確保錄製已經停止
        yield return null;
    }
    public async void StopRecording()
    {
        Debug.Log("Record Stopping...");
        // 停止录制
        isRecording = false;
        recordingEndTing = DateTime.Now;
        await SaveAudioClipAsWav();
        LoadAudioClip();
    }



    private async Task SaveAudioClipAsWav()
    {
        int numChannels = 2; // 兩個聲道
        int totalSamples = Mathf.Max(recordedClip1.samples, recordedClip2.samples);

        // 計算裁剪後的音頻長度
        float clipLengthInSeconds = (float)(recordingEndTing - recordingStartTime).TotalSeconds;
        int clipLengthInSamples = (int)(clipLengthInSeconds * outputSampleRate);
        int clippedSamples = Mathf.Min(totalSamples, clipLengthInSamples);

        // 創建一個二維數組來存儲兩個聲道的音訊數據
        float[,] samples = new float[numChannels, clippedSamples];
        // 創建數組來存儲音訊數據
        float[] data1 = new float[recordedClip1.samples];
        float[] data2 = new float[recordedClip2.samples];

        // 獲取每個音訊片段的音訊數據
        recordedClip1.GetData(data1, 0);
        recordedClip2.GetData(data2, 0);

        // 將音訊數據存儲到對應的聲道
        for (int i = 0; i < clippedSamples; i++)
        {
            if (i < recordedClip1.samples)
            {
                samples[0, i] = data1[i]; // 將第一個音訊片段存儲到聲道1
            }

            if (i < recordedClip2.samples)
            {
                samples[1, i] = data2[i]; // 將第二個音訊片段存儲到聲道2
            }
        }

        // 將音訊數據轉換為16位整數數組
        short[] intData = new short[numChannels * clippedSamples];
        for (int i = 0; i < clippedSamples; i++)
        {
            for (int channel = 0; channel < numChannels; channel++)
            {
                intData[i * numChannels + channel] = (short)(samples[channel, i] * 32767);
            }
        }


        // 使用非同步操作保存音訊檔案
        await Task.Run(() =>
        {
            WavUtility.Create(savePath, intData, numChannels, outputSampleRate);
        });

        isSaved = true;
    }

    private void LoadAudioClip()
    {
        audioSource.clip = GetRecordedAsset();
    }

    private AudioClip GetRecordedAsset()
    {
        // 使用NAudio读取录制的音频文件并创建Unity的AudioClip对象
        using (var audioFile = new AudioFileReader(savePath))
        {
            var buffer = new float[audioFile.Length];
            audioFile.Read(buffer, 0, buffer.Length);
            var audioClip = AudioClip.Create("RecordedAudioClip", buffer.Length, 2, audioFile.WaveFormat.SampleRate, false);
            audioClip.SetData(buffer, 0);
            clip = audioClip;
            return audioClip;
        }
    }
    public async Task<AudioClip> GetRecordAsync(string filePath)
    {
        // 读取 WAV 文件的字节数据
        byte[] wavData = await ReadFileBytesAsync(filePath);

        // 创建 AudioClip
        AudioClip audioClip = await Task.Run(() => CreateAudioClipFromWavData(wavData));

        return audioClip;
    }

    private async Task<byte[]> ReadFileBytesAsync(string filePath)
    {
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            byte[] buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, buffer.Length);
            return buffer;
        }
    }

    private AudioClip CreateAudioClipFromWavData(byte[] wavData)
    {
        // 将 WAV 字节数据转换为 AudioClip
        AudioClip audioClip = WavUtility.ToAudioClip(wavData);

        return audioClip;
    }
    public void PlayDing()
    {
        SFX.Play();
    }
    public IEnumerator PlayAudioClip(int channel, float startSeconds, float endSeconds)
    {
        //float startSeconds = (float)(startTime - DateTime.MinValue).TotalSeconds;
        //float endSeconds = (float)(endTime - DateTime.MinValue).TotalSeconds;
        AudioClip audioClip = clip;
        // 创建左声道和右声道数据
        float[] leftBuffer = new float[audioClip.samples];
        float[] rightBuffer = new float[audioClip.samples];
        float[] data = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(data, 0);

        // 提取左声道和右声道数据
        for (int i = 0; i < audioClip.samples; i++)
        {
            int leftIndex = i * audioClip.channels + 0; // 左声道索引
            int rightIndex = i * audioClip.channels + 1; // 右声道索引

            leftBuffer[i] = data[leftIndex];
            rightBuffer[i] = data[rightIndex];
        }

        // 创建单声道音频数据
        int startSample = Mathf.RoundToInt(startSeconds * audioClip.frequency);
        int endSample = Mathf.RoundToInt(endSeconds * audioClip.frequency);
        int numSamples = endSample - startSample;

        Debug.Log(endSeconds);
        float[] monoBuffer = new float[numSamples];

        // 将指定声道数据复制到单声道数据
        float[] channelData = channel == 0 ? leftBuffer : rightBuffer;
        for (int i = startSample; i < endSample; i++)
        {
            monoBuffer[i - startSample] = channelData[i];
        }

        // 创建单声道的 AudioClip
        var monoAudioClip = AudioClip.Create("MonoAudioClip", numSamples, 1, audioClip.frequency, false);
        monoAudioClip.SetData(monoBuffer, 0);

        // 在 AudioSource 上播放单声道 AudioClip
        audioSource.clip = monoAudioClip;
        audioSource.Play();

        // 等待音频播放完毕
        yield return new WaitForSecondsRealtime(endSeconds - startSeconds);

        // 停止播放并清空 AudioClip
        audioSource.Stop();
        audioSource.clip = null;
        yield break;
    }

}
