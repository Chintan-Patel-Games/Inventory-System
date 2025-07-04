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
    [SerializeField] private Button gatherButton;

    [Header("Stats UI")]
    [SerializeField] private TMP_Text totalWeightText;
    [SerializeField] private TMP_Text totalValueText;

    private List<ItemSlotUI> slotUIs = new();
    private UIManager uiManager;

    // Delegates
    private Action<int, int> onSwapRequest;
    private Action<ItemSlot, int> onSellRequest;
    private Action onGatherClicked;
    private Func<int> getTotalWeight;
    private Func<int> getTotalValue;
    private Func<int> getMaxWeightLimit;

    public void Initialize(Action<int, int> onSwapRequest, Action<ItemSlot, int> onSellRequest, UIManager uiManager)
    {
        this.onSwapRequest = onSwapRequest;
        this.onSellRequest = onSellRequest;
        this.uiManager = uiManager;
    }

    public void LinkGathering(Action onGatherClicked, Func<int> getTotalWeight, Func<int> getTotalValue, Func<int> getMaxWeightLimit)
    {
        this.onGatherClicked = onGatherClicked;
        this.getTotalWeight = getTotalWeight;
        this.getTotalValue = getTotalValue;
        this.getMaxWeightLimit = getMaxWeightLimit;

        gatherButton.onClick.AddListener(OnGatherClicked);
    }

    public void RefreshUI(IReadOnlyList<ItemSlot> inventorySlots)
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        slotUIs.Clear();

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            var slotUI = obj.GetComponent<ItemSlotUI>();

            slotUI.InventorySetup(slot, i, onSwapRequest, onSellRequest);
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

    private void OnGatherClicked()
    {
        if (getTotalWeight == null || getMaxWeightLimit == null) return;

        int totalWeight = getTotalWeight();
        int totalValue = getTotalValue();

        if (totalWeight >= getMaxWeightLimit())
        {
            uiManager.ShowPopup(StringConstants.MAX_WEIGHT_LIMIT_REACHED_POPUP);
            return;
        }

        onGatherClicked?.Invoke();
    }

    private void UpdateStatsUI()
    {
        if (getTotalWeight != null && getMaxWeightLimit != null)
            totalWeightText.text = $"{StringConstants.CURRENT_WEIGHT_LABEL} {getTotalWeight()} / {getMaxWeightLimit()}";

        if (getTotalValue != null)
            totalValueText.text = $"{StringConstants.CURRENT_VALUE_LABEL} {getTotalValue()}";
    }
}