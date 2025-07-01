using System.Collections.Generic;

public class ShopModel : ItemContainerBase
{
    public int PlayerCoins { get; private set; }

    public ShopModel(List<ItemSlot> initialItems)
    {
        slots = new List<ItemSlot>(DefaultSlotCount);

        // Add initial items first
        foreach (var item in initialItems)
            slots.Add(item);

        // Fill remaining slots with empty ItemSlots
        int remaining = DefaultSlotCount - slots.Count;
        for (int i = 0; i < remaining; i++)
            slots.Add(new ItemSlot());
    }

    public bool CanBuy(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0) return false;
        return PlayerCoins >= item.buyValue * quantity;
    }

    public bool CanSell(ItemData item, int quantity)
    {
        return item != null && quantity > 0;
    }

    public void CompletePurchase(ItemData item, int quantity)
    {
        PlayerCoins -= item.buyValue * quantity;
        RemoveItem(item, quantity);
    }

    public void CompleteSale(ItemData item, int quantity)
    {
        PlayerCoins += item.sellValue * quantity;
    }

    public void RemoveItem(ItemData item, int quantity = 1)
    {
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                slot.count -= quantity;
                if (slot.count <= 0)
                    slot.Clear();
                break;
            }
        }
    }
}