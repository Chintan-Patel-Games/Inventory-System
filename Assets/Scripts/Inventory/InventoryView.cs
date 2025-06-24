using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
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

    private Action<int, int> onSwapRequest;
    private Action onGatherClicked;
    private Func<int> getTotalWeight;
    private Func<int> getTotalValue;
    private Func<int> getmaxWeightLimit;

    public void Initialize(Action<int, int> onSwapRequest) => this.onSwapRequest = onSwapRequest;

    public void LinkGathering(Action onGatherClicked, Func<int> getTotalWeight, Func<int> getTotalValue, Func<int> getmaxWeightLimit)
    {
        this.onGatherClicked = onGatherClicked;
        this.getTotalWeight = getTotalWeight;
        this.getTotalValue = getTotalValue;
        this.getmaxWeightLimit = getmaxWeightLimit;

        gatherButton.onClick.AddListener(OnGatherClicked);
        confirmPopupButton.onClick.AddListener(HidePopup);
    }

    private void OnGatherClicked()
    {
        int totalWeight = getTotalWeight();
        int totalValue = getTotalValue();

        if (totalWeight >= getmaxWeightLimit())
        {
            ShowPopup("You cannot carry more weight!");
            return;
        }

        onGatherClicked?.Invoke();
    }

    public void ShowPopup(string message)
    {
        TMP_Text popupText = popupPanel.GetComponentInChildren<TMP_Text>();

        if (popupText != null)
        {
            popupText.text = message;
            popupPanel.SetActive(true);
        }
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

        UpdateStatsUI();
    }

    public void RefreshAllSlots()
    {
        foreach (var slotUI in slotUIs)
            slotUI.UpdateSlot();

        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (getTotalWeight != null && getmaxWeightLimit != null)
            totalWeightText.text = $"Weight {getTotalWeight()} / {getmaxWeightLimit()}";

        if (getTotalValue != null)
            totalValueText.text = $"Value {getTotalValue()}";
    }
}
