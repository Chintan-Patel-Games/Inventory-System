using System.Collections.Generic;

public class ShopModel : ItemContainerBase
{
    public int GoldCoins { get; private set; }

    public ShopModel(List<ItemSlot> initialItems)
    {
        slots = new List<ItemSlot>(DefaultSlotCount);

        // Add initial items first
        foreach (var item in initialItems)
            slots.Add(item);
    }

    public bool CanBuy(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0) return false;
        return GoldCoins >= item.buyValue * quantity;
    }

    public bool CanSell(ItemData item, int quantity) => item != null && quantity > 0;

    public void CompletePurchase(ItemData item, int quantity) => GoldCoins -= item.buyValue * quantity;

    public void CompleteSale(ItemData item, int quantity) => GoldCoins += item.sellValue * quantity;
}