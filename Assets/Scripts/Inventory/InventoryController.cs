using GDS.Basic;
using System;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView view;
    [SerializeField] private GatherManager gatherManager;
    [SerializeField] private int maxSlots;
    [SerializeField] private int maxWeightLimit;

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

    private void InitializeModel() => model = new InventoryModel(maxSlots, maxWeightLimit);

    private void InitializeView()
    {
        view.Initialize(SwapSlots);
        view.LinkGathering(TryGatherItem, GetTotalWeight, GetTotalValue, GetmaxWeightLimit);
        view.RefreshUI(model.Slots);
    }

    public void RefreshAllSlots() => view.RefreshAllSlots();

    // Public calls from external systems
    public void TryGatherItem()
    {
        int totalWeight = GetTotalWeight();

        if (totalWeight >= GetmaxWeightLimit())
        {
            view.ShowPopup("You cannot carry more weight!");
            return;
        }

        ItemData item = gatherManager.GetRandomItemBasedOnValue(GetTotalValue(), out int quantity);
        if (item == null)
            return;

        bool added = AddItem(item, item.maxStack >= 10 ? 10 : 1);
        if (!added)
            view.ShowPopup("No available slots in inventory!");
    }

    public bool AddItem(ItemData item, int count = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to add a null item.");
            return false;
        }

        bool success = model.AddItem(item, count);

        if (!success)
        {
            view.ShowPopup("No available space for this item.");
        }

        return success;
    }


    public void RemoveItem(ItemData item, int count = 1) => model.RemoveItem(item, count);

    public void BuyItem(ItemData item, int quantity) => model.BuyItem(item, quantity);

    public void SellItem(ItemData item, int quantity) => model.SellItem(item, quantity);

    public void SwapSlots(int indexA, int indexB) => model.SwapSlots(indexA, indexB);

    public InventorySlot GetSlot(int index) => model.GetSlot(index);

    public int GetTotalWeight() => model.GetTotalWeight();

    public int GetTotalValue() => model.GetTotalValue();

    public int GetmaxWeightLimit() => model.GetMaxWeightLimit();
}