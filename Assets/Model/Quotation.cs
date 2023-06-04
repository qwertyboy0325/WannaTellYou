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
            "與你在一起，時間變得無比寶貴，每一刻都值得珍惜。",
            "你的微笑讓我心跳加速，你的愛讓我感到無比幸福。",
            "Thank you for reminding me that the best things in life are shared.",
            "我不會後悔晚點遇見你，因為我們每個人的旅程都是獨一無二的，你就是我生命中最美妙的那個巧合。",
            "你是我心中失落的一塊拼圖，與你在一起，我感到完整。",
            "Sometimes strangers become friends, and I'm grateful to have met you.",
            "謝謝你陪伴我度過這麼美好的一天。",
            "感謝命運讓我們相遇，我們的情誼將是無可取代的。",
            "感謝你的存在，你讓我的世界變得更加豐富多彩。",
            "感謝這次美好的相遇，讓我們的回憶永存心中。"
        };
    }

    public string GetQuotation()
    {
        string result;
        result = quotation[Random.Range(0, quotation.Length)];
        return result;
    }
}
