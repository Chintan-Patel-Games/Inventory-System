using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView view;
    [SerializeField] private GatherManager gatherManager;
    [SerializeField] private ShopController shopController;
    [SerializeField] private int maxWeightLimit = 500;

    private InventoryModel model;

    private void Awake()
    {
        InitializeModel();
        InitializeView();

        model.OnInventoryChanged += RefreshAllSlots;

        gatherManager.Initialize(GetTotalLifetimeValue, GetTotalWeight, GetMaxWeightLimit);
        gatherManager.OnItemGathered += HandleItemGathered;

        // Subscribe to shop events
        shopController.OnItemBought += HandleItemBought;
        shopController.OnItemSold += HandleItemSold;
    }

    private void InitializeModel() => model = new InventoryModel(maxWeightLimit);

    private void InitializeView()
    {
        view.Initialize(SwapSlots, shopController.TrySellItem);
        view.LinkGathering(gatherManager.Gather, GetTotalWeight, GetTotalLifetimeValue, GetMaxWeightLimit);
        view.RefreshUI(model.GetAllSlots());
    }

    public void RefreshAllSlots() => view.RefreshAllSlots();

    private void HandleItemGathered(ItemData item, int quantity)
    {
        bool added = AddItem(item, quantity);
        if (!added)
            PopupUI.Instance.Show(StringConstants.INVENTORY_FULL_POPUP);
    }

    private void HandleItemBought(ItemData item, int quantity)
    {
        bool added = AddItem(item, quantity);

        if (!added)
            PopupUI.Instance.Show(StringConstants.INVENTORY_FULL_POPUP);
    }

    private void HandleItemSold(ItemData item, int quantity) => RemoveItem(item, quantity);

    public bool AddItem(ItemData item, int count = 1)
    {
        if (item == null) return false;

        bool success = model.AddItem(item, count);

        if (!success) PopupUI.Instance.Show(StringConstants.INVENTORY_FULL_POPUP);

        return success;
    }

    public void RemoveItem(ItemData item, int count = 1) => model.RemoveItem(item, count);

    public void SwapSlots(int indexA, int indexB) => model.SwapSlots(indexA, indexB);

    public ItemSlot GetSlot(int index) => model.GetSlot(index);

    public int GetTotalWeight() => model.GetTotalWeight();

    public int GetTotalLifetimeValue() => model.GetTotalLifetimeValue();

    public int GetMaxWeightLimit() => model.MaxWeightLimit;
}