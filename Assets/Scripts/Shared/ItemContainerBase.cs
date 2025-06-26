using System.Collections.Generic;

public abstract class ItemContainerBase
{
    protected List<ItemSlot> slots;

    public List<ItemSlot> GetAllSlots() => slots;

    public ItemSlot GetSlot(int index) => IsValidIndex(index) ? slots[index] : null;

    protected bool IsValidIndex(int index) => index >= 0 && index < slots.Count;
}