
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workflow : MonoBehaviour
{
    // 0: Friend 1: Couple 2: Stranger -1: Not Selected , other number will ignore it.
    private ERelation _selectedRelation;
    public ERelation selectedRelation
    {
        set
        {
            _selectedRelation = value;
            resetFlow();
        }
        get => _selectedRelation;
    }
    private Relation relation;
    public string[] questions;
    public PlayerStatus player1, player2;
    void Start()
    {
        // init var
        // RelationSets relationSets = new RelationSets();
        // relations = relationSets.relations;
        player1 = new PlayerStatus();
        player2 = new PlayerStatus();

        // init workflow status
        selectedRelation = ERelation.None;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Code for express the flow (just for reference, dont use it.)
    void flow()
    {
        assignRelationType();
    }
    void resetFlow()
    {

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
    private void OnValidate()
    {

    }
}
public class PlayerStatus
{
    public PlayerStatus()
    {

    }
}
public class Round
{

}
