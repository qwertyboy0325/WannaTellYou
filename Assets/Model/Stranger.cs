using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Stranger : Relation
{
    public Stranger()
    {
        Questions = new string[]{
            "�ۤv�̳��w�Y������",
            "�̪�̷дo���Ʊ�",
            "���ˤ@�Ӧۤv���w���b�a����",
            "���˹�誺�U�@�\",
            "���w�����������q��? ���ɤ@���ۤv�̳��w��",
            "���ɳ̪�o�ͪ����",
            "���ɧA������Υ𶢬���",
            "�̷Q�h���̮ȹC?���ƻ�?",
            "���w����B��?",
            "�ʪ���N��H���ʮ�A�Aı�o�A�����ذʪ�?���ƻ�?",
            "�Q����{�Ѷܡ]�T�w�ĥ|�D�^"
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
