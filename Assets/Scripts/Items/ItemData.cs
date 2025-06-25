using UnityEngine;

public enum ItemType
{
    Materials,
    Weapons,
    Consumables,
    Treasur
}
public enum Rarity
{
    VeryCommon,
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public ItemType type;
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite icon;

    [Header("Stats")]
    public int buyValue;
    public int sellValue;
    public int weight;
    public Rarity rarity;
    public int maxStack;
}