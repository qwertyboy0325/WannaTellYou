using UnityEngine;

public class AudioManager : MonoBehaviour
{
    void Start()
    {
        AudioConfiguration config = AudioSettings.GetConfiguration();
        int numDevices = config.dspBufferSize; 

        for (int i = 0; i < numDevices; i++)
        {
            //string deviceName = AudioSettings.GetOutputDeviceName(i);
            //Debug.Log("Device " + i + ": " + deviceName);
        }
    }
}