
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private EGameState state;
    public static event System.Action<EGameState> OnGameStateChanged;
    void Awake()
    {
        Instance = this;
    }
    public void UpdateGameState(EGameState newState)
    {
        state =  newState;
        switch (state)
        {
            case EGameState.Idle:
                break;
            case EGameState.Introduce:
                break;
            case EGameState.PutQuestion:
                break;
            case EGameState.AwaitAnswer:
                break;
            case EGameState.FinishAnser:
                break;
            case EGameState.ReviewAnser:
                break;
            case EGameState.Ending:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState);
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
    FinishAnser,
    ReviewAnser,
    Ending,
}