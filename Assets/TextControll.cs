using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TextControll : MonoBehaviour
{
    public float fadeDuration = .3f;
    private Coroutine currentFadeCoroutine;
    public Text text;
    private IEnumerator FadeInTextCoroutine(string newText)
    {

        // 淡出效果
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = 1f - (elapsedTime / fadeDuration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 更新文本内容
        text.text = newText;

        // 淡入效果
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = elapsedTime / fadeDuration;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentFadeCoroutine = null;
    }
    void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
        QuestionManager.OnQuestionChanged += OnQuestionChanged;
    }

    private void OnQuestionChanged(string question)
    {
    }

    private void OnGameStateChanged(EGameState state)
    {
        switch (state)
        {
            case EGameState.Idle:
                text.enabled = false;
                break;
            case EGameState.Introduce:
                text.enabled = true;

                UpdateText("");
                break;
            case EGameState.PutQuestion:
                text.enabled = true;
                UpdateText(QuestionManager.Instance.GetCurrentQuestion());
                break;
            case EGameState.AwaitAnswer:
                text.enabled = true;
                break;
            case EGameState.FinishAnswer:
                text.enabled = true;
                UpdateText("");
                break;
            case EGameState.ReviewAnswer:
                text.enabled = true;
                UpdateText("");
                break;
            case EGameState.Ending:
                Quotation q = new Quotation();
                string quotation = q.GetQuotation();
                UpdateText(quotation);
                break;
            default:
                break;
        }
    }

    public void UpdateText(string newText)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeInTextCoroutine(newText));
    }
    public void CleanText()
    {
        UpdateText("");
    }

    void Start()
    {
        text.enabled = false;
        text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
