using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image rarityBG;       // bg_img for rarity
    public Image icon;           // Item image
    public TMP_Text countText;   // Quantity text

    private Canvas canvas;
    private InventoryManager manager;
    private int index;

    private GameObject dragIconObject;

    public void Setup(InventoryManager manager, int index)
    {
        this.manager = manager;
        this.index = index;
        canvas = GetComponentInParent<Canvas>();
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        InventorySlot slot = manager.slots[index];

        if (slot.item != null)
        {
            icon.sprite = slot.item.icon;
            icon.gameObject.SetActive(true);
            countText.text = slot.count > 1 ? slot.count.ToString() : "";

            // If item is VeryCommon, make background transparent
            if (slot.item.rarity == Rarity.VeryCommon)
            {
                rarityBG.sprite = null;
                rarityBG.color = new Color(1f, 1f, 1f, 0f); // fully transparent
            }
            else
            {
                rarityBG.sprite = manager.GetRaritySprite(slot.item.rarity);
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


    public void OnPointerClick(PointerEventData eventData)
    {
        // Optional: Handle selection
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventorySlot slot = manager.slots[index];
        if (slot.IsEmpty) return;

        rarityBG.enabled = false;
        icon.enabled = false;
        countText.enabled = false;

        dragIconObject = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasGroup));
        dragIconObject.transform.SetParent(canvas.transform, false);
        dragIconObject.transform.SetAsLastSibling();

        Image dragImage = dragIconObject.AddComponent<Image>();
        dragImage.sprite = slot.item.icon;
        dragImage.raycastTarget = false;

        // Size and position
        RectTransform dragRect = dragIconObject.GetComponent<RectTransform>();
        dragRect.sizeDelta = icon.rectTransform.sizeDelta;
        dragRect.position = Input.mousePosition;

        // Optional: Add text
        if (slot.count > 1)
        {
            GameObject countGO = new GameObject("Count", typeof(RectTransform));
            countGO.transform.SetParent(dragIconObject.transform, false);

            var text = countGO.AddComponent<TextMeshProUGUI>();
            text.text = slot.count.ToString();
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.BottomRight;
            text.color = Color.white;

            var textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
        {
            dragIconObject.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
            Destroy(dragIconObject);

        rarityBG.enabled = true;
        icon.enabled = true;
        countText.enabled = true;

        GameObject hovered = eventData.pointerEnter;
        InventorySlotUI target = hovered?.GetComponentInParent<InventorySlotUI>();

        if (target != null && target.index != index)
        {
            manager.SwapSlots(index, target.index);
        }

        manager.RefreshAllSlots();
    }
}