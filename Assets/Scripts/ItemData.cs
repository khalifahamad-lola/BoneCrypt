using UnityEngine;

public enum ItemType { Key, Potion, Weapon, Shield }
public enum ItemRarity { Common, Rare, Legendary }

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;

    public int buyPrice;

    public ItemRarity itemRarity;
}