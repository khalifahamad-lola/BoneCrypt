using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup pauseMenuRoot;
    [SerializeField] private CanvasGroup darkOverlay;
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private CanvasGroup settingsPanel;

    private bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 1f;

        Show(pauseMenuRoot, false);
        Show(darkOverlay, true);
        Show(mainPanel, true);
        Show(settingsPanel, false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (settingsPanel.alpha == 1)
                    BackToPauseMenu();
                else
                    ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        Show(pauseMenuRoot, true);
        Show(darkOverlay, true);
        Show(mainPanel, true);
        Show(settingsPanel, false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        Show(pauseMenuRoot, false);
        Show(mainPanel, true);
        Show(settingsPanel, false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        Show(darkOverlay, true);
        Show(mainPanel, false);
        Show(settingsPanel, true);
    }

    public void BackToPauseMenu()
    {
        Show(settingsPanel, false);
        Show(mainPanel, true);
        Show(darkOverlay, true);
    }

    private void Show(CanvasGroup cg, bool state)
    {
        if (cg == null) return;

        cg.alpha = state ? 1 : 0;
        cg.interactable = state;
        cg.blocksRaycasts = state;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

