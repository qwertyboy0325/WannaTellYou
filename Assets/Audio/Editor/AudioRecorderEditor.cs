using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[CustomEditor(typeof(AudioManager))]
public class AudioRecorderEditor : Editor
{
    private SerializedProperty recordingTimeProperty;
    private SerializedProperty outputMixerGroupProperty;
    private SerializedProperty savePathProperty;
    private SerializedProperty availableDevicesProperty;
    private SerializedProperty SFXProperty;
    private SerializedProperty selectedFirstDeviceProperty;
    private SerializedProperty selectedSecDeviceProperty;

    private void OnEnable()
    {
        recordingTimeProperty = serializedObject.FindProperty("recordingTime");
        outputMixerGroupProperty = serializedObject.FindProperty("outputMixerGroup");
        savePathProperty = serializedObject.FindProperty("savePath");
        SFXProperty = serializedObject.FindProperty("SFX");
        availableDevicesProperty = serializedObject.FindProperty("availableDevices");
        selectedFirstDeviceProperty = serializedObject.FindProperty("selectedFirstDeviceProperty");
        selectedSecDeviceProperty = serializedObject.FindProperty("selectedSecDeviceProperty");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(recordingTimeProperty);
        EditorGUILayout.PropertyField(outputMixerGroupProperty);
        EditorGUILayout.PropertyField(savePathProperty);
        EditorGUILayout.PropertyField(SFXProperty);

        AudioManager audioRecorder = (AudioManager)target;

        // 获取可用的录音设备列表
        string[] availableDevices = Microphone.devices;
        int firstSelectedDeviceIndex = GetSelectedDeviceIndex(selectedFirstDeviceProperty.stringValue, availableDevices);
        int secondSelectedDeviceIndex = GetSelectedDeviceIndex(selectedSecDeviceProperty.stringValue, availableDevices);

        // 显示下拉式选项
        int newFirstSelectedDeviceIndex = EditorGUILayout.Popup("First Recording Device", firstSelectedDeviceIndex, availableDevices);
        int newSecondSelectedDeviceIndex = EditorGUILayout.Popup("Second Recording Device", secondSelectedDeviceIndex, availableDevices);
        if (newFirstSelectedDeviceIndex != firstSelectedDeviceIndex)
        {
            selectedFirstDeviceProperty.stringValue = availableDevices[newFirstSelectedDeviceIndex];
        }
        if(newSecondSelectedDeviceIndex != secondSelectedDeviceIndex)
        {
            selectedSecDeviceProperty.stringValue = availableDevices[newSecondSelectedDeviceIndex];
        }

        serializedObject.ApplyModifiedProperties();
    }

    private int GetSelectedDeviceIndex(string selectedDevice, string[] availableDevices)
    {
        for (int i = 0; i < availableDevices.Length; i++)
        {
            if (availableDevices[i] == selectedDevice)
            {
                return i;
            }
        }
        Debug.Log(selectedDevice + " is not founded.");
        return 0;
    }
}
