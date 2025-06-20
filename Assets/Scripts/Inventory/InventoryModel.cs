using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    public List<InventorySlot> Slots { get; private set; }
    public int MaxSlotCount { get; private set; }

    public InventoryModel(int slotCount)
    {
        MaxSlotCount = slotCount;
        Slots = new List<InventorySlot>(slotCount);
        for (int i = 0; i < slotCount; i++)
        {
            Slots.Add(new InventorySlot());
        }
    }

    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= Slots.Count) return null;
        return Slots[index];
    }
}