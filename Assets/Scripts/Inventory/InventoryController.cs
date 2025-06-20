using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public int maxSlots = 24;

    private InventoryModel model;

    [Header("Rarity Backgrounds")]
    public Sprite commonBG;
    public Sprite rareBG;
    public Sprite epicBG;
    public Sprite legendaryBG;

    // Events for buying/selling
    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;

    private void Awake()
    {
        InitializeModel();
        InitializeView();
    }

    private void InitializeModel()
    {
        model = new InventoryModel(maxSlots);
    }

    private void InitializeView()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            InventorySlotUI ui = obj.GetComponent<InventorySlotUI>();
            ui.Setup(model.GetSlot(i), i, SwapSlots, GetRaritySprite);
        }
    }

    public Sprite GetRaritySprite(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => commonBG,
            Rarity.Rare => rareBG,
            Rarity.Epic => epicBG,
            Rarity.Legendary => legendaryBG,
            _ => null
        };
    }

    // Called by gathering/pickup scripts
    public bool AddItem(ItemData item, int count = 1)
    {
        int originalCount = count;

        // Try stacking in existing stacks
        foreach (var slot in model.Slots)
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
        foreach (var slot in model.Slots)
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
        for (int i = 0; i < model.Slots.Count && count > 0; i++)
        {
            if (model.Slots[i].item == item)
            {
                int remove = Mathf.Min(count, model.Slots[i].count);
                model.Slots[i].count -= remove;
                count -= remove;

                if (model.Slots[i].count <= 0)
                    model.Slots[i].Clear();
            }
        }

        RefreshAllSlots();
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
        var temp = model.Slots[indexA];
        model.Slots[indexA] = model.Slots[indexB];
        model.Slots[indexB] = temp;

        RefreshAllSlots();
    }

    public void RefreshAllSlots()
    {
        // Update the UI for each slot
        InventorySlotUI[] slotUIs = slotParent.GetComponentsInChildren<InventorySlotUI>();
        foreach (var slotUI in slotUIs)
            slotUI.UpdateSlot();
    }
}