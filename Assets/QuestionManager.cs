
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton
public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;
    public AudioManager audioManager;
    public static event System.Action<ERelation> OnRelationChanged;
    public static event System.Action<String> OnQuestionChanged;
    public TextControll textControll;
    private int lstPhoneOneState = 0;
    private int lstPhoneTwoState = 0;

    // 0: Friend 1: Couple 2: Stranger -1: Not Selected , other number will ignore it.
    private ERelation _selectedRelation;
    private byte state;
    public ERelation selectedRelation
    {
        private set
        {
            _selectedRelation = value;
        }
        get => _selectedRelation;
    }

    public Relation relation { private set; get; }
    [HideInInspector]
    public string[] questions;
    public int roundCount { private set; get; } = 0;
    public Player player1, player2;
    public bool[][] hasAnswer;
    public bool[][] hasHangUp;
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

    public bool IsCompletedQuestion()
    {
        // TODO: condition current round is completed or not.
        return true ;
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
                AwaitAnswerHandler();
                break;
            case EGameState.FinishAnswer:
                break;
            case EGameState.ReviewAnswer:
                ReviewAnswerHandler();
                break;
            case EGameState.Ending:
                ClearFlow();
                break;
            default:
                break;
        }

    }
    public void RecievePhoneStatus(int status)
    {
        Debug.Log(status);
        if (lstPhoneOneState != (status & 2))
        {
            lstPhoneTwoState = status & 2 >> 1;
            switch (lstPhoneOneState)
            {
                case 0:
                    OnPlayerOneCallHangUp();
                    break;
                case 1:
                    OnPlayerOneCallAnswered();
                    break;
                default:
                    break;
            }
        }
        if (lstPhoneTwoState == (status & 1))
        {
            lstPhoneTwoState = status % 1;
            switch (lstPhoneTwoState)
            {
                case 0:
                    OnPlayerTwoCallHangUp();
                    break;
                case 1:
                    OnPlayerTwoCallAnswered();
                    break;
                default:
                    break;
            }
        }
    }

    private void ReviewAnswerHandler()
    {
        StartCoroutine(ReviewAnswer());
    }
    private IEnumerator ReviewAnswer()
    {
        for (int i = 0; i < relation.maxQuestionLimit; i++)
        {
            textControll.UpdateText(questions[i]);
            yield return new WaitForSeconds(1f);
            yield return audioManager.PlayAudioClip(0, (float)(player1.timestamp[i].startTime - audioManager.recordingStartTime).TotalSeconds, (float)(player1.timestamp[i].endTime - audioManager.recordingStartTime).TotalSeconds);
            yield return new WaitForSeconds(1f);
            yield return audioManager.PlayAudioClip(1, (float)(player2.timestamp[i].startTime - audioManager.recordingStartTime).TotalSeconds, (float)(player2.timestamp[i].endTime - audioManager.recordingStartTime).TotalSeconds);
        }

        yield break;
    }

    private void AwaitAnswerHandler()
    {
        audioManager.PlayDing();
        Debug.Log(relation.maxQuestionLimit);
        player1.timestamp[roundCount].startTime = DateTime.Now;
        player2.timestamp[roundCount].startTime = DateTime.Now;
    }

    void ClearFlow()
    {
        player1 = new Player();
        player2 = new Player();

        relation = null;
        roundCount = 0;
    }
    public void OnPlayerOneCallAnswered()
    {
        if (hasAnswer[0][roundCount]) return;
        player1.phoneState = true;
        player1.SetStartTime(DateTime.Now,roundCount);
        hasAnswer[0][roundCount] = true;
    }
    public void OnPlayerTwoCallAnswered()
    {
        if (hasAnswer[1][roundCount]) return;
        player2.phoneState = true;
        player2.SetStartTime(DateTime.Now, roundCount);
        hasAnswer[1][roundCount] = true;
    }
    public void OnPlayerOneCallHangUp()
    {
        if (!hasHangUp[0][roundCount]) return;
        player1.phoneState = false;
        player1.SetEndTime(DateTime.Now, (int)roundCount);
        Debug.Log(player1.timestamp[roundCount].endTime);
    }

    public void OnPlayerTwoCallHangUp()
    {
        if (!hasHangUp[0][roundCount]) return;
        player2.phoneState = false;
        player2.SetEndTime(DateTime.Now, (int)roundCount);

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
        hasAnswer = new bool[2][];
        hasAnswer[0] = new bool[relation.maxQuestionLimit];
        hasAnswer[1] = new bool[relation.maxQuestionLimit];

        hasHangUp = new bool[2][];
        hasHangUp[0] = new bool[relation.maxQuestionLimit];
        hasHangUp[1] = new bool[relation.maxQuestionLimit];
        Debug.Log(hasHangUp[1][0]);
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
        hasAnswer[0][roundCount] = false;
        hasAnswer[1][roundCount] = false;
        OnQuestionChanged?.Invoke(questions[roundCount]);
    }
}
public class Player
{
    public bool phoneState = false;
    public List<RecordTimeStamp> timestamp;

    public Player()
    {
    }

    public Player(int roundCount)
    {
        setRound(roundCount);
    }

    public void setRound(int roundCount)
    {
        Debug.Log("setting Round");
        timestamp = new List<RecordTimeStamp>(roundCount);
        for (int i = 0; i < roundCount; i++)
        {
            timestamp.Add(new RecordTimeStamp());
        }
    }

    public void SetStartTime(DateTime time, int currentRound)
    {
        timestamp[currentRound].startTime = time;
    }

    public void SetEndTime(DateTime time, int currentRound)
    {
        timestamp[currentRound].endTime = time;
    }
}

public class RecordTimeStamp
{
    public DateTime startTime;
    public DateTime endTime;
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