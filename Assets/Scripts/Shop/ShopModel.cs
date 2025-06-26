using System.Collections.Generic;

public class ShopModel : ItemContainerBase
{
    public ShopModel(List<ItemSlot> initialItems) => slots = new List<ItemSlot>(initialItems);

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