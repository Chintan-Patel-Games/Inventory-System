using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuantityPopupUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image rarityBG;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text totalPriceText;

    [SerializeField] private Slider quantitySlider;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private ConfirmationPopupUI confirmationPopup;
    [SerializeField] private PopupUI popup;

    private ItemSlot currentItemSlot;
    private int currentQty;
    private int affordableQty;
    private bool isBuying;

    private Action<ItemData, int> onConfirm;

    private void Awake()
    {
        quantitySlider.onValueChanged.AddListener(OnSliderValueChanged);
        increaseButton.onClick.AddListener(IncreaseQuantity);
        decreaseButton.onClick.AddListener(DecreaseQuantity);
        confirmButton.onClick.AddListener(Confirm);
        closeButton.onClick.AddListener(Hide);

        panel.SetActive(false);
    }

    public void ShowBuyPopup(ItemSlot itemSlot, int maxQty, int currentGoldCoins, int currentWeight, int maxWeight, Action<ItemData, int> confirmCallback)
    {
        currentItemSlot = itemSlot;

        // Calculate max quantity based on coins
        int affordableQty = Mathf.FloorToInt((float)currentGoldCoins / itemSlot.item.buyValue);

        // Calculate max quantity based on weight
        int weightLimitQty = Mathf.FloorToInt((maxWeight - currentWeight) / itemSlot.item.weight);

        int sliderMax = Mathf.Min(maxQty, affordableQty, weightLimitQty);

        if (sliderMax <= 0)
        {
            popup.Show(StringConstants.NOT_ENOUGH_SPACE_OR_COINS);
            return;
        }

        SetupSlider(sliderMax, confirmCallback, true);
    }

    public void ShowSellPopup(ItemSlot itemSlot, int maxQty, Action<ItemData, int> confirmCallback)
    {
        currentItemSlot = itemSlot;
        int sliderMax = maxQty;
        SetupSlider(sliderMax, confirmCallback, false);
    }

    private void SetupSlider(int sliderMax, Action<ItemData, int> confirmCallback, bool isBuying)
    {
        currentQty = 1;
        this.isBuying = isBuying;
        onConfirm = confirmCallback;

        quantitySlider.minValue = 1;
        quantitySlider.maxValue = sliderMax;
        quantitySlider.wholeNumbers = true;
        quantitySlider.value = currentQty;

        UpdateUI();
        panel.SetActive(true);
    }

    private void UpdateUI()
    {
        itemIcon.sprite = currentItemSlot.item.icon;
        rarityBG.sprite = currentItemSlot.item.rarityBg;
        rarityBG.color = currentItemSlot.item.rarity == Rarity.VeryCommon ? new Color(1f, 1f, 1f, 0f) : Color.white;

        nameText.text = currentItemSlot.item.itemName;
        descriptionText.text = currentItemSlot.item.description;

        quantityText.text = currentQty.ToString();

        int pricePerItem = isBuying ? currentItemSlot.item.buyValue : currentItemSlot.item.sellValue;
        totalPriceText.text = (pricePerItem * currentQty).ToString();
    }

    private void OnSliderValueChanged(float value)
    {
        currentQty = (int)value;
        UpdateUI();
    }

    private void IncreaseQuantity()
    {
        if (currentQty < affordableQty)
        {
            currentQty++;
            quantitySlider.SetValueWithoutNotify(currentQty);
            UpdateUI();
        }
    }

    private void DecreaseQuantity()
    {
        if (currentQty > quantitySlider.minValue)
        {
            currentQty--;
            quantitySlider.SetValueWithoutNotify(currentQty);
            UpdateUI();
        }
    }

    private void Confirm()
    {
        int totalPrice = (isBuying ? currentItemSlot.item.buyValue : currentItemSlot.item.sellValue) * currentQty;
        string message = isBuying
            ? StringConstants.FormatBuyConfirmation(currentQty, currentItemSlot.item.itemName, totalPrice)
            : StringConstants.FormatSellConfirmation(currentQty, currentItemSlot.item.itemName, totalPrice);

        confirmationPopup.Show(message, () =>
        {
            onConfirm?.Invoke(currentItemSlot.item, currentQty);
            Hide();
        });
    }

    public void Hide()
    {
        panel.SetActive(false);
        onConfirm = null;
        currentItemSlot = null;
    }
}