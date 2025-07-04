using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopView view;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private UIManager uiManager;

    [Header("Shop Item Generation")]
    [SerializeField] private ItemData[] availableItems;

    private ShopModel model;
    private Dictionary<ItemType, List<ItemSlot>> categorizedSlots;

    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int, int> OnItemSold;

    private void Awake()
    {
        BuildCategorizedSlots(); // This must come first

        // Create model using default category: Materials
        model = new ShopModel(categorizedSlots.TryGetValue(ItemType.Materials, out var materialSlots)
            ? materialSlots
            : new List<ItemSlot>());

        HandleCategorySelected(ItemType.Materials, false); // Safe to call now
        InitializeView();
    }

    private void Start() => uiManager.ShowCurrency(model.GoldCoins);

    public void Initialize(InventoryController inventory) => inventoryController = inventory;

    private void InitializeView()
    {
        view.Initialize(HandleCategorySelected, TryBuyItem, TrySellItem);
        view.RefreshUI(model.GetAllSlots());
    }

    private void BuildCategorizedSlots()
    {
        categorizedSlots = new Dictionary<ItemType, List<ItemSlot>>();

        foreach (var item in availableItems)
        {
            if (item == null) continue;

            if (!categorizedSlots.ContainsKey(item.type))
                categorizedSlots[item.type] = new List<ItemSlot>();

            categorizedSlots[item.type].Add(new ItemSlot { item = item, count = 1 });
        }

        // Fill all categories to 30 slots
        foreach (var key in categorizedSlots.Keys)
        {
            int remaining = 30 - categorizedSlots[key].Count;
            for (int i = 0; i < remaining; i++)
                categorizedSlots[key].Add(new ItemSlot());
        }
    }

    public void HandleCategorySelected(ItemType selectedType, bool playSound = true)
    {
        if (playSound)
            SoundManager.Instance.PlayUIClick();

        if (categorizedSlots.TryGetValue(selectedType, out List<ItemSlot> filtered))
            view.RefreshUI(filtered);
        else
            view.RefreshUI(new List<ItemSlot>()); // Empty category
    }

    public void TryBuyItem(ItemSlot shopSlot)
    {
        if (shopSlot == null || shopSlot.item == null || shopSlot.IsEmpty) return;
        int maxQty = shopSlot.item.maxStack <= 1 ? 100 : shopSlot.item.maxStack;
        uiManager.ShowBuyQuantityPopup(shopSlot, maxQty, model.GoldCoins, inventoryController.GetTotalWeight(), inventoryController.GetMaxWeightLimit(), BuyItem);
    }

    private void BuyItem(ItemData item, int quantity)
    {
        if (!model.CanBuy(item, quantity)) return;

        model.CompletePurchase(item, quantity);
        SoundManager.Instance.PlayBuySound();
        uiManager.ShowCurrency(model.GoldCoins);
        uiManager.ShowToaster(StringConstants.FormatBuyToaster(quantity, item.itemName));
        view.RefreshAllSlots();

        OnItemBought?.Invoke(item, quantity);
    }

    public void TrySellItem(ItemSlot inventorySlot, int slotIndex)
    {
        if (inventorySlot == null || inventorySlot.item == null || inventorySlot.IsEmpty) return;
        int totalQuantity = inventoryController.GetTotalQuantityOf(inventorySlot.item);
        uiManager.ShowSellQuantityPopup(inventorySlot, totalQuantity, (item, qty) => SellItem(item, qty, slotIndex));
    }

    private void SellItem(ItemData item, int quantity, int slotIndex)
    {
        if (!model.CanSell(item, quantity))
            return;

        model.CompleteSale(item, quantity);
        SoundManager.Instance.PlaySellSound();
        uiManager.ShowCurrency(model.GoldCoins);
        uiManager.ShowToaster(StringConstants.FormatSellToaster(quantity, item.itemName));
        view.RefreshAllSlots();

        OnItemSold?.Invoke(item, slotIndex, quantity);
    }
}