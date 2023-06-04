using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Couple : Relation
{
    public Couple()
    {
        Questions = new string[]{
            "用三個詞形容對方在你眼中的樣子(ex 體貼、溫柔、迷人)",
            "說出對方讓你心動的瞬間或行為",
            "感謝對方的話",
            "未來想一起旅行的地方(ex國家)",
            "對方的哪些特質吸引你",
            "想要跟對方坦白/認錯的事情",
            "如果你們無意間得到一百萬，會想要一起做什麼事?",
            "如果你們能夠一起開一家店，你們會開什麼店？為什麼?",
            "如果雙方吵架了，你會先道歉還是先妥協?",
            "告白時/被告白時的感受?",
            "交往過程中，讓你們印象深刻的回憶?"
        };
        maxQuestionLimit = 3;
    }
    override public string[] getQuestions()
    {
        List<string> result = new List<string>();
        while (result.Count < maxQuestionLimit)
        {
            string randomString = Questions[Random.Range(0, Questions.Length)];
            if (!result.Contains(randomString))
            {
                result.Add(randomString);
            }

        }
        return result.ToArray();
    }
    public override string getQuestion(string[] current)
    {
        bool isRepeat = false;
        string result;
        do
        {
            string randomString = Questions[Random.Range(0, Questions.Length)];
            foreach (var i in current)
            {
                if (System.Array.Exists(current, element => element == randomString)) isRepeat = true;
            }
            result = randomString;

        } while (isRepeat);

        return result;
    }
}
