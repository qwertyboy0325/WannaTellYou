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
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            EGameState state = GameManager.Instance.state;
            switch (state)
            {
                case EGameState.Idle:
                    break;
                case EGameState.Introduce:
                    GameManager.Instance.UpdateGameState(EGameState.PutQuestion);
                    break;
                case EGameState.PutQuestion:
                    GameManager.Instance.UpdateGameState(EGameState.AwaitAnswer);
                    break;
                case EGameState.AwaitAnswer:
                    break;
                case EGameState.FinishAnswer:
                    GameManager.Instance.UpdateGameState(EGameState.ReviewAnswer);
                    break;
                case EGameState.ReviewAnswer:
                    GameManager.Instance.UpdateGameState(EGameState.Ending);
                    break;
                case EGameState.Ending:
                    GameManager.Instance.UpdateGameState(EGameState.Idle);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            QuestionManager.Instance.RedrawQuestion();
        }
    }
}
