using System;

public class InventoryModel : ItemContainerBase
{
    public int MaxWeightLimit { get; private set; }

    private int totalLifetimeValue = 0;

    // Event to notify when inventory changes (items added/removed/swapped)
    public event Action OnInventoryChanged;

    public InventoryModel(int maxWeightLimit) => MaxWeightLimit = maxWeightLimit;

    public bool AddItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0)
            return false;

        bool inventoryChanged = false;
        int originalCount = count; // Track how many we tried to add

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

        int addedAmount = originalCount - count; // this is the actual added quantity
        if (addedAmount > 0)
            totalLifetimeValue += item.sellValue * addedAmount;

        if (inventoryChanged)
            OnInventoryChanged?.Invoke();

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

    public int GetTotalLifetimeValue() => totalLifetimeValue;
}