using System;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private int maxSlots = 24;

    private InventoryModel model;

    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;

    private void Awake()
    {
        InitializeModel();
        InitializeView();

        // Subscribe to model events
        model.OnInventoryChanged += RefreshAllSlots;
        model.OnItemBought += (item, quantity) => OnItemBought?.Invoke(item, quantity);
        model.OnItemSold += (item, quantity) => OnItemSold?.Invoke(item, quantity);
    }

    private void InitializeModel() => model = new InventoryModel(maxSlots);

    private void InitializeView()
    {
        inventoryView.Initialize(SwapSlots);
        inventoryView.RefreshUI(model.Slots);
    }

    public void RefreshAllSlots() => inventoryView.RefreshAllSlots();

    // Public calls from external systems
    public void AddItem(ItemData item, int count = 1) => model.AddItem(item, count);

    public void RemoveItem(ItemData item, int count = 1) => model.RemoveItem(item, count);

    public void BuyItem(ItemData item, int quantity) => model.BuyItem(item, quantity);

    public void SellItem(ItemData item, int quantity) => model.SellItem(item, quantity);

    public void SwapSlots(int indexA, int indexB) => model.SwapSlots(indexA, indexB);
}