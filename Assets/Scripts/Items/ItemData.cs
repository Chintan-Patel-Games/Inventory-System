using UnityEngine;

public enum ItemType { Materials, Weapons, Consumables, Treasure }
public enum Rarity { VeryCommon, Common, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public Sprite icon;
    public string description;
    public int buyValue;
    public int sellValue;
    public int weight;
    public Rarity rarity;
    public int quantity;
    public int maxStack;
}