using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreenUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI deathText;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.8f;
    [SerializeField] private float textDelay = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.8f;

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (deathText != null)
            deathText.gameObject.SetActive(false);
    }

    public IEnumerator PlayDeathSequence(float holdDuration)
    {
        if (deathText != null)
            deathText.gameObject.SetActive(false);

        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeInDuration));

        if (deathText != null)
        {
            yield return new WaitForSeconds(textDelay);
            deathText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(holdDuration);
    }

    public IEnumerator PlayRespawnFadeOut()
    {
        if (deathText != null)
            deathText.gameObject.SetActive(false);

        yield return StartCoroutine(FadeCanvas(1f, 0f, fadeOutDuration));
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