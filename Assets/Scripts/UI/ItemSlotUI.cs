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

    // Inventory Delegates
    private Action<int, int> onSwapRequest;

    // Shop Delegates
    private Action<ItemSlot> onBuyRequest;
    private Action<ItemSlot> onSellRequest;

    public void InventorySetup(ItemSlot slot, int index, Action<int, int> onSwapRequest, Action<ItemSlot> onSellRequest)
    {
        slotData = slot;
        slotData.owner = SlotOwner.Inventory; // Set owner to Inventory
        this.index = index;
        this.onSwapRequest = onSwapRequest;
        this.onSellRequest = onSellRequest;

        canvasGroup = GetComponent<CanvasGroup>();
        UpdateSlot();
    }

    public void ShopSetup(ItemSlot slot, int index, Action<ItemSlot> onBuyRequest, Action<ItemSlot> onSellRequest)
    {
        slotData = slot;
        slotData.owner = SlotOwner.Shop; // Set owner to Shop
        this.index = index;
        this.onBuyRequest = onBuyRequest;
        this.onSellRequest = onSellRequest;

        canvasGroup = GetComponent<CanvasGroup>();
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (slotData.item != null)
        {
            icon.sprite = slotData.item.icon;
            icon.gameObject.SetActive(true);
            countText.text = slotData.count > 1 ? $"{slotData.count.ToString()}/{slotData.item.maxStack.ToString()}" : string.Empty;

            rarityBG.sprite = slotData.item.rarityBg;
            rarityBG.color = slotData.item.rarity == Rarity.VeryCommon ? new Color(1f, 1f, 1f, 0f) : Color.white;
        }
        else
        {
            icon.gameObject.SetActive(false);
            countText.text = string.Empty;
            rarityBG.color = new Color(1f, 1f, 1f, 0f); // faded
        }
    }

    public ItemSlot GetSlot() => slotData;

    private IEnumerator ShowTooltipWithDelay()
    {
        yield return new WaitForSeconds(tooltipDelay);

        if (slotData != null && TooltipUI.Instance != null)
            TooltipUI.Instance.Show(slotData);
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

        if (hovered == null)
        {
            UpdateSlot();
            return;
        }

        ItemSlotUI targetSlot = hovered.GetComponentInParent<ItemSlotUI>();

        if (targetSlot != null && targetSlot != this)
        {
            if (slotData.owner == SlotOwner.Inventory && targetSlot.slotData.owner == SlotOwner.Inventory)
            {
                if (targetSlot.slotData == null)
                {
                    onSwapRequest?.Invoke(index, targetSlot.index);
                    return;
                }
                else if (slotData.item == targetSlot.slotData.item && targetSlot.slotData.count < targetSlot.slotData.item.maxStack)
                {
                    int transferableAmount = Mathf.Min(
                        slotData.count,
                        targetSlot.slotData.item.maxStack - targetSlot.slotData.count
                    );

                    if (transferableAmount > 0)
                    {
                        targetSlot.slotData.count += transferableAmount;
                        slotData.count -= transferableAmount;

                        if (slotData.count <= 0)
                            slotData.Clear();

                        UpdateSlot();
                        targetSlot.UpdateSlot();
                        return;
                    }
                }
            }

            if (slotData.owner == SlotOwner.Shop && targetSlot.slotData.owner == SlotOwner.Inventory)
            {
                onBuyRequest?.Invoke(slotData);
                return;
            }

            if (slotData.owner == SlotOwner.Inventory && targetSlot.slotData.owner == SlotOwner.Shop)
            {
                onSellRequest?.Invoke(slotData);
                return;
            }
        }

        // Fallback
        UpdateSlot();
    }
}