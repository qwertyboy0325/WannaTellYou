
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    // 0: Friend 1: Couple 2: Stranger -1: Not Selected , other number will ignore it.
    private ERelation _selectedRelation;
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

