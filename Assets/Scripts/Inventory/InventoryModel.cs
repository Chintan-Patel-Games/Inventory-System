using System;
using System.Collections.Generic;

public class InventoryModel : ItemContainerBase
{
    public int MaxSlotCount { get; private set; }
    public int MaxWeightLimit { get; private set; }
    public event Action OnInventoryChanged;

    public InventoryModel(int slotCount, int maxWeightLimit)
    {
        MaxSlotCount = slotCount;
        MaxWeightLimit = maxWeightLimit;
        slots = new List<ItemSlot>(slotCount);

        for (int i = 0; i < slotCount; i++)
            slots.Add(new ItemSlot());
    }

    public bool AddItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0)
            return false;

        bool inventoryChanged = false;

        // Try stacking into existing slots
        foreach (var slot in slots)
        {
            if (slot.item == item && slot.count < item.maxStack)
            {
                int space = item.maxStack - slot.count;
                int toAdd = Math.Min(space, count);
                slot.count += toAdd;
                count -= toAdd;
                inventoryChanged = true;

                if (count <= 0)
                    break;
            }
        }

        // Try filling empty slots
        if (count > 0)
        {
            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    int toAdd = Math.Min(item.maxStack, count);
                    slot.item = item;
                    slot.count = toAdd;
                    count -= toAdd;
                    inventoryChanged = true;

                    if (count <= 0)
                        break;
                }
            }
        }

        if (inventoryChanged)
            OnInventoryChanged?.Invoke();

        // Return true if all items were added, false if some couldn't be added
        return count == 0;
    }

    public void RemoveItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return;

        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                int remove = Math.Min(quantity, slot.count);
                slot.count -= remove;
                quantity -= remove;

                if (slot.count <= 0)
                    slot.Clear();
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public void SwapSlots(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB) || indexA == indexB)
            return;

        var slotA = slots[indexA];
        var slotB = slots[indexB];

        // Swap item and count, not the slot reference
        (slotA.item, slotB.item) = (slotB.item, slotA.item);
        (slotA.count, slotB.count) = (slotB.count, slotA.count);

        OnInventoryChanged?.Invoke();
    }

    public int GetTotalWeight()
    {
        int totalWeight = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
                totalWeight += slot.item.weight * slot.count;
        }
        return totalWeight;
    }

    public int GetTotalValue()
    {
        int totalValue = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
                totalValue += slot.item.sellValue * slot.count;
        }
        return totalValue;
    }

    public int GetMaxWeightLimit() => MaxWeightLimit;
}