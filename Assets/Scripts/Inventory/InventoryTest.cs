using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    [SerializeField] private InventoryController inventory;
    [SerializeField] private ItemData ironOre;
    [SerializeField] private ItemData potion;
    [SerializeField] private ItemData sword;
    [SerializeField] private ItemData diamond;

    private void Start()
    {
        // Add items to test the inventory logic
        inventory.AddItem(ironOre, 5);    // Stackable
        inventory.AddItem(potion, 3);     // Stackable
        inventory.AddItem(sword, 1);      // Non-stackable
        inventory.AddItem(diamond, 2);    // High rarity
    }
}