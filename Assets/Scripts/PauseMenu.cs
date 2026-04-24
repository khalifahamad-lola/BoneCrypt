using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Main References")]
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private RectTransform panel;
    [SerializeField] private GameObject quickSlotsPanel;

    [Header("Settings Panel")]
    [SerializeField] private CanvasGroup settingsPanel;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float scaleSpeed = 8f;
    [SerializeField] private Vector3 hiddenScale = new Vector3(0.9f, 0.9f, 1f);
    [SerializeField] private Vector3 shownScale = Vector3.one;

    [Header("Main Menu Scene Name")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    private bool isAnimating = false;
    private bool showingSettings = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (pauseMenu != null)
        {
            pauseMenu.alpha = 0f;
            pauseMenu.interactable = false;
            pauseMenu.blocksRaycasts = false;
        }

        if (panel != null)
            panel.localScale = hiddenScale;

        if (settingsPanel != null)
        {
            settingsPanel.alpha = 0f;
            settingsPanel.interactable = false;
            settingsPanel.blocksRaycasts = false;
        }

        if (quickSlotsPanel != null)
            quickSlotsPanel.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isAnimating)
        {
            if (showingSettings)
            {
                CloseSettings();
                return;
            }

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StopAllCoroutines();
        StartCoroutine(AnimateMenu(true));
    }

    public void Resume()
    {
        if (!isPaused) return;

        CloseSettings();

        isPaused = false;
        Time.timeScale = 1f;

        if (quickSlotsPanel != null)
            quickSlotsPanel.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StopAllCoroutines();
        StartCoroutine(AnimateMenu(false));
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) return;

        showingSettings = true;

        settingsPanel.alpha = 1f;
        settingsPanel.interactable = true;
        settingsPanel.blocksRaycasts = true;

        if (quickSlotsPanel != null)
            quickSlotsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        showingSettings = false;

        if (settingsPanel != null)
        {
            settingsPanel.alpha = 0f;
            settingsPanel.interactable = false;
            settingsPanel.blocksRaycasts = false;
        }

        if (quickSlotsPanel != null)
            quickSlotsPanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Quit Game pressed");
    }

    private System.Collections.IEnumerator AnimateMenu(bool show)
    {
        isAnimating = true;

        float targetAlpha = show ? 1f : 0f;
        Vector3 targetScale = show ? shownScale : hiddenScale;

        if (show)
        {
            pauseMenu.interactable = true;
            pauseMenu.blocksRaycasts = true;
        }

        while (!Mathf.Approximately(pauseMenu.alpha, targetAlpha) ||
               Vector3.Distance(panel.localScale, targetScale) > 0.001f)
        {
            pauseMenu.alpha = Mathf.Lerp(
                pauseMenu.alpha,
                targetAlpha,
                Time.unscaledDeltaTime * fadeSpeed
            );

            panel.localScale = Vector3.Lerp(
                panel.localScale,
                targetScale,
                Time.unscaledDeltaTime * scaleSpeed
            );

            if (Mathf.Abs(pauseMenu.alpha - targetAlpha) < 0.01f)
                pauseMenu.alpha = targetAlpha;

            if (Vector3.Distance(panel.localScale, targetScale) < 0.01f)
                panel.localScale = targetScale;

            yield return null;
        }

        if (!show)
        {
            pauseMenu.interactable = false;
            pauseMenu.blocksRaycasts = false;
        }

        isAnimating = false;
    }
}