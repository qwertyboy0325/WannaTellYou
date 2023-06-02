using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Start()
    {
        
    }

    void Update()
    {
        
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