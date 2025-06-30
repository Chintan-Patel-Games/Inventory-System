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
    public ItemSlot GetSlot(int index) => IsValidIndex(index) ? slots[index] : null;
    public int Count => slots?.Count ?? 0;

    protected bool IsValidIndex(int index) => index >= 0 && index < Count;

    public void ClearAllSlots()
    {
        foreach (var slot in slots)
            slot.Clear();
    }
}