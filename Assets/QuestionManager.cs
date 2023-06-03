
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton
public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;
    public static event System.Action<ERelation> OnRelationChanged;

    // 0: Friend 1: Couple 2: Stranger -1: Not Selected , other number will ignore it.
    private ERelation _selectedRelation;
    private byte questionState;
    public ERelation selectedRelation
    {
        set
        {
            UpdateRelation(value);
        }
        get => _selectedRelation;
    }

    public void UpdateRelation(ERelation newState)
    {
        _selectedRelation = newState;
        switch (newState)
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
    }
    private Relation relation;
    [HideInInspector]
    public string[] questions;
    public PlayerStatus player1, player2;
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
        UpdateRelation(ERelation.None);
    }

    // Update is called once per frame
    void Update()
    {

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
            case EGameState.FinishAnser:
                break;
            case EGameState.ReviewAnser:
                break;
            case EGameState.Ending:
                ResetFlow();
                break;
            default:
                break;
        }

    }
        // Code for express the flow (just for reference, dont use it.)
        void Flow()
    {
        AssignRelation();
    }
    void ResetFlow()
    {
        // init var
        player1 = new PlayerStatus();
        player2 = new PlayerStatus();

        // init workflow status
        selectedRelation = ERelation.None;
    }
    void EndFlow()
    {
        ResetFlow();
        GameManager.Instance.UpdateGameState(EGameState.ReviewAnser);
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
        }
    }
    }
    public class PlayerStatus
{
    bool phoneState;
    Record[] records;
    public PlayerStatus()
    {
    }
    public PlayerStatus(int roundCount)
    {
        setRound(roundCount);
    }
    public void setRound(int roundCount) {
        records = new Record[roundCount];
        for(int i = 0; i < records.Length; i++)
        {
            records[i] = new Record();
        }
    }
}

public class Record
{
    float startTime;
    float endTime;
    public Record()
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
enum EQuestionState : byte
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