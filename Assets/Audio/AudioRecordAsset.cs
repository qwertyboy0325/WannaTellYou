using UnityEngine;

[CreateAssetMenu(fileName = "RecordedAudioAsset", menuName = "Custom/Recorded Audio Asset")]
public class AudioRecordAsset : ScriptableObject
{
    public AudioClip audioClip;
    public System.DateTime recordingStartTime;

    public void SaveAsset(string savePath)
    {
        Debug.Log(audioClip);
        UnityEditor.AssetDatabase.AddObjectToAsset(audioClip, this);
        UnityEditor.AssetDatabase.CreateAsset(this, savePath);
        Debug.Log(audioClip);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
}