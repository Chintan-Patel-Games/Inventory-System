using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotContainer;
    public GameObject popupPanel;

    public void RefreshUI(List<InventorySlot> inventorySlots)
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (var slot in inventorySlots)
        {
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            Image icon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            TMP_Text countText = obj.transform.Find("ItemCount").GetComponent<TMP_Text>();

            if (slot.item != null)
            {
                icon.sprite = slot.item.icon;
                icon.gameObject.SetActive(true);

                if (slot.count > 1)
                {
                    countText.text = slot.count.ToString();
                    countText.gameObject.SetActive(true);
                }
            }
        }
    }
}