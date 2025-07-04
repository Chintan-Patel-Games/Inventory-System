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

    private CanvasGroup canvasGroup;

    private ItemSlot currentItemSlot;
    private int currentQty;
    private int affordableQty;
    private int maxAvailableQty;
    private bool isBuying = true;

    // Notifies the UIManager to show a confirmation popup
    public event Action<string, Action, Action> OnConfirmPopup;

    // Holds the actual transaction logic (buy/sell logic)
    private Action<ItemData, int> onTransactionConfirmed;

    public Action OnHide;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

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
        affordableQty = Mathf.FloorToInt((float)currentGoldCoins / itemSlot.item.buyValue);
        maxAvailableQty = maxQty; // max stack or limit

        // Calculate max quantity based on weight
        int weightLimitQty = Mathf.FloorToInt((maxWeight - currentWeight) / itemSlot.item.weight);

        int sliderMax = Mathf.Min(maxQty, affordableQty, weightLimitQty);

        if (sliderMax <= 0)
        {
            SoundManager.Instance.PlayErrorSound();
            popup.Show(StringConstants.NOT_ENOUGH_SPACE_OR_COINS);
            return;
        }

        SetupSlider(sliderMax, confirmCallback, true);
    }

    public void ShowSellPopup(ItemSlot itemSlot, int maxQty, Action<ItemData, int> confirmCallback)
    {
        currentItemSlot = itemSlot;
        int sliderMax = maxQty;
        maxAvailableQty = maxQty; // total quantity of that item
        SetupSlider(sliderMax, confirmCallback, false);
    }

    private void SetupSlider(int sliderMax, Action<ItemData, int> confirmCallback, bool isBuying)
    {
        currentQty = 1;
        this.isBuying = isBuying;
        onTransactionConfirmed = confirmCallback;

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
        SoundManager.Instance.PlayUIClick(); // Play click sound on increase
        // Enforce a hard limit of 100
        affordableQty = Mathf.Min(affordableQty, 100);

        int maxQty = isBuying ? affordableQty : maxAvailableQty;

        if (currentQty < maxQty)
        {
            currentQty++;
            quantitySlider.SetValueWithoutNotify(currentQty);
            UpdateUI();
        }
    }

    private void DecreaseQuantity()
    {
        SoundManager.Instance.PlayUIClick(); // Play click sound on decrease
        if (currentQty > quantitySlider.minValue)
        {
            currentQty--;
            quantitySlider.SetValueWithoutNotify(currentQty);
            UpdateUI();
        }
    }

    private void Confirm()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        int totalPrice = (isBuying ? currentItemSlot.item.buyValue : currentItemSlot.item.sellValue) * currentQty;
        string message = isBuying
            ? StringConstants.FormatBuyConfirmation(currentQty, currentItemSlot.item.itemName, totalPrice)
            : StringConstants.FormatSellConfirmation(currentQty, currentItemSlot.item.itemName, totalPrice);

        OnConfirmPopup?.Invoke(message, () =>
        {
            onTransactionConfirmed?.Invoke(currentItemSlot.item, currentQty);
            Hide();
        },  null);
    }

    public void Hide()
    {
        SoundManager.Instance.PlayPopupCloseClick(); // Play Popup close sound
        panel.SetActive(false);
        onTransactionConfirmed = null;
        currentItemSlot = null;
        OnHide?.Invoke(); // <- Notify UIManager
    }

    public void Reactivate()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}