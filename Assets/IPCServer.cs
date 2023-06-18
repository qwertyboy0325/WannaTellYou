using System.IO;
using System.IO.Pipes;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;

public class IPCServer : MonoBehaviour
{
    private NamedPipeServerStream serverStream;
    private MemoryMappedFile memoryMappedFile;
    private Task mainTask;

    public delegate void MessageReceivedDelegate(string message);
    public static event MessageReceivedDelegate OnMessageReceived;

    private void Start()
    {
        // 在另一執行緒中進行初始化
        mainTask = Task.Run(async () =>
        {
            serverStream = new NamedPipeServerStream("AudioIPC",PipeDirection.InOut);
            memoryMappedFile = MemoryMappedFile.CreateNew("AudioIPCSharedMemory", 200000000);
            await serverStream.WaitForConnectionAsync();

            Debug.Log("Connected!");
            // 在另一執行緒中持續接收訊息
            _ = ReceiveMessagesAsync();

            // 在遊戲開始時發送訊息給 Client Process
            _ = SendMessageToClient("Hello from Unity!");
        });
    }
    
    private async Task ReceiveMessagesAsync()
    {
        using (StreamReader reader = new StreamReader(serverStream))
        {
            while (true)
            {
                string message = await reader.ReadLineAsync();
                if (message != null)
                {
                    // 傳遞訊息給事件訂閱者
                    OnMessageReceived?.Invoke(message);
                }
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

                AudioClip audioClip = WavUtility.ToAudioClip(byteData);
                return audioClip;
            }
        }
    }

    // 發送訊息給 Client Process
    public async Task SendMessageToClient(string message)
    {
        using (NamedPipeClientStream clientStream = new NamedPipeClientStream(".","AudioIPC"))
        {
            // 連接到命名管道伺服器
            await clientStream.ConnectAsync();

            using (StreamWriter writer = new StreamWriter(clientStream))
            {
                // 寫入訊息
                writer.AutoFlush = true;
                writer.WriteLine(message);
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
        }
    }
}
