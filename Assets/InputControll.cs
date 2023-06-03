using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControll : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            QuestionManager.Instance.InitFlow(ERelation.Friend);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            QuestionManager.Instance.InitFlow(ERelation.Couple);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            QuestionManager.Instance.InitFlow(ERelation.Stranger);
        }
    }
}
