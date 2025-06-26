using System;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView view;
    [SerializeField] private GatherManager gatherManager;
    [SerializeField] private int maxSlots = 30;
    [SerializeField] private int maxWeightLimit = 500;

    private InventoryModel model;

    private void Awake()
    {
        InitializeModel();
        InitializeView();

        model.OnInventoryChanged += RefreshAllSlots;
    }

    private void InitializeModel() => model = new InventoryModel(maxSlots, maxWeightLimit);

    private void InitializeView()
    {
        view.Initialize(SwapSlots);
        view.LinkGathering(TryGatherItem, GetTotalWeight, GetTotalValue, GetMaxWeightLimit);
        view.RefreshUI(model.GetAllSlots());
    }

    public void RefreshAllSlots() => view.RefreshAllSlots();

    public void TryGatherItem()
    {
        ItemData item = gatherManager.GetRandomItemBasedOnValue(GetTotalValue(), out int quantity);
        if (item == null) return;

        int stackSize = item.maxStack >= 10 ? 10 : 1;
        int totalWeightIfAdded = GetTotalWeight() + (item.weight * stackSize);

        if (totalWeightIfAdded > GetMaxWeightLimit())
        {
            PopupUI.Instance.Show(StringConstants.MAXWEIGHTLIMITREACHED_POPUP);
            return;
        }

        bool added = AddItem(item, stackSize);
        if (!added)
            PopupUI.Instance.Show(StringConstants.NOAVAILABLESLOTS_POPUP);
    }

    public bool AddItem(ItemData item, int count = 1)
    {
        if (item == null) return false;

        bool success = model.AddItem(item, count);

        if (!success) PopupUI.Instance.Show(StringConstants.NOAVAILABLESLOTS_POPUP);

        return success;
    }

    public void RemoveItem(ItemData item, int count = 1) => model.RemoveItem(item, count);

    public void BuyItem(ItemData item, int quantity) => model.BuyItem(item, quantity);

    public void SellItem(ItemData item, int quantity) => model.SellItem(item, quantity);

    public void SwapSlots(int indexA, int indexB) => model.SwapSlots(indexA, indexB);

    public ItemSlot GetSlot(int index) => model.GetSlot(index);

    public int GetTotalWeight() => model.GetTotalWeight();

    public int GetTotalValue() => model.GetTotalValue();

    public int GetMaxWeightLimit() => model.GetMaxWeightLimit();
}