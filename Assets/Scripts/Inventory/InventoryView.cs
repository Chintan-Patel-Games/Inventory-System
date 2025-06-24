using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Button gatherButton;
    [SerializeField] private Button confirmPopupButton;

    [Header("Rarity Backgrounds")]
    [SerializeField] private Sprite commonBG;
    [SerializeField] private Sprite rareBG;
    [SerializeField] private Sprite epicBG;
    [SerializeField] private Sprite legendaryBG;

    private List<InventorySlotUI> slotUIs = new();

    private Action<int, int> onSwapRequest;
    private Action onGatherClicked;
    private Func<int> getTotalWeight;
    private Func<int> getTotalValue;
    private Func<int> getmaxWeightLimit;

    public void Initialize(Action<int, int> onSwapRequest) => this.onSwapRequest = onSwapRequest;

    public void LinkGathering(Action gatherAction, Func<int> getWeight, Func<int> getValue, Func<int> getWeightLimit)
    {
        onGatherClicked = gatherAction;
        getTotalWeight = getWeight;
        getTotalValue = getValue;
        getmaxWeightLimit = getWeightLimit;

        gatherButton.onClick.AddListener(OnGatherClicked);
        confirmPopupButton.onClick.AddListener(HidePopup);
    }

    private void OnGatherClicked()
    {
        int totalWeight = getTotalWeight();
        int totalValue = getTotalValue();

        if (totalWeight >= getmaxWeightLimit())
        {
            popupPanel.SetActive(true);
            return;
        }

        onGatherClicked?.Invoke();
    }

    private void HidePopup() => popupPanel.SetActive(false);

    private Sprite GetRaritySprite(Rarity rarity)
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
