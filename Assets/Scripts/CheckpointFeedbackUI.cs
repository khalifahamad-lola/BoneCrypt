using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CheckpointFeedbackUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.35f;
    [SerializeField] private float holdDuration = 1.2f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine currentRoutine;

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        if (messageText != null)
            messageText.text = message;

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