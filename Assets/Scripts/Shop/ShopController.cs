using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PopupUI popup;
    [SerializeField] private ShopView view;
    [SerializeField] private QuantityPopupUI quantityPopup;
    [SerializeField] private CurrencyUI currencyUI;
    [SerializeField] private ToasterUI toasterUI;
    [SerializeField] private InventoryController inventoryController;

    [Header("Shop Item Generation")]
    [SerializeField] private ItemData[] availableItems;

    private ShopModel model;
    private Dictionary<ItemType, List<ItemSlot>> categorizedSlots;

    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;

    private void Awake()
    {
        BuildCategorizedSlots(); // This must come first

        // Create model using default category: Materials
        model = new ShopModel(categorizedSlots.TryGetValue(ItemType.Materials, out var materialSlots)
            ? materialSlots
            : new List<ItemSlot>());

        HandleCategorySelected(ItemType.Materials); // Safe to call now
        InitializeView();
    }

    private void Start() => currencyUI.SetCoinsText(model.GoldCoins);

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

    public void HandleCategorySelected(ItemType selectedType)
    {
        if (categorizedSlots.TryGetValue(selectedType, out List<ItemSlot> filtered))
            view.RefreshUI(filtered);
        else
            view.RefreshUI(new List<ItemSlot>()); // Empty category
    }

    public void TryBuyItem(ItemSlot shopSlot)
    {
        if (shopSlot == null || shopSlot.item == null || shopSlot.IsEmpty) return;

        int maxQty = shopSlot.item.maxStack <= 1 ? 100 : shopSlot.item.maxStack;

        quantityPopup.ShowBuyPopup(shopSlot, maxQty, model.GoldCoins, inventoryController.GetTotalWeight(), inventoryController.GetMaxWeightLimit(), BuyItem);
    }

    private void BuyItem(ItemData item, int quantity)
    {
        if (!model.CanBuy(item, quantity))
        {
            popup.Show(StringConstants.NOT_ENOUGH_COINS_POPUP);
            return;
        }

        model.CompletePurchase(item, quantity);
        currencyUI.SetCoinsText(model.GoldCoins);
        toasterUI.ShowMessage(StringConstants.FormatBuyToaster(quantity, item.itemName));
        view.RefreshAllSlots();

        OnItemBought?.Invoke(item, quantity);
    }

    public void TrySellItem(ItemSlot inventorySlot)
    {
        if (inventorySlot == null || inventorySlot.item == null || inventorySlot.IsEmpty)
            return;

        int totalQuantity = inventoryController.GetTotalQuantityOf(inventorySlot.item);

        quantityPopup.ShowSellPopup(inventorySlot, totalQuantity, SellItem);
    }

    private void SellItem(ItemData item, int quantity)
    {
        if (!model.CanSell(item, quantity))
            return;

        model.CompleteSale(item, quantity);
        currencyUI.SetCoinsText(model.GoldCoins);
        toasterUI.ShowMessage(StringConstants.FormatSellToaster(quantity, item.itemName));
        view.RefreshAllSlots();

        OnItemSold?.Invoke(item, quantity);  // InventoryController will handle RemoveItem
    }
}