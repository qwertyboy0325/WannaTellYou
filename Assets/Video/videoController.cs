using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;

public enum EVideo
{
    Idle,
    Introduce,
    PutQuestion,
    AwaitAnswer,
    FinishAnser,
    ReviewAnser,
    Ending
}
public class videoController : MonoBehaviour
{
    string[] videoUrl = {
        "Assets/Video/c.mp4"
    };

    public VideoPlayer videoPlayer;
    private EVideo currentPlaying;

    private VideoStatus videoStatus;

    public EVideo playingVideo
    {
        get { return currentPlaying; }
        set { currentPlaying = value; }
    }

    public static event System.Action<VideoStatus> OnVideoStateChanged;

    void Start()
    {
        currentPlaying = EVideo.Idle;
        // 初始化VideoPlayer
        videoPlayer = gameObject.GetComponent<VideoPlayer>();


        PlayVideo(currentPlaying);
    }

    // Update is called once per frame
    void Update()
    {
        // When video play end
        if ((videoPlayer.frame) > 0 && (videoPlayer.isPlaying == false))
        {
            videoPlayer.Stop();

            UpdatePlayingState(currentPlaying, 1);
            Debug.Log("Video Play End");
        }
    }

    public void PlayVideo(EVideo state)
    {
        try
        {
            // Set video resource route
            videoPlayer.url = videoUrl[(int)state];
            // Play the video
            videoPlayer.Play();
            playingVideo = state;

            UpdatePlayingState(currentPlaying, 0);
            Debug.Log("Playing: "+ (int)state);
        }
        // Avoid index out of bound
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    public void UpdatePlayingState(EVideo playing, int isEnd)
    {
        videoStatus = new VideoStatus(playing, isEnd);
        OnVideoStateChanged?.Invoke(videoStatus);
    }
}
public struct VideoStatus
{
    public VideoStatus(EVideo EV, int CS)
    {
        currentVideo = EV;
        currentState CS;
    }
    EVideo currentVideo;
    int currentState;
}