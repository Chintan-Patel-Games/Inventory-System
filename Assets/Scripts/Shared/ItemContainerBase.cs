using System.Collections.Generic;

public abstract class ItemContainerBase
{
    protected const int DefaultSlotCount = 30;
    protected List<ItemSlot> slots;

    protected ItemContainerBase()
    {
        slots = new List<ItemSlot>(DefaultSlotCount);
        for (int i = 0; i < DefaultSlotCount; i++)
            slots.Add(new ItemSlot());
    }

    public IReadOnlyList<ItemSlot> GetAllSlots() => slots;

    public int Count => slots?.Count ?? 0;

    protected bool IsValidIndex(int index) => index >= 0 && index < Count;

    public int GetTotalQuantityOf(ItemData item)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.item == item)
                total += slot.count;
        }
        return total;
    }
}