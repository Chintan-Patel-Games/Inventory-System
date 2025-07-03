using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    [Header("Slot Setup")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;

    [Header("Item Category Buttons")]
    [SerializeField] private Button materialItems;
    [SerializeField] private Button weaponItems;
    [SerializeField] private Button consumableItems;
    [SerializeField] private Button treasureItems;

    private List<ItemSlotUI> slotUIs = new();

    // Delegates
    private Action<ItemType> OnCategorySelected;
    private Action<ItemSlot> onBuyRequest;
    private Action<ItemSlot> onSellRequest;

    public void Initialize(Action<ItemType> onCategorySelected, Action<ItemSlot> onBuyRequest, Action<ItemSlot> onSellRequest)
    {
        this.OnCategorySelected = onCategorySelected;
        this.onBuyRequest = onBuyRequest;
        this.onSellRequest = onSellRequest;

        materialItems.onClick.AddListener(() => OnCategorySelected?.Invoke(ItemType.Materials));
        weaponItems.onClick.AddListener(() => OnCategorySelected?.Invoke(ItemType.Weapons));
        consumableItems.onClick.AddListener(() => OnCategorySelected?.Invoke(ItemType.Consumables));
        treasureItems.onClick.AddListener(() => OnCategorySelected?.Invoke(ItemType.Treasure));
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