using UnityEngine;
using UnityEngine.Audio;
using System;
using NAudio.Wave;

[RequireComponent(typeof(AudioSource))]
public class AudioRecorder : MonoBehaviour
{
    public int recordingTime = 5;  // 录制音频的时长（单位：秒）
    public AudioMixerGroup outputMixerGroup;
    public string savePath = "Assets/Audio/recordedAudio.wav";
    public string[] availableDevices;
    public string selectedDevice;
    public AudioSource audioSource;
    private string device;
    public AudioClip recordedClip;
    private DateTime recordingStartTime;
    private bool isRecording;
    private WaveInEvent waveIn;
    private WaveFileWriter writer;
    private BufferedWaveProvider bufferedWaveProvider;
    private WaveFormat waveFormat;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (availableDevices != null && availableDevices.Length > 0)
        {
            device = availableDevices[0];
        }
        else
        {
            Debug.LogWarning("No available recording devices found.");
        }
    }

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Recording already in progress.");
            return;
        }

        recordingStartTime = DateTime.Now; // 记录开始录制的时间戳

        waveIn = new WaveInEvent();
        waveIn.DeviceNumber = Array.IndexOf(Microphone.devices, device);
        waveIn.WaveFormat = new WaveFormat(AudioSettings.outputSampleRate, 16, 2); // 双声道的音频格式

        bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
        bufferedWaveProvider.DiscardOnBufferOverflow = true;

        waveIn.DataAvailable += WaveIn_DataAvailable;
        waveIn.RecordingStopped += WaveIn_RecordingStopped;

        writer = new WaveFileWriter(savePath, waveIn.WaveFormat);
        isRecording = true;

        waveIn.StartRecording();
    }

    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("No recording in progress.");
            return;
        }

        isRecording = false;
        waveIn.StopRecording();
    }

    private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        writer.Write(e.Buffer, 0, e.BytesRecorded);
    }

    private void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
    {
        writer.Close();
        writer.Dispose();
    }

    public AudioClip GetRecordedAsset()
    {
        // 使用NAudio读取录制的音频文件并创建Unity的AudioClip对象
        using (var audioFile = new AudioFileReader(savePath))
        {
            var buffer = new float[audioFile.Length];
            audioFile.Read(buffer, 0, buffer.Length);
            var audioClip = AudioClip.Create("RecordedAudioClip", buffer.Length, 2, audioFile.WaveFormat.SampleRate, false);
            audioClip.SetData(buffer, 0);
            return audioClip;
        }
    }
}
