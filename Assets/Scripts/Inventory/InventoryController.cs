using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView view;
    [SerializeField] private GatherManager gatherManager;
    [SerializeField] private ShopController shopController;
    [SerializeField] private UIManager uiManager;
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
        view.Initialize(SwapSlots, shopController.TrySellItem, uiManager);
        view.LinkGathering(gatherManager.Gather, GetTotalWeight, GetTotalLifetimeValue, GetMaxWeightLimit);
        view.RefreshUI(model.GetAllSlots());
    }

    public void RefreshAllSlots() => view.RefreshAllSlots();

    private void HandleItemGathered(ItemData item, int quantity) => AddItem(item, quantity);

    private void HandleItemBought(ItemData item, int quantity) => AddItem(item, quantity);

    public void HandleItemSold(ItemData item, int slotIndex, int quantity) => RemoveItem(item, slotIndex, quantity);

    public bool AddItem(ItemData item, int count = 1)
    {
        if (item == null) return false;

        bool success = model.AddItem(item, count);

        if (!success) uiManager.ShowPopup(StringConstants.INVENTORY_FULL_POPUP, 2f);

        return success;
    }

    public void RemoveItem(ItemData item, int specificSlotIndex, int count = 1) => model.RemoveItem(item, specificSlotIndex, count);

    public void SwapSlots(int indexA, int indexB) => model.SwapSlots(indexA, indexB);

    public int GetTotalWeight() => model.GetTotalWeight();

    public int GetTotalLifetimeValue() => model.GetTotalLifetimeValue();

    public int GetMaxWeightLimit() => model.MaxWeightLimit;

    public int GetTotalQuantityOf(ItemData item) => model.GetTotalQuantityOf(item);
}