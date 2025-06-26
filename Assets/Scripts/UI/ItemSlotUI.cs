using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    [SerializeField] private Image slotBackground; // Reference to Bg_Slot_img
    [SerializeField] private Image rarityBG;       // bg_img for rarity
    [SerializeField] private Image icon;           // Item image
    [SerializeField] private TMP_Text countText;   // Quantity text

    [Header("Rarity Backgrounds")]
    [SerializeField] private Sprite commonBG;
    [SerializeField] private Sprite rareBG;
    [SerializeField] private Sprite epicBG;
    [SerializeField] private Sprite legendaryBG;

    private CanvasGroup canvasGroup;
    private GameObject dragIconObject;
    private ItemSlot slotData;
    private int index;
    
    private Coroutine tooltipCoroutine;
    private const float tooltipDelay = 1f; // seconds
    private Action<int, int> onSwapRequest;

    public void Setup(ItemSlot slot, int index, Action<int, int> onSwapRequest)
    {
        slotData = slot;
        this.index = index;
        this.onSwapRequest = onSwapRequest;

        canvasGroup = GetComponent<CanvasGroup>();
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (slotData.item != null)
        {
            icon.sprite = slotData.item.icon;
            icon.gameObject.SetActive(true);
            countText.text = slotData.count > 1 ? slotData.count.ToString() : string.Empty;

            Sprite raritySprite = GetRaritySpriteForItem(slotData.item);
            rarityBG.sprite = raritySprite;
            rarityBG.color = raritySprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
        }
        else
        {
            icon.gameObject.SetActive(false);
            countText.text = string.Empty;
            rarityBG.color = new Color(1f, 1f, 1f, 0f); // faded
        }
    }
    public ItemSlot GetSlot() => slotData;

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

    public Sprite GetRaritySpriteForItem(ItemData item) => (item == null || item.rarity == Rarity.VeryCommon) ? null : GetRaritySprite(item.rarity);

    private IEnumerator ShowTooltipWithDelay()
    {
        yield return new WaitForSeconds(tooltipDelay);

        if (slotData.item != null && TooltipUI.Instance != null)
        {
            Sprite raritySprite = GetRaritySpriteForItem(slotData.item);
            TooltipUI.Instance.Show(slotData.item, raritySprite);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotData.item != null && TooltipUI.Instance != null)
            tooltipCoroutine = StartCoroutine(ShowTooltipWithDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
            tooltipCoroutine = null;
        }

        TooltipUI.Instance?.Hide();
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
            CreateDragCountText(dragIconObject.transform, slotData.count);
    }
    private void CreateDragCountText(Transform parent, int count)
    {
        GameObject countGO = new GameObject("Count", typeof(RectTransform));
        countGO.transform.SetParent(parent, false);

        var text = countGO.AddComponent<TextMeshProUGUI>();
        text.text = count.ToString();
        text.fontSize = 18;
        text.alignment = TextAlignmentOptions.BottomRight;
        text.color = Color.white;
        text.raycastTarget = false;

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
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
        ItemSlotUI target = hovered?.GetComponentInParent<ItemSlotUI>();

        if (target != null && target.index != index)
            onSwapRequest?.Invoke(index, target.index);
    }
}