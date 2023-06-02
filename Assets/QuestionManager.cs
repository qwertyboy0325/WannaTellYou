
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton
public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;
    // 0: Friend 1: Couple 2: Stranger -1: Not Selected , other number will ignore it.
    private ERelation _selectedRelation;
    private EQuestionState questionState;
    public ERelation selectedRelation
    {
        set
        {
            _selectedRelation = value;
            ResetFlow();
        }
        get => _selectedRelation;
    }
    private Relation relation;
    public string[] questions;
    public PlayerStatus player1, player2;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResetFlow();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Code for express the flow (just for reference, dont use it.)
    void Flow()
    {
        assignRelationType();
    }
    void ResetFlow()
    {
        // init var
        player1 = new PlayerStatus();
        player2 = new PlayerStatus();

        // init workflow status
        selectedRelation = ERelation.None;
    }
    void assignRelationType()
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
    Round[] rounds;
    public PlayerStatus()
    {
    }
    public PlayerStatus(int roundCount)
    {
        setRound(roundCount);
    }
    public void setRound(int roundCount) {
        rounds = new Round[roundCount];
    }
}
public class Round
{
    float startTime;
    float endTime;
    public Round()
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