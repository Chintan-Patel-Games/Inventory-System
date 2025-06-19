using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int maxSlots = 24;
    public List<InventorySlot> slots = new();
    public Transform slotParent;
    public GameObject slotPrefab;

    // Events for buying/selling
    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot());

            GameObject obj = Instantiate(slotPrefab, slotParent);
            InventorySlotUI ui = obj.GetComponent<InventorySlotUI>();
            ui.Setup(this, i);
        }
    }

    // Called by gathering/pickup scripts
    public bool AddItem(ItemData item, int count = 1)
    {
        int originalCount = count;

        // Try stacking in existing stacks
        foreach (var slot in slots)
        {
            if (slot.item == item && slot.count < item.maxStack)
            {
                int space = item.maxStack - slot.count;
                int toAdd = Mathf.Min(space, count);
                slot.count += toAdd;
                count -= toAdd;

                if (count <= 0)
                {
                    RefreshAllSlots();
                    return true;
                }
            }
        }

        // Try placing in empty slots
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                int toAdd = Mathf.Min(item.maxStack, count);
                slot.item = item;
                slot.count = toAdd;
                count -= toAdd;

                if (count <= 0)
                {
                    RefreshAllSlots();
                    return true;
                }
            }
        }

        // Partial or no addition
        if (originalCount != count)
            RefreshAllSlots();

        return false; // Not all items could be added
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