using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image rarityBG;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text buyValueText;
    [SerializeField] private TMP_Text sellValueText;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private TMP_Text maxStackText;

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

    public void Show(ItemData item, Sprite raritySprite)
    {
        if (item == null) return;

        itemIcon.sprite = item.icon;
        rarityBG.sprite = raritySprite;
        rarityBG.color = raritySprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;

        nameText.text = UIConstants.ITEM_LABEL + item.itemName;
        typeText.text = UIConstants.TYPE_LABEL + item.type;
        rarityText.text = UIConstants.RARITY_LABEL + item.rarity;
        descriptionText.text = UIConstants.DESCRIPTION_LABEL + item.description;
        buyValueText.text = UIConstants.BUY_VALUE_LABEL + item.buyValue;
        sellValueText.text = UIConstants.SELL_VALUE_LABEL + item.sellValue;
        weightText.text = UIConstants.WEIGHT_LABEL + item.weight;
        maxStackText.text = UIConstants.MAX_STACK_LABEL + item.maxStack;

        panel.SetActive(true);
    }

    public void Hide() => panel.SetActive(false);
}
