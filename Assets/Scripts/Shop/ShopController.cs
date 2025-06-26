using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private ShopView view;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private QuantityPopupUI quantityPopup;
    [SerializeField] private CurrencyUI currencyUI;

    private ShopModel model;
    private int playerCurrency = 1000;

    public event Action<ItemData, int> OnItemBought;
    public event Action<ItemData, int> OnItemSold;

    private void Awake()
    {
        // Option 1: Create shop items via ScriptableObjects or predefined setup
        List<ItemSlot> shopItems = new List<ItemSlot>{};

        model = new ShopModel(shopItems);

        view.Initialize(model.GetAllSlots(), TryBuyItem);
    }

    private void Start()
    {
        view.Initialize(model.GetAllSlots(), TryBuyItem);
        currencyUI.SetCoins(playerCurrency);
    }

    //private void OnItemBought(ItemData item, int quantity)
    //{
    //    model.RemoveItem(item, quantity);
    //    view.RefreshAllSlots();
    //}

    private void TryBuyItem(ItemSlot shopSlot)
    {
        quantityPopup.Show(shopSlot.item, 1, shopSlot.item.maxStack, ConfirmBuy);
    }

    private void ConfirmBuy(ItemData item, int quantity)
    {
        int totalCost = item.buyValue * quantity;

        if (playerCurrency < totalCost)
        {
            // Show "Not enough coins" popup
            return;
        }

        bool success = inventoryController.AddItem(item, quantity);

        if (success)
        {
            playerCurrency -= totalCost;
            currencyUI.SetCoins(playerCurrency);

            OnItemBought(item, quantity); // Call it here

            // Show confirmation popup or overlay text
        }
    }

    public void SellItem(ItemData item, int quantity)
    {
        inventoryController.RemoveItem(item, quantity);
        int totalEarned = item.sellValue * quantity;
        playerCurrency += totalEarned;
        currencyUI.SetCoins(playerCurrency);
        // Show confirmation popup or overlay text
    }
}