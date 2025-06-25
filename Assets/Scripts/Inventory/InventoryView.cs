using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [Header("Slot Setup")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;

    [Header("Gathering UI")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Button gatherButton;
    [SerializeField] private Button confirmPopupButton;

    [Header("Stats UI")]
    [SerializeField] private TMP_Text totalWeightText;
    [SerializeField] private TMP_Text totalValueText;

    [Header("Rarity Backgrounds")]
    [SerializeField] private Sprite commonBG;
    [SerializeField] private Sprite rareBG;
    [SerializeField] private Sprite epicBG;
    [SerializeField] private Sprite legendaryBG;

    private List<InventorySlotUI> slotUIs = new();

    // Delegates
    private Action<int, int> onSwapRequest;
    private Action onGatherClicked;
    private Func<int> getTotalWeight;
    private Func<int> getTotalValue;
    private Func<int> getMaxWeightLimit;

    public void Initialize(Action<int, int> onSwapRequest) => this.onSwapRequest = onSwapRequest;

    public void LinkGathering(Action onGatherClicked, Func<int> getTotalWeight, Func<int> getTotalValue, Func<int> getMaxWeightLimit)
    {
        this.onGatherClicked = onGatherClicked;
        this.getTotalWeight = getTotalWeight;
        this.getTotalValue = getTotalValue;
        this.getMaxWeightLimit = getMaxWeightLimit;

        gatherButton.onClick.AddListener(OnGatherClicked);
        confirmPopupButton.onClick.AddListener(HidePopup);
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

            slotUI.Setup(slot, i, onSwapRequest, GetRaritySprite);
            slotUIs.Add(slotUI);
        }

        UpdateStatsUI();
    }

    public void RefreshAllSlots()
    {
        foreach (var slotUI in slotUIs)
            slotUI.UpdateSlot();

        UpdateStatsUI();
    }

    public void ShowPopup(string message)
    {
        if (popupPanel != null && popupPanel.TryGetComponent(out TMP_Text popupText))
        {
            popupText.text = message;
            popupPanel.SetActive(true);
        }
        else
        {
            TMP_Text childText = popupPanel?.GetComponentInChildren<TMP_Text>();
            if (childText != null)
            {
                childText.text = message;
                popupPanel.SetActive(true);
            }
        }
    }

    // Private Methods

    private void OnGatherClicked()
    {
        if (getTotalWeight == null || getMaxWeightLimit == null) return;

        int totalWeight = getTotalWeight();
        int totalValue = getTotalValue();

        if (totalWeight >= getMaxWeightLimit())
        {
            ShowPopup(UIConstants.MAXWEIGHTLIMITREACHED_POPUP);
            return;
        }

        onGatherClicked?.Invoke();
    }

    private void HidePopup() => popupPanel.SetActive(false);

    private void UpdateStatsUI()
    {
        if (getTotalWeight != null && getMaxWeightLimit != null)
            totalWeightText.text = $"{UIConstants.CURRENT_WEIGHT_LABEL} {getTotalWeight()} / {getMaxWeightLimit()}";

        if (getTotalValue != null)
            totalValueText.text = $"{UIConstants.CURRENT_VALUE_LABEL} {getTotalValue()}";
    }

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
}