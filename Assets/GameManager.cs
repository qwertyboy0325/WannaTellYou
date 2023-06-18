
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// Singleton
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public VideoController videoController;
    public AudioManager audioManager;
    public TextControll textControll;
    public ArduinoSerial arduinoSerial;
    private EGameState _state;
    private Coroutine currentPutQuestionCoroutine;
    public EGameState state { get => _state; }
    public static event System.Action<EGameState> OnGameStateChanged;
    void Awake()
    {
        Instance = this;
        VideoController.OnVideoStateChanged += OnVideoStateChanged;
    }

    private void OnVideoStateChanged(VideoStatus videoStatus)
    {
        if (videoStatus.currentState == 0) return;
        switch (videoStatus.currentVideo)
        {
            case EVideo.Idle:
                break;
            case EVideo.Introduce:
                break;
            case EVideo.FriendQuestion:
                textControll.CleanText();
                break;
            case EVideo.CoupleQuestion:
                textControll.CleanText();
                break;
            case EVideo.StrangerQuestion:
                textControll.CleanText();
                break;
            case EVideo.FirstLevel:
                break;
            case EVideo.SecondLevel:
                break;
            case EVideo.ThirdLevel:
                break;
        }
    }

    public void UpdateGameState(EGameState newState)
    {
        _state = newState;
        Debug.Log("Updated Game state: " + _state);
        switch (_state)
        {
            case EGameState.Idle:
                IdleHandler();
                break;
            case EGameState.Introduce:
                IntroduceHandler();
                break;
            case EGameState.PutQuestion:
                PutQuestionHandler();
                break;
            case EGameState.AwaitAnswer:
                AwaitAnswerHandler();
                break;
            case EGameState.FinishAnswer:
                FinishAnserHandler();
                break;
            case EGameState.ReviewAnswer:
                ReviewAnserHandler();
                break;
            case EGameState.Ending:
                EndingHandler();
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState);
    }
    private IEnumerator UpdatePutQuestionToAwaitQuestion()
    {
        yield return new WaitForSeconds(3f);
        _state = EGameState.AwaitAnswer;
        int roundCount = QuestionManager.Instance.roundCount;
        //QuestionManager.Instance.
        if (roundCount == 0) audioManager.StartRecording();
        OnGameStateChanged?.Invoke(EGameState.AwaitAnswer);
        Debug.Log("Updated Game state: " + _state);
    }
    private void IdleHandler()
    {
        videoController.PlayVideo(EVideo.Idle);
    }

    private void EndingHandler()
    {
    }

    private void ReviewAnserHandler()
    {
    }

    private void FinishAnserHandler()
    {
        StopCoroutine(currentPutQuestionCoroutine);
        audioManager.StopRecording();
        videoController.PlayVideo(EVideo.ThirdLevel);
    }

    private void AwaitAnswerHandler()
    {
        StopCoroutine(currentPutQuestionCoroutine);
        if (!QuestionManager.Instance.IsCompletedQuestion())
        {
            QuestionManager.Instance.RedrawQuestion();
            return;
        }
        int roundCount = QuestionManager.Instance.roundCount;
        int maxCount = QuestionManager.Instance.relation.maxQuestionLimit;
        Debug.Log(roundCount);
        if (roundCount >= maxCount - 1)
        {
            UpdateGameState(EGameState.FinishAnswer);
            return;
        }
        // Next Question case:
        QuestionManager.Instance.NextQuestion();
    }

    private void PutQuestionHandler()
    {
        if (currentPutQuestionCoroutine != null) StopCoroutine(currentPutQuestionCoroutine);
        switch (QuestionManager.Instance.selectedRelation)
        {
            case ERelation.Friend:
                videoController.PlayVideo(EVideo.FriendQuestion);
                break;
            case ERelation.Couple:
                videoController.PlayVideo(EVideo.CoupleQuestion);
                break;
            case ERelation.Stranger:
                videoController.PlayVideo(EVideo.StrangerQuestion);
                break;
            case ERelation.None:
                break;
        }
        currentPutQuestionCoroutine = StartCoroutine(UpdatePutQuestionToAwaitQuestion());
    }

    private void IntroduceHandler()
    {
        if (currentPutQuestionCoroutine != null) StopCoroutine(currentPutQuestionCoroutine);
        videoController.PlayVideo(EVideo.Introduce);
    }

    private void Start()
    {
        UpdateGameState(EGameState.Idle);
    }
}

public enum EGameState
{
    Idle,
    Introduce,
    PutQuestion,
    AwaitAnswer,
    FinishAnswer,
    ReviewAnswer,
    Ending,
}