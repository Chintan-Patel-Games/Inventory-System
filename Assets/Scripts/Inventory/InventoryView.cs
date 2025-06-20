using UnityEngine;
using System.Collections.Generic;

public class InventoryView : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotContainer;
    public GameObject popupPanel;

    private List<InventorySlotUI> slotUIs = new();

    private System.Func<Rarity, Sprite> getRaritySprite;
    private System.Action<int, int> onSwapRequest;

    public void Initialize(System.Action<int, int> onSwapRequest, System.Func<Rarity, Sprite> getRaritySprite)
    {
        this.onSwapRequest = onSwapRequest;
        this.getRaritySprite = getRaritySprite;
    }

    public void RefreshUI(List<InventorySlot> inventorySlots)
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        slotUIs.Clear();

        System.Action refresh = () => RefreshUI(inventorySlots);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            var slotUI = obj.GetComponent<InventorySlotUI>();

            slotUI.Setup(slot, i, onSwapRequest, getRaritySprite);

            slotUIs.Add(slotUI);
        }
    }

    public void RefreshAllSlots()
    {
        foreach (var slotUI in slotUIs)
            slotUI.UpdateSlot();
    }
}