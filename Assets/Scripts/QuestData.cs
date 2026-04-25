using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    NotStarted,
    InProgress,
    ReadyToComplete,
    Completed
}

[System.Serializable]
public class QuestRequirement
{
    public ItemData item;
    public int amount = 1;
}

[CreateAssetMenu(menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questId;
    public string questName;
    [TextArea(2, 5)] public string questDescription;

    [Header("Required Items")]
    public List<QuestRequirement> requirements = new List<QuestRequirement>();

    [Header("Rewards")]
    public ItemData rewardItem;
    public int rewardItemAmount = 0;
    public float rewardXP = 0f;
    public int rewardSouls = 0;
}