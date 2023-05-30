using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Stranger : Relation
{
    public Stranger()
    {
        Questions = new string[]{
            "自己最喜歡吃的食物",
            "最近最煩惱的事情",
            "推薦一個自己喜歡的在地美食",
            "推薦對方的下一餐",
            "喜歡哪種類型的歌曲? 分享一首自己最喜歡的",
            "分享最近發生的趣事",
            "分享你的興趣或休閒活動",
            "最想去哪裡旅遊?為甚麼?",
            "喜歡什麼運動?",
            "動物能代表人的性格，你覺得你像哪種動物?為甚麼?",
            "想跟對方認識嗎（固定第四題）"
        };
        maxQuestionLimit = 4;
    }
    override public string[] getQuestions()
    {
        List<string> result = new List<string>();
        while (result.Count < maxQuestionLimit)
        {
            string randomString = Questions[Random.Range(0, Questions.Length - 1)];
            if (!result.Contains(randomString))
            {
                result.Add(randomString);
            }
            // Add Last Question that will always be in List.
            result.Add(Questions[Questions.Length - 1]);
        }
        return result.ToArray();
    }
    public override string getQuestion(string[] current)
    {
        bool isRepeat = false;
        string result;
        do
        {
            string randomString = Questions[Random.Range(0, Questions.Length-1)];
            foreach (var i in current)
            {
                if (System.Array.Exists(current, element => element == i)) isRepeat = true;
            }
            result = randomString;

        } while (!isRepeat);

        return result;
    }
}
