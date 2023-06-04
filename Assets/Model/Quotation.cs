using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quotation
{
    string[] quotation;
    public Quotation()
    {
        quotation = new string[]
        {
            "�P�A�b�@�_�A�ɶ��ܱo�L���_�Q�A�C�@�賣�ȱo�ñ��C",
            "�A���L�����ڤ߸��[�t�A�A���R���ڷP��L�񩯺֡C",
            "Thank you for reminding me that the best things in life are shared.",
            "�ڤ��|�ᮬ���I�J���A�A�]���ڭ̨C�ӤH���ȵ{���O�W�@�L�G���A�A�N�O�ڥͩR���̬��������ӥ��X�C",
            "�A�O�ڤߤ��������@�����ϡA�P�A�b�@�_�A�ڷP�짹��C",
            "Sometimes strangers become friends, and I'm grateful to have met you.",
            "���§A����ګ׹L�o����n���@�ѡC",
            "�P�©R�B���ڭ̬۹J�A�ڭ̪����˱N�O�L�i���N���C",
            "�P�§A���s�b�A�A���ڪ��@���ܱo��[�״I�h�m�C",
            "�P�³o�����n���۹J�A���ڭ̪��^�Хæs�ߤ��C"
        };
    }

    public string GetQuotation()
    {
        string result;
        result = quotation[Random.Range(0, quotation.Length)];
        return result;
    }
}
