using System;
using System.Collections.Generic;

public class InventoryModel : ItemContainerBase
{
    public int MaxSlotCount { get; private set; }
    public int MaxWeightLimit { get; private set; }

    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;
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

    public void RemoveItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0) return;

        for (int i = 0; i < slots.Count && count > 0; i++)
        {
            var slot = slots[i];
            if (slot.item == item)
            {
                int remove = Math.Min(count, slots[i].count);
                slots[i].count -= remove;
                count -= remove;

                if (slots[i].count <= 0)
                    slots[i].Clear();
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public void BuyItem(ItemData item, int quantity)
    {
        if (AddItem(item, quantity))
            OnItemBought?.Invoke(item, quantity);
    }

    public void SellItem(ItemData item, int quantity)
    {
        RemoveItem(item, quantity);
        OnItemSold?.Invoke(item, quantity);
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