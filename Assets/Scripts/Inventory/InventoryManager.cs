using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int maxSlots = 20;
    public List<InventorySlot> slots = new();
    public Transform slotParent;

    // Events for buying/selling
    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < maxSlots; i++)
            slots.Add(new InventorySlot());
    }

    // Called by gathering/pickup scripts
    public bool AddItem(ItemData item, int count = 1)
    {
        // Check if stackable
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item && slots[i].count < item.maxStack)
            {
                int space = item.maxStack - slots[i].count;
                int added = Mathf.Min(space, count);
                slots[i].count += added;
                count -= added;

                if (count <= 0) return true;
            }
        }

        // Add to empty slot(s)
        for (int i = 0; i < slots.Count && count > 0; i++)
        {
            if (slots[i].IsEmpty)
            {
                int toAdd = Mathf.Min(item.maxStack, count);
                slots[i].item = item;
                slots[i].count = toAdd;
                count -= toAdd;
            }
        }

        return count <= 0;
    }

    public void RemoveItem(ItemData item, int count = 1)
    {
        for (int i = 0; i < slots.Count && count > 0; i++)
        {
            if (slots[i].item == item)
            {
                int remove = Mathf.Min(count, slots[i].count);
                slots[i].count -= remove;
                count -= remove;

                if (slots[i].count <= 0)
                    slots[i].Clear();
            }
        }
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
        // Swap the data
        InventorySlot temp = slots[indexA];
        slots[indexA] = slots[indexB];
        slots[indexB] = temp;
    }

    public void RefreshAllSlots()
    {
        // Update the UI for each slot
        InventorySlotUI[] slotUIs = slotParent.GetComponentsInChildren<InventorySlotUI>();
        foreach (var slotUI in slotUIs)
        {
            slotUI.UpdateSlot();
        }
    }
}