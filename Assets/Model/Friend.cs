using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Friend : Relation
{
    public Friend()
    {
        Questions = new string[]{
            "���X�T�ӹﭱ���B�ͪ��u�I",
            "��谵�L�ƻ����A�P�ʩηP�ª���",
            "����z�Ѫ����}�]���쪺���� �קK�����^",
            "�p�G�Τ@�ذʪ��ήe���A�L�̹����ذʪ��H��]�O�H",
            "�Q�@�_�������Ʊ�(ex�@�_�X��...)",
            "�Ĥ@�L�H�β{�b�L�H",
            "��谵�L�̰g���Ʊ�",
            "�p�G�A�̯���洫�����@�ѡA�A�̷Q�����誺���ǥͬ��H�֦������ǯS�x�H",
            "�B�����Y���A�֬O��ť�̡A�֬O�D����?",
            "���̳��w���F��Ψƪ��H",
            "�J�����̤j�����ܬO�H"
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
