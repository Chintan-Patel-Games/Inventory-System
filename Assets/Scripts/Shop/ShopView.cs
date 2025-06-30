using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopView : MonoBehaviour
{
    [Header("Slot Setup")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;

    private List<ItemSlotUI> slotUIs = new();

    // Delegates
    private Action<ItemSlot> onBuyRequest;
    private Action<ItemSlot> onSellRequest;

    public void Initialize(Action<ItemSlot> onBuyRequest, Action<ItemSlot> onSellRequest)
    {
        this.onBuyRequest = onBuyRequest;
        this.onSellRequest = onSellRequest;
    }

    public void RefreshUI(IReadOnlyList<ItemSlot> shopSlots)
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        slotUIs.Clear();

        for (int i = 0; i < shopSlots.Count; i++)
        {
            var slot = shopSlots[i];
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            var slotUI = obj.GetComponent<ItemSlotUI>();

            slotUI.ShopSetup(slot, i, onBuyRequest, onSellRequest);
            slotUIs.Add(slotUI);
        }
    }

    public void RefreshAllSlots()
    {
        foreach (var slotUI in slotUIs)
            slotUI.UpdateSlot();
    }
}