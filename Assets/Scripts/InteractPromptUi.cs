using UnityEngine;
using TMPro;

public class InteractPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private void Start()
    {
        HidePrompt();
    }

    public void ShowPrompt(string message = "Press E")
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = message;
    }

    public void HidePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}