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
    [SerializeField] private ItemData[] availableItems;  // Assigned via inspector

    private ShopModel model;
    private int playerCurrency = 1000;

    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;

    private void Awake()
    {
        InitializeModel();
        InitializeView();
    }

    private void InitializeModel() => model = new ShopModel(GenerateRandomShopItems(6));

    private void InitializeView() => view.Initialize(model.GetAllSlots(), TryBuyItem, TrySellItem);

    private void Start() => currencyUI.SetCoins(playerCurrency);

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

        quantityPopup.Show(shopSlot.item, 1, shopSlot.item.maxStack, BuyItem, isBuying: true);
    }

    private void BuyItem(ItemData item, int quantity)
    {
        int totalCost = item.buyValue * quantity;

        if (playerCurrency < totalCost)
        {
            PopupUI.Instance.Show(StringConstants.NOT_ENOUGH_COINS_POPUP);
            return;
        }

        OnItemBought?.Invoke(item, quantity);

        playerCurrency -= totalCost;
        currencyUI.SetCoins(playerCurrency);

        model.RemoveItem(item, quantity);
        view.RefreshAllSlots();
    }

    public void TrySellItem(ItemSlot inventorySlot)
    {
        if (inventorySlot == null || inventorySlot.item == null || inventorySlot.IsEmpty)
            return;

        quantityPopup.Show(inventorySlot.item, 1, inventorySlot.count, SellItem, isBuying: false);
    }

    private void SellItem(ItemData item, int quantity)
    {
        OnItemSold?.Invoke(item, quantity);

        int totalEarned = item.sellValue * quantity;
        playerCurrency += totalEarned;
        currencyUI.SetCoins(playerCurrency);
    }
}