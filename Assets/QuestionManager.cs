
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton
public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;
    public static event System.Action<ERelation> OnRelationChanged;
    public static event System.Action<String> OnQuestionChanged;

    // 0: Friend 1: Couple 2: Stranger -1: Not Selected , other number will ignore it.
    private ERelation _selectedRelation;
    private byte state;
    public ERelation selectedRelation
    {
        set
        {
            _selectedRelation = value;
        }
        get => _selectedRelation;
    }

    public Relation relation { private set; get; }
    [HideInInspector]
    public string[] questions;
    public uint roundCount { private set; get; }
    public Player player1, player2;
    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChanged += OnGameManagerStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameManagerStateChanged;
    }

    void Start()
    {
        roundCount = 0;
        UpdateRelationState(ERelation.None);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsCompleteQuestion()
    {
        return false;
    }

    void OnGameManagerStateChanged(EGameState state)
    {
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
            case EGameState.FinishAnswer:
                break;
            case EGameState.ReviewAnswer:
                break;
            case EGameState.Ending:
                ClearFlow();
                break;
            default:
                break;
        }

    }
    void ClearFlow()
    {
        player1 = new Player();
        player2 = new Player();

        relation = null;
        roundCount = 0;
    }
    void AssignRelation()
    {
        switch (selectedRelation)
        {
            case ERelation.None:
                break;
            case ERelation.Friend:
                relation = new Friend();
                break;
            case ERelation.Couple:
                relation = new Couple();
                break;
            case ERelation.Stranger:
                relation = new Stranger();
                break;
            default:
                break;
        }
    }

    public void InitFlow(ERelation newState)
    {
        if (newState == ERelation.None) return;
        roundCount = 0;
        UpdateRelationState(newState);
        GameManager.Instance.UpdateGameState(EGameState.Introduce);
        AssignRelation();

        GenerateQuestion();
        InitPlayers();
    }

    private void InitPlayers()
    {
        player1 = new Player(relation.maxQuestionLimit);
        player2 = new Player(relation.maxQuestionLimit);
    }

    private void GenerateQuestion()
    {
        questions = relation.getQuestions();
    }

    public void UpdateRelationState(ERelation newState)
    {
        selectedRelation = newState;

        Debug.Log("Updated Relation Type: "+_selectedRelation);
        switch (selectedRelation)
        {
            case ERelation.Friend:
                break;
            case ERelation.Couple:
                break;
            case ERelation.Stranger:
                break;
            case ERelation.None:
                break;
            default:
                break;
        }
        OnRelationChanged?.Invoke(newState);
    }

    public string GetCurrentQuestion()
    {
        return questions[roundCount];
    }

    public void NextQuestion()
    {
        roundCount++;
        OnQuestionChanged?.Invoke(questions[roundCount]);
    }

    public void RedrawQuestion()
    {
        Debug.Log("redraw : " + roundCount);
        questions[roundCount] = relation.getQuestion(questions);
        OnQuestionChanged?.Invoke(questions[roundCount]);
    }
}
public class Player
{
    bool phoneState;
    RecordTimeStamp[] recordsTimeStamps;
    public Player()
    {
    }
    public Player(int roundCount)
    {
        setRound(roundCount);
    }
    public void setRound(int roundCount)
    {
        recordsTimeStamps = new RecordTimeStamp[roundCount];
        for (int i = 0; i < recordsTimeStamps.Length; i++)
        {
            recordsTimeStamps[i] = new RecordTimeStamp();
        }
    }
}

public class RecordTimeStamp
{
    float startTime;
    float endTime;
    public RecordTimeStamp()
    {

    }
}

// Question Event:
// 0x00000000
//         ^^
//         ||_player 1 finished
//         |
//         |__player 2 finished
//         
public enum EQuestionState : byte
{
    None = 0x00,
    PlayerOneIsFinished = 0x01,
    PlayerTwoIsFinished = 0x02,
    RoundSkipped = 0x04,
    RoundStored = 0x08,
    RoundFinished = 0x10,
    AllRoundComplete = 0x20,
    InRound = 0x40,
    ResetQuestion = 0x80
}