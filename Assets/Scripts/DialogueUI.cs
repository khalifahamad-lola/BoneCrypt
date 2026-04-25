using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueLineText;
    [SerializeField] private Button nextButton;

    private DialogueManager dialogueManager;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);
    }

    public void SetDialogueManager(DialogueManager manager)
    {
        dialogueManager = manager;
    }

    public void ShowDialogue(string npcName, string line)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (npcNameText != null)
            npcNameText.text = npcName;

        if (dialogueLineText != null)
            dialogueLineText.text = line;
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void OnNextClicked()
    {
        if (dialogueManager != null)
            dialogueManager.ShowNextLine();
    }
}