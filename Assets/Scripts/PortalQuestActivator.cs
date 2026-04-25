using UnityEngine;

public class PortalQuestActivator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PortalGate_Controller portalGate;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private QuestData requiredQuest;

    [Header("Activation State")]
    [SerializeField] private bool activateOnReadyToComplete = true;

    private bool hasActivated = false;

    private void Start()
    {
        if (portalGate == null)
            portalGate = GetComponent<PortalGate_Controller>();

        if (questManager == null)
            questManager = FindFirstObjectByType<QuestManager>();

        CheckPortalState();
    }

    private void Update()
    {
        if (hasActivated)
            return;

        CheckPortalState();
    }

    private void CheckPortalState()
    {
        if (portalGate == null || questManager == null || requiredQuest == null)
            return;

        QuestState state = questManager.GetQuestState(requiredQuest);

        bool shouldActivate = false;

        if (activateOnReadyToComplete)
        {
            shouldActivate = state == QuestState.ReadyToComplete || state == QuestState.Completed;
        }
        else
        {
            shouldActivate = state == QuestState.Completed;
        }

        if (shouldActivate)
        {
            portalGate.F_TogglePortalGate(true);
            hasActivated = true;
            Debug.Log("Portal activated for quest: " + requiredQuest.questName);
        }
    }
}