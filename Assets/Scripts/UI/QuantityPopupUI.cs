using System;
using TMPro;
using Unity.VisualScripting;
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

    private int currentQuantity;
    private int maxQuantity;
    private ItemData currentItem;
    private Action<ItemData, int> onConfirm;
    private bool isBuying;

    public void Show(ItemData item, int startQty, int totalQnt, int maxQty, Action<ItemData, int> confirmCallback, bool isBuying = true)
    {
        currentItem = item;
        currentQuantity = Mathf.Clamp(startQty, totalQnt, maxQty);
        maxQuantity = maxQty;

        // Setting up the slider
        quantitySlider.minValue = 1;
        quantitySlider.maxValue = maxQuantity;
        quantitySlider.wholeNumbers = true;
        quantitySlider.value = currentQuantity;

        onConfirm = confirmCallback;
        this.isBuying = isBuying;

        UpdateUI();
        panel.SetActive(true);
    }

    private void Awake()
    {
        quantitySlider.onValueChanged.AddListener(OnSliderValueChanged);
        increaseButton.onClick.AddListener(IncreaseQuantity);
        decreaseButton.onClick.AddListener(DecreaseQuantity);
        confirmButton.onClick.AddListener(Confirm);
        closeButton.onClick.AddListener(Hide);

        panel.SetActive(false);
    }

    private void UpdateUI()
    {
        itemIcon.sprite = currentItem.icon;
        rarityBG.sprite = currentItem.rarityBg;
        rarityBG.color = currentItem.rarity == Rarity.VeryCommon ? new Color(1f, 1f, 1f, 0f) : Color.white;
        nameText.text = currentItem.itemName;
        descriptionText.text = currentItem.description;
        quantitySlider.value = currentQuantity;

        quantityText.text = currentQuantity.ToString();
        int pricePerItem = isBuying ? currentItem.buyValue : currentItem.sellValue;
        totalPriceText.text = (pricePerItem * currentQuantity).ToString();
    }

    private void OnSliderValueChanged(float value)
    {
        currentQuantity = (int)value;
        UpdateUI();
    }

    private void IncreaseQuantity()
    {
        if (currentQuantity < maxQuantity)
        {
            currentQuantity++;
            UpdateUI();
        }
    }

    private void DecreaseQuantity()
    {
        if (currentQuantity > 1)
        {
            currentQuantity--;
            UpdateUI();
        }
    }

    private void Confirm()
    {
        int totalPrice = (isBuying ? currentItem.buyValue : currentItem.sellValue) * currentQuantity;
        string message = isBuying
            ? StringConstants.FormatBuyConfirmation(currentQuantity, currentItem.itemName, totalPrice)
            : StringConstants.FormatSellConfirmation(currentQuantity, currentItem.itemName, totalPrice);

        confirmationPopup.Show(message, () =>
        {
            onConfirm?.Invoke(currentItem, currentQuantity);
            Hide();
        });
    }

    public void Hide()
    {
        panel.SetActive(false);
        onConfirm = null;
        currentItem = null;
    }
}