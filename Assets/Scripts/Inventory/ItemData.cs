using UnityEngine;

public enum ItemType { Resource, Tool, Consumable }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public int maxStack = 64;
    public int value;
}