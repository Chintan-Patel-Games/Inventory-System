using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopView view;
    [SerializeField] private QuantityPopupUI quantityPopup;
    [SerializeField] private CurrencyUI currencyUI;

    [Header("Shop Item Generation")]
    [SerializeField] private ItemData[] availableItems;

    private ShopModel model;

    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;

    private void Awake()
    {
        InitializeModel();
        InitializeView();
    }

    private void InitializeModel() => model = new ShopModel(GenerateRandomShopItems(6));

    private void InitializeView()
    {
        view.Initialize(TryBuyItem, TrySellItem);
        view.RefreshUI(model.GetAllSlots());
    }

    private void Start() => currencyUI.SetCoins(model.PlayerCoins);

    private List<ItemSlot> GenerateRandomShopItems(int count)
    {
        List<ItemSlot> result = new();
        HashSet<ItemData> usedItems = new();

        while (result.Count < count && usedItems.Count < availableItems.Length)
        {
            ItemData item = availableItems[UnityEngine.Random.Range(0, availableItems.Length)];

            if (item != null && !usedItems.Contains(item))
            {
                result.Add(new ItemSlot { item = item, count = 1 }); // count will be handled in popup
                usedItems.Add(item);
            }
        }

        return result;
    }

    public void TryBuyItem(ItemSlot shopSlot)
    {
        if (shopSlot == null || shopSlot.item == null || shopSlot.IsEmpty)
            return;

        quantityPopup.Show(shopSlot.item, 1, 1, shopSlot.item.maxStack, BuyItem, isBuying: true);
    }

    private void BuyItem(ItemData item, int quantity)
    {
        if (!model.CanBuy(item, quantity))
        {
            PopupUI.Instance.Show(StringConstants.NOT_ENOUGH_COINS_POPUP);
            return;
        }

        model.CompletePurchase(item, quantity);
        currencyUI.SetCoins(model.PlayerCoins);
        view.RefreshAllSlots();

        OnItemBought?.Invoke(item, quantity);
    }

    public void TrySellItem(ItemSlot inventorySlot)
    {
        if (inventorySlot == null || inventorySlot.item == null || inventorySlot.IsEmpty)
            return;

        quantityPopup.Show(inventorySlot.item, 1, inventorySlot.count, inventorySlot.count, SellItem, isBuying: false);
    }

    private void SellItem(ItemData item, int quantity)
    {
        if (!model.CanSell(item, quantity))
            return;

        model.CompleteSale(item, quantity);
        currencyUI.SetCoins(model.PlayerCoins);

        OnItemSold?.Invoke(item, quantity);  // InventoryController will handle RemoveItem
    }
}