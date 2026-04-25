using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    public enum DialogueRole
    {
        LoreOnly,
        QuestGiver,
        Vendor
    }

    [Header("NPC")]
    [SerializeField] private string npcName = "NPC";
    [SerializeField] private DialogueRole dialogueRole = DialogueRole.LoreOnly;

    [Header("Lore Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private string[] defaultLines;

    [Header("Quest")]
    [SerializeField] private QuestData questData;

    [TextArea(2, 5)]
    [SerializeField] private string[] beforeQuestLines;

    [TextArea(2, 5)]
    [SerializeField] private string[] inProgressLines;

    [TextArea(2, 5)]
    [SerializeField] private string[] readyToCompleteLines;

    [TextArea(2, 5)]
    [SerializeField] private string[] completedLines;

    [Header("Vendor")]
    [SerializeField] private Vendor vendor;
    [SerializeField] private VendorUI vendorUI;

    [Header("Post Dialogue Objective")]
    [SerializeField] private bool showObjectiveAfterDialogue = false;
    [SerializeField] private string objectiveText = "";

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private QuestFeedbackUI questFeedbackUI;

    private string[] activeLines;

    public string NPCName => npcName;
    public string[] DialogueLines => activeLines;
    public DialogueRole Role => dialogueRole;

    private void Start()
    {
        if (dialogueManager == null)
            dialogueManager = FindFirstObjectByType<DialogueManager>();

        if (questManager == null)
            questManager = FindFirstObjectByType<QuestManager>();

        if (vendorUI == null)
            vendorUI = FindFirstObjectByType<VendorUI>();

        if (questFeedbackUI == null)
            questFeedbackUI = FindFirstObjectByType<QuestFeedbackUI>();
    }

    public void Interact(GameObject player)
    {
        if (dialogueManager == null)
        {
            Debug.LogWarning("NPCDialogue: No DialogueManager found.");
            return;
        }

        if (dialogueManager.IsDialogueOpen)
            return;

        activeLines = GetLinesForCurrentState();

        if (activeLines == null || activeLines.Length == 0)
        {
            Debug.LogWarning("NPCDialogue: No dialogue lines available.");
            return;
        }

        dialogueManager.StartDialogue(this);
    }

    private string[] GetLinesForCurrentState()
    {
        switch (dialogueRole)
        {
            case DialogueRole.LoreOnly:
                return defaultLines;

            case DialogueRole.QuestGiver:
                if (questData == null || questManager == null)
                    return defaultLines;

                QuestState state = questManager.GetQuestState(questData);

                switch (state)
                {
                    case QuestState.NotStarted:
                        return beforeQuestLines;
                    case QuestState.InProgress:
                        return inProgressLines;
                    case QuestState.ReadyToComplete:
                        return readyToCompleteLines;
                    case QuestState.Completed:
                        return completedLines;
                    default:
                        return defaultLines;
                }

            case DialogueRole.Vendor:
                return defaultLines;
        }

        return defaultLines;
    }

    public void OnDialogueFinished()
    {
        switch (dialogueRole)
        {
            case DialogueRole.LoreOnly:
                break;

            case DialogueRole.QuestGiver:
                HandleQuestDialogueEnd();
                break;

            case DialogueRole.Vendor:
                if (vendorUI != null && vendor != null)
                {
                    vendorUI.OpenVendor(vendor);
                }
                else
                {
                    Debug.LogWarning("NPCDialogue: Vendor UI or Vendor reference missing.");
                }
                break;
        }

        if (showObjectiveAfterDialogue && !string.IsNullOrWhiteSpace(objectiveText) && questFeedbackUI != null)
        {
            questFeedbackUI.ShowMessage("Objective", objectiveText);
        }
    }

    private void HandleQuestDialogueEnd()
    {
        if (questData == null || questManager == null)
            return;

        QuestState state = questManager.GetQuestState(questData);

        if (state == QuestState.NotStarted)
        {
            questManager.StartQuest(questData);
        }
        else if (state == QuestState.ReadyToComplete)
        {
            questManager.CompleteQuest(questData);
        }
    }
}