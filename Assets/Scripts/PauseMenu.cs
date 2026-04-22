using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public CanvasGroup pauseMenu;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.alpha = isPaused ? 1 : 0;
        pauseMenu.interactable = isPaused;
        pauseMenu.blocksRaycasts = isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void Resume()
    {
        TogglePause();
    }
}