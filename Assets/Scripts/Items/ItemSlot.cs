public enum SlotOwner
{
    Inventory,
    Shop
}

[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int count;
    public SlotOwner owner;

    public bool IsEmpty => item == null || count <= 0;

    public void Clear()
    {
        item = null;
        count = 0;
    }
}