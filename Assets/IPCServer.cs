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
        // �b�t�@��������i���l��
        mainTask = Task.Run(async () =>
        {
            serverStream = new NamedPipeServerStream("AudioIPC",PipeDirection.InOut);
            memoryMappedFile = MemoryMappedFile.CreateNew("AudioIPCSharedMemory", 200000000);
            await serverStream.WaitForConnectionAsync();

            Debug.Log("Connected!");
            // �b�t�@����������򱵦��T��
            _ = ReceiveMessagesAsync();

            // �b�C���}�l�ɵo�e�T���� Client Process
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
                    // �ǻ��T�����ƥ�q�\��
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

    // ��L�{���s���@�ɰO���骺��k
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

    // �o�e�T���� Client Process
    public async Task SendMessageToClient(string message)
    {
        using (NamedPipeClientStream clientStream = new NamedPipeClientStream(".","AudioIPC"))
        {
            // �s����R�W�޹D���A��
            await clientStream.ConnectAsync();

            using (StreamWriter writer = new StreamWriter(clientStream))
            {
                // �g�J�T��
                writer.AutoFlush = true;
                writer.WriteLine(message);
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
        }
    }
}
