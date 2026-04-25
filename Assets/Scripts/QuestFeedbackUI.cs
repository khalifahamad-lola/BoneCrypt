using System.Collections;
using UnityEngine;
using TMPro;

public class QuestFeedbackUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.4f;

    private Coroutine currentRoutine;

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (titleText != null)
            titleText.text = "";

        if (bodyText != null)
            bodyText.text = "";
    }

    public void ShowMessage(string title, string body)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(title, body));
    }

    private IEnumerator ShowRoutine(string title, string body)
    {
        if (titleText != null)
            titleText.text = title;

        if (bodyText != null)
            bodyText.text = body;

        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeInDuration));
        yield return new WaitForSecondsRealtime(holdDuration);
        yield return StartCoroutine(FadeCanvas(1f, 0f, fadeOutDuration));

        currentRoutine = null;
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        if (canvasGroup == null)
            yield break;

        float timer = 0f;
        canvasGroup.alpha = from;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}