using UnityEngine;
using System.Collections.Generic;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject popupPanel;

    [Header("Rarity Backgrounds")]
    [SerializeField] private Sprite commonBG;
    [SerializeField] private Sprite rareBG;
    [SerializeField] private Sprite epicBG;
    [SerializeField] private Sprite legendaryBG;

    private List<InventorySlotUI> slotUIs = new();
    private System.Action<int, int> onSwapRequest;

    public void Initialize(System.Action<int, int> onSwapRequest) => this.onSwapRequest = onSwapRequest;

    public Sprite GetRaritySprite(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => commonBG,
            Rarity.Rare => rareBG,
            Rarity.Epic => epicBG,
            Rarity.Legendary => legendaryBG,
            _ => null
        };
    }

    public void RefreshUI(List<InventorySlot> inventorySlots)
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        slotUIs.Clear();

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            var slotUI = obj.GetComponent<InventorySlotUI>();

            // pass this view’s GetRaritySprite directly
            slotUI.Setup(slot, i, onSwapRequest, GetRaritySprite);

            slotUIs.Add(slotUI);
        }
    }

    public void RefreshAllSlots()
    {
        foreach (var slotUI in slotUIs)
            slotUI.UpdateSlot();
    }
}
