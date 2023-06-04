using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Couple : Relation
{
    public Couple()
    {
        Questions = new string[]{
            "�ΤT�ӵ��ήe���b�A�������ˤl(ex ��K�B�ŬX�B�g�H)",
            "���X������A�߰ʪ������Φ欰",
            "�P�¹�誺��",
            "���ӷQ�@�_�Ȧ檺�a��(ex��a)",
            "��誺���ǯS��l�ާA",
            "�Q�n����Z��/�{�����Ʊ�",
            "�p�G�A�̵L�N���o��@�ʸU�A�|�Q�n�@�_�������?",
            "�p�G�A�̯���@�_�}�@�a���A�A�̷|�}���򩱡H������?",
            "�p�G����n�[�F�A�A�|���D�p�٬O������?",
            "�i�ծ�/�Q�i�ծɪ��P��?",
            "�橹�L�{���A���A�̦L�H�`�誺�^��?"
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
