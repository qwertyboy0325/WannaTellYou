using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Friend : Relation
{
    public Friend()
    {
        Questions = new string[]{
            "說出三個對面的朋友的優點",
            "對方做過甚麼讓你感動或感謝的事",
            "不能理解的怪癖（有趣的那種 避免爭執）",
            "如果用一種動物形容對方，他最像哪種動物？原因是？",
            "想一起完成的事情(ex一起出國...)",
            "第一印象及現在印象",
            "對方做過最迷的事情",
            "如果你們能夠交換身份一天，你們想體驗對方的哪些生活？擁有對方哪些特徵？",
            "朋友關係中，誰是傾聽者，誰是訴說者?",
            "對方最喜歡的東西或事物？",
            "遇到對方後最大的轉變是？"
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
                if (System.Array.Exists(current, element => element == i)) isRepeat = true;
            }
            result = randomString;

        } while (!isRepeat);

        return result;
    }
}
