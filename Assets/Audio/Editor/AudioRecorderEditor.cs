using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[CustomEditor(typeof(AudioRecorder))]
public class AudioRecorderEditor : Editor
{
    private SerializedProperty recordingTimeProperty;
    private SerializedProperty outputMixerGroupProperty;
    private SerializedProperty savePathProperty;
    private SerializedProperty availableDevicesProperty;
    private SerializedProperty selectedDeviceProperty;

    private void OnEnable()
    {
        recordingTimeProperty = serializedObject.FindProperty("recordingTime");
        outputMixerGroupProperty = serializedObject.FindProperty("outputMixerGroup");
        savePathProperty = serializedObject.FindProperty("savePath");
        availableDevicesProperty = serializedObject.FindProperty("availableDevices");
        selectedDeviceProperty = serializedObject.FindProperty("selectedDevice");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(recordingTimeProperty);
        EditorGUILayout.PropertyField(outputMixerGroupProperty);
        EditorGUILayout.PropertyField(savePathProperty);

        AudioRecorder audioRecorder = (AudioRecorder)target;

        // 获取可用的录音设备列表
        string[] availableDevices = Microphone.devices;
        int selectedDeviceIndex = GetSelectedDeviceIndex(selectedDeviceProperty.stringValue, availableDevices);

        // 显示下拉式选项
        int newSelectedDeviceIndex = EditorGUILayout.Popup("Recording Device", selectedDeviceIndex, availableDevices);

        if (newSelectedDeviceIndex != selectedDeviceIndex)
        {
            selectedDeviceProperty.stringValue = availableDevices[newSelectedDeviceIndex];
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
        return 0;
    }
}
