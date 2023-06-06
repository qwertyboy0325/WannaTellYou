using System.IO;
using System.IO.Pipes;
using System.IO.MemoryMappedFiles;
using System.Text;
using UnityEngine;

public class IPCServer : MonoBehaviour
{
    private NamedPipeServerStream serverStream;
    private MemoryMappedFile memoryMappedFile;

    public delegate void MessageReceivedDelegate(string message);
    public static event MessageReceivedDelegate OnMessageReceived;

    private void Start()
    {
        serverStream = new NamedPipeServerStream("AudioIPC");
        memoryMappedFile = MemoryMappedFile.CreateNew("AudioIPCSharedMemory", 200000000);
        serverStream.WaitForConnection();

        // 在另一執行緒中持續接收訊息
        new System.Threading.Thread(ReceiveMessages).Start();
    }

    private void ReceiveMessages()
    {
        StreamReader reader = new StreamReader(serverStream);

        while (true)
        {
            string message = reader.ReadLine();
            if (message != null)
            {
                // 傳遞訊息給事件訂閱者
                OnMessageReceived?.Invoke(message);
            }
        }
    }

    private void WriteToSharedMemory(string message)
    {
        using (MemoryMappedViewAccessor accessor = memoryMappedFile.CreateViewAccessor())
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            accessor.WriteArray(0, data, 0, data.Length);
        }
    }

    private void OnDestroy()
    {
        serverStream.Close();
        memoryMappedFile.Dispose();
    }

    // 其他程式存取共享記憶體的方法
    public static AudioClip ReadSharedMemoryAsAudioClip()
    {
        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("AudioIPCSharedMemory"))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                long byteCount = accessor.Capacity;
                byte[] byteData = new byte[byteCount];
                accessor.ReadArray(0, byteData, 0, (int)byteCount);

                // 轉換整數型態的音訊資料為浮點數型態
                float[] floatData = new float[byteCount / sizeof(float)];
                for (int i = 0; i < floatData.Length; i++)
                {
                    try
                    {
                        int intValue = System.BitConverter.ToInt32(byteData, i * sizeof(int));
                        floatData[i] = IntToFloatSample(intValue);
                    }
                    catch (System.Exception ex)
                    {
                        // 處理例外狀況，例如記錄錯誤訊息或回傳預設的音訊資料
                        Debug.LogError("Error converting memory content to float: " + ex.Message);
                        // 在這裡可以回傳預設的 AudioClip 或其他適當的處理方式
                        return null;
                    }
                }

                // 建立 AudioClip
                AudioClip audioClip = AudioClip.Create("SharedAudioClip", floatData.Length, 1, 44100, false);
                audioClip.SetData(floatData, 0);

                return audioClip;
            }
        }
    }

    private void NotifyIPCClient(string message)
    {
        using (NamedPipeClientStream clientStream = new NamedPipeClientStream(".", "AudioIPC"))
        {
            // 連接到命管道伺服器
            clientStream.Connect();

            using (StreamWriter writer = new StreamWriter(clientStream))
            {
                // 寫入訊息
                writer.WriteLine(message);
                writer.Flush();
            }
        }
    }

    private static float IntToFloatSample(int value)
    {
        const float intToFloatConversionFactor = 1.0f / (1 << 31);
        return value * intToFloatConversionFactor;
    }

    public int IPCCall(EIPCAction action)
    {
        switch (action)
        {
            case EIPCAction.StartRecord:
                NotifyIPCClient("startRec");
                break;
            case EIPCAction.StopRecord:
                NotifyIPCClient("stopRec");
                break;
            case EIPCAction.Exit:
                NotifyIPCClient("exit");
                break;
        }
        return 1;
    }
    public enum EIPCAction
    {
        StartRecord,
        StopRecord,
        Exit
    }
}
