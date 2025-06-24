using System;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView view;
    [SerializeField] private GatherManager gatherManager;
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
        view.Initialize(SwapSlots);
        view.LinkGathering(GatherItem, GetTotalWeight, GetTotalValue, GetmaxWeightLimit);
        view.RefreshUI(model.Slots);
    }

    public void RefreshAllSlots() => view.RefreshAllSlots();

    // Public calls from external systems
    public void GatherItem()
    {
        var item = gatherManager.GetRandomItemBasedOnValue(GetTotalValue(), out int quantity);
        if (item != null)
            AddItem(item, quantity);
    }

    public void AddItem(ItemData item, int count = 1) => model.AddItem(item, count);

    public void RemoveItem(ItemData item, int count = 1) => model.RemoveItem(item, count);

    public void BuyItem(ItemData item, int quantity) => model.BuyItem(item, quantity);

    public void SellItem(ItemData item, int quantity) => model.SellItem(item, quantity);

    public void SwapSlots(int indexA, int indexB) => model.SwapSlots(indexA, indexB);

    public InventorySlot GetSlot(int index) => model.GetSlot(index);

    public int GetTotalWeight() => model.GetTotalWeight();

    public int GetTotalValue() => model.GetTotalValue();

    public int GetmaxWeightLimit() => model.GetMaxWeightLimit();
}