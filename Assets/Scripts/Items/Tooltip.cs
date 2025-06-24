using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        panel.SetActive(false);
    }

    private void Update()
    {
        if (panel.activeSelf)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                Input.mousePosition,
                null,
                out localMousePos
            );

            // Top-left alignment (no offset)
            RectTransform tooltipRect = panel.GetComponent<RectTransform>();
            Vector2 size = tooltipRect.sizeDelta;

            // Move the panel so its top-left corner is at the cursor
            Vector2 anchoredPos = localMousePos + new Vector2(size.x * 0.5f, -size.y * 0.5f);
            tooltipRect.anchoredPosition = anchoredPos;
        }
    }

    public void Show(ItemData item, Sprite raritySprite)
    {
        itemIcon.sprite = item.icon;
        rarityBG.sprite = raritySprite;
        rarityBG.color = raritySprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;

        nameText.text = $"Item : {item.itemName}";
        typeText.text = $"Type : {item.type}";
        rarityText.text = $"Rarity : {item.rarity}";
        descriptionText.text = $"Description : {item.description}";
        buyValueText.text = $"Buy Value : {item.buyValue}";
        sellValueText.text = $"Sell Value : {item.sellValue}";
        weightText.text = $"Weight : {item.weight}";
        maxStackText.text = $"Max Stack : {item.maxStack}";

        panel.SetActive(true);
    }

    public void Hide() => panel.SetActive(false);
}
