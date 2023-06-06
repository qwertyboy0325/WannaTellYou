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
        memoryMappedFile = MemoryMappedFile.CreateNew("AudioIPCSharedMemory",200000000);
        serverStream.WaitForConnection();

        // �b�t�@����������򱵦��T��
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
                // �ǻ��T�����ƥ�q�\��
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
    // ��L�{���s���@�ɰO���骺��k
    // ��L�{���s���@�ɰO���骺��k
    public static AudioClip ReadSharedMemoryAsAudioClip()
    {
        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("MySharedMemory"))
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                long byteCount = accessor.Capacity;
                byte[] byteData = new byte[byteCount];
                accessor.ReadArray(0, byteData, 0, (int)byteCount);

                // �ഫ��ƫ��A�����T��Ƭ��B�I�ƫ��A
                float[] floatData = new float[byteCount / sizeof(float)];
                for (int i = 0; i < floatData.Length; i++)
                {
                    floatData[i] = (float)System.BitConverter.ToInt32(byteData, i * sizeof(float));
                }

                // �إ� AudioClip
                AudioClip audioClip = AudioClip.Create("SharedAudioClip", floatData.Length, 1, 44100, false);
                audioClip.SetData(floatData, 0);

                return audioClip;
            }
        }
    }


}
