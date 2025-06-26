using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopView : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;

    private List<ItemSlotUI> slotUIs = new();

    private Action<ItemSlot> onSlotDragged;

    public void Initialize(List<ItemSlot> shopSlots, Action<ItemSlot> onSlotDragged)
    {
        this.onSlotDragged = onSlotDragged;

        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        slotUIs.Clear();

        for (int i = 0; i < shopSlots.Count; i++)
        {
            var slot = shopSlots[i];
            var obj = Instantiate(slotPrefab, slotContainer);
            var ui = obj.GetComponent<ItemSlotUI>();
            ui.Setup(slot, i, OnSlotDragAttempt);
            slotUIs.Add(ui);
        }
    }

    public void RefreshAllSlots()
    {
        foreach (var slotUI in slotUIs)
            slotUI.UpdateSlot();
    }

    private void OnSlotDragAttempt(int index, int _)
    {
        ItemSlot slot = slotUIs[index].GetSlot();
        onSlotDragged?.Invoke(slot);
    }
}