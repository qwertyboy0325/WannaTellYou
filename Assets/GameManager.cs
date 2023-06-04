
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
    private EGameState _state;
    private Coroutine currentPutQuestionCoroutine;
    public EGameState state { get => _state; }
    public static event System.Action<EGameState> OnGameStateChanged;
    void Awake()
    {
        Instance = this;
    }
    public void UpdateGameState(EGameState newState)
    {
        _state = newState;
        Debug.Log("Updated Game state: " + newState);
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
        Debug.Log("AAAA");
        yield return new WaitForSeconds(3f);
        _state = EGameState.AwaitAnswer;
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
        videoController.PlayVideo(EVideo.ThirdLevel);
    }

    private void AwaitAnswerHandler()
    {
        StopCoroutine(currentPutQuestionCoroutine);
        if (!QuestionManager.Instance.IsCompleteQuestion())
        {
            QuestionManager.Instance.RedrawQuestion();
            return;
        }
        uint roundCount = QuestionManager.Instance.roundCount;
        int maxCount = QuestionManager.Instance.relation.maxQuestionLimit;
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