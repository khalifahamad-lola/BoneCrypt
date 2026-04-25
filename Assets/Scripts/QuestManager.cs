using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private SoulsWallet soulsWallet;
    [SerializeField] private QuestFeedbackUI questFeedbackUI;

    private Dictionary<QuestData, QuestState> questStates = new Dictionary<QuestData, QuestState>();

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (soulsWallet == null)
            soulsWallet = FindFirstObjectByType<SoulsWallet>();

        if (questFeedbackUI == null)
            questFeedbackUI = FindFirstObjectByType<QuestFeedbackUI>();
    }

    public QuestState GetQuestState(QuestData quest)
    {
        if (quest == null)
            return QuestState.NotStarted;

        if (questStates.TryGetValue(quest, out QuestState state))
            return state;

        return QuestState.NotStarted;
    }

    public void StartQuest(QuestData quest)
    {
        if (quest == null) return;

        QuestState currentState = GetQuestState(quest);

        if (currentState != QuestState.NotStarted)
            return;

        questStates[quest] = QuestState.InProgress;
        Debug.Log("Quest Started: " + quest.questName);

        if (questFeedbackUI != null)
        {
            questFeedbackUI.ShowMessage(
                "New Quest",
                quest.questName + "\n" + quest.questDescription
            );
        }
    }

    public void CheckQuestProgress()
    {
        foreach (var pair in new Dictionary<QuestData, QuestState>(questStates))
        {
            QuestData quest = pair.Key;
            QuestState state = pair.Value;

            if (state != QuestState.InProgress)
                continue;

            if (quest == null || quest.requirements == null || quest.requirements.Count == 0)
                continue;

            bool allRequirementsMet = true;

            foreach (QuestRequirement requirement in quest.requirements)
            {
                if (requirement == null || requirement.item == null)
                {
                    allRequirementsMet = false;
                    break;
                }

                int quantity = playerInventory.GetItemQuantity(requirement.item);

                Debug.Log("Checking quest: " + quest.questName +
                          " | Required item: " + requirement.item.itemName +
                          " | Required amount: " + requirement.amount +
                          " | Player has: " + quantity);

                if (quantity < requirement.amount)
                {
                    allRequirementsMet = false;
                    break;
                }
            }

            if (allRequirementsMet)
            {
                questStates[quest] = QuestState.ReadyToComplete;
                Debug.Log("Quest Ready To Complete: " + quest.questName);

                if (questFeedbackUI != null)
                {
                    questFeedbackUI.ShowMessage(
                        "Quest Updated",
                        quest.questName + "\nReturn to the quest giver."
                    );
                }
            }
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        if (quest == null) return;

        QuestState currentState = GetQuestState(quest);

        if (currentState != QuestState.ReadyToComplete)
            return;

        if (quest.requirements != null)
        {
            foreach (QuestRequirement requirement in quest.requirements)
            {
                if (requirement == null || requirement.item == null)
                    continue;

                for (int i = 0; i < requirement.amount; i++)
                {
                    playerInventory.RemoveOne(requirement.item);
                }
            }
        }

        if (quest.rewardItem != null && quest.rewardItemAmount > 0)
        {
            for (int i = 0; i < quest.rewardItemAmount; i++)
            {
                playerInventory.Add(quest.rewardItem);
            }
        }

        if (quest.rewardXP > 0f && playerStats != null)
        {
            playerStats.GainXp(quest.rewardXP);
        }

        if (quest.rewardSouls > 0 && soulsWallet != null)
        {
            soulsWallet.AddSouls(quest.rewardSouls);
        }

        questStates[quest] = QuestState.Completed;
        Debug.Log("Quest Completed: " + quest.questName);

        if (questFeedbackUI != null)
        {
            questFeedbackUI.ShowMessage(
                "Quest Completed",
                BuildCompletionMessage(quest)
            );
        }
    }

    private string BuildCompletionMessage(QuestData quest)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(quest.questName);

        string rewardLine = "Rewards: ";
        bool hasAnyReward = false;

        if (quest.rewardItem != null && quest.rewardItemAmount > 0)
        {
            rewardLine += quest.rewardItem.itemName + " x" + quest.rewardItemAmount;
            hasAnyReward = true;
        }

        if (quest.rewardXP > 0f)
        {
            if (hasAnyReward) rewardLine += " | ";
            rewardLine += "XP +" + quest.rewardXP;
            hasAnyReward = true;
        }

        if (quest.rewardSouls > 0)
        {
            if (hasAnyReward) rewardLine += " | ";
            rewardLine += "Souls +" + quest.rewardSouls;
            hasAnyReward = true;
        }

        if (hasAnyReward)
            sb.AppendLine(rewardLine);
        else
            sb.AppendLine("Objective complete.");

        return sb.ToString().TrimEnd();
    }
}