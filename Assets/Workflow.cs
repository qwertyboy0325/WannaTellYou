
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workflow : MonoBehaviour
{
    // 0: Friend 1: Couple 2: Stranger -1: Not Selected , other number will ignore it.
    public ERelation selectedRelation;
    private Relation[] Relations;
    public string[] questions;
    void Start()
    {
        // init var
        RelationSets relationSets = new RelationSets();
        Relations = relationSets.relations;

        // init workflow status
        selectedRelation = ERelation.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class PlayerStatus
{

}
public class Round
{

}
