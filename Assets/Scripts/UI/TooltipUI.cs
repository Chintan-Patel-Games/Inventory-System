using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image rarityBG;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text weightText;

    private RectTransform tooltipRect;
    private RectTransform canvasRect;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        tooltipRect = panel.GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();

        panel.SetActive(false);
    }

    private void Update()
    {
        if (!panel.activeSelf || canvasRect == null)
            return;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out localMousePos);

        // Position top-left corner at mouse pointer
        Vector2 size = tooltipRect.sizeDelta;
        tooltipRect.anchoredPosition = localMousePos + new Vector2(size.x * 0.5f, -size.y * 0.5f);
    }

    public void Show(ItemSlot itemSlot)
    {
        if (itemSlot.item == null) return;

        itemIcon.sprite = itemSlot.item.icon;
        rarityBG.sprite = itemSlot.item.rarityBg;
        rarityBG.color = itemSlot.item.rarity == Rarity.VeryCommon ? new Color(1f, 1f, 1f, 0f) : Color.white;

        countText.text = GetItemCountText(itemSlot);
        nameText.text = StringConstants.ITEM_LABEL + itemSlot.item.itemName;
        typeText.text = StringConstants.TYPE_LABEL + itemSlot.item.type;
        rarityText.text = StringConstants.RARITY_LABEL + itemSlot.item.rarity;
        descriptionText.text = StringConstants.DESCRIPTION_LABEL + itemSlot.item.description;
        valueText.text = GetItemValueText(itemSlot);
        weightText.text = StringConstants.WEIGHT_LABEL + itemSlot.item.weight;

        panel.SetActive(true);
    }

    public void Hide() => panel.SetActive(false);

    private string GetItemCountText(ItemSlot itemSlot)
    {
        if (itemSlot.owner == SlotOwner.Inventory)
            return StringConstants.FormatItemCount(itemSlot.count, itemSlot.item.maxStack);
        else
            return string.Empty;
    }

    private string GetItemValueText(ItemSlot itemSlot)
    {
        if (itemSlot.owner == SlotOwner.Shop)
            return StringConstants.BUY_VALUE_LABEL + itemSlot.item.buyValue;
        else if (itemSlot.owner == SlotOwner.Inventory)
            return StringConstants.SELL_VALUE_LABEL + itemSlot.item.sellValue;
        else
            return string.Empty; // fallback
    }
}