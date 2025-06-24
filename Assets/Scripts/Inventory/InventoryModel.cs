using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    public List<InventorySlot> Slots { get; private set; }
    public int MaxSlotCount { get; private set; }
    private const int maxWeightLimit = 100;

    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;
    public event Action OnInventoryChanged;

    public InventoryModel(int slotCount)
    {
        MaxSlotCount = slotCount;
        Slots = new List<InventorySlot>(slotCount);
        for (int i = 0; i < slotCount; i++)
            Slots.Add(new InventorySlot());
    }

    public bool AddItem(ItemData item, int count = 1)
    {
        int originalCount = count;
        bool changed = false;

        // Try stacking into existing slots
        foreach (var slot in Slots)
        {
            if (slot.item == item && slot.count < item.maxStack)
            {
                int space = item.maxStack - slot.count;
                int toAdd = Math.Min(space, count);
                slot.count += toAdd;
                count -= toAdd;
                changed = true;

                if (count <= 0)
                    break;
            }
        }

        // Try filling empty slots
        if (count > 0)
        {
            foreach (var slot in Slots)
            {
                if (slot.IsEmpty)
                {
                    int toAdd = Math.Min(item.maxStack, count);
                    slot.item = item;
                    slot.count = toAdd;
                    count -= toAdd;
                    changed = true;

                    if (count <= 0)
                        break;
                }
            }
        }

        if (changed)
            OnInventoryChanged?.Invoke();

        // Return true if all items were added, false if some couldn't be added
        return count == 0;
    }

    public void RemoveItem(ItemData item, int count = 1)
    {
        for (int i = 0; i < Slots.Count && count > 0; i++)
        {
            if (Slots[i].item == item)
            {
                int remove = Math.Min(count, Slots[i].count);
                Slots[i].count -= remove;
                count -= remove;

                if (Slots[i].count <= 0)
                    Slots[i].Clear();
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
        var slotA = Slots[indexA];
        var slotB = Slots[indexB];

        // Swap item and count, not the slot reference
        (slotA.item, slotB.item) = (slotB.item, slotA.item);
        (slotA.count, slotB.count) = (slotB.count, slotA.count);

        OnInventoryChanged?.Invoke();
    }

    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= Slots.Count) return null;
        return Slots[index];
    }

    public int GetTotalWeight()
    {
        int totalWeight = 0;
        foreach (var slot in Slots)
        {
            if (!slot.IsEmpty)
                totalWeight += slot.item.weight * slot.count;
        }
        return totalWeight;
    }

    public int GetTotalValue()
    {
        int totalValue = 0;
        foreach (var slot in Slots)
        {
            if (!slot.IsEmpty)
                totalValue += slot.item.sellValue * slot.count;
        }
        return totalValue;
    }

    public int GetMaxWeightLimit() => maxWeightLimit;
}