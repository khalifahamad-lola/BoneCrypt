using UnityEngine;

[CreateAssetMenu(menuName="Items/Potion")]
public class PotionItemData : ItemData
{
    public int healAmount = 20;
    public int maxStack = 3;
}
