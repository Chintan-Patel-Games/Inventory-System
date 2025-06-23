using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image rarityBG;       // bg_img for rarity
    [SerializeField] private Image icon;           // Item image
    [SerializeField] private TMP_Text countText;   // Quantity text

    private CanvasGroup canvasGroup;
    private GameObject dragIconObject;
    private InventorySlot slotData;
    private int index;

    private System.Func<Rarity, Sprite> getRaritySprite;
    private System.Action<int, int> onSwapRequest;

    public void Setup(InventorySlot slot, int index, System.Action<int, int> onSwapRequest, System.Func<Rarity, Sprite> getRaritySprite)
    {
        this.slotData = slot;
        this.index = index;
        this.onSwapRequest = onSwapRequest;
        this.getRaritySprite = getRaritySprite;
        canvasGroup = GetComponent<CanvasGroup>();
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (slotData.item != null)
        {
            icon.sprite = slotData.item.icon;
            icon.gameObject.SetActive(true);
            countText.text = slotData.count > 1 ? slotData.count.ToString() : "";

            if (slotData.item.rarity == Rarity.VeryCommon)
            {
                rarityBG.sprite = null;
                rarityBG.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                rarityBG.sprite = getRaritySprite?.Invoke(slotData.item.rarity);
                rarityBG.color = Color.white;
            }
        }
        else
        {
            icon.gameObject.SetActive(false);
            countText.text = "";
            rarityBG.color = new Color(1f, 1f, 1f, 0f); // faded
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //TooltipManager.Show(slotData.item);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData.IsEmpty) return;

        canvasGroup.blocksRaycasts = false;
        rarityBG.enabled = false;
        icon.enabled = false;
        countText.enabled = false;

        dragIconObject = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasGroup));
        dragIconObject.transform.SetParent(canvasGroup.transform.root, false);
        dragIconObject.transform.SetAsLastSibling();

        Image dragImage = dragIconObject.AddComponent<Image>();
        dragImage.sprite = slotData.item.icon;
        dragImage.raycastTarget = false;

        RectTransform dragRect = dragIconObject.GetComponent<RectTransform>();
        dragRect.sizeDelta = icon.rectTransform.sizeDelta;
        dragRect.position = Input.mousePosition;

        if (slotData.count > 1)
        {
            GameObject countGO = new GameObject("Count", typeof(RectTransform));
            countGO.transform.SetParent(dragIconObject.transform, false);

            var text = countGO.AddComponent<TextMeshProUGUI>();
            text.text = slotData.count.ToString();
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.BottomRight;
            text.color = Color.white;
            text.raycastTarget = false;

            var textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
            dragIconObject.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
            Destroy(dragIconObject);

        canvasGroup.blocksRaycasts = true;
        rarityBG.enabled = true;
        icon.enabled = true;
        countText.enabled = true;

        GameObject hovered = eventData.pointerEnter;
        InventorySlotUI target = hovered?.GetComponentInParent<InventorySlotUI>();

        if(target != null && target.index != index)
            onSwapRequest?.Invoke(index, target.index);
    }
}