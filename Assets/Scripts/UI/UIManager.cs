using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Raycast Blocker")]
    [SerializeField] private GameObject raycastBlocker;

    [Header("Popup References")]
    [SerializeField] private QuantityPopupUI quantityPopupUI;
    [SerializeField] private ConfirmationPopupUI confirmationPopupUI;
    [SerializeField] private PopupUI popupUI;
    [SerializeField] private ToasterUI toasterUI;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private Button closeGame;

    private int blockCount = 0;

    private void Awake()
    {
        quantityPopupUI.OnHide += HideQuantityPopup;
        quantityPopupUI.OnConfirmPopup += ShowConfirmationPopup;
        confirmationPopupUI.OnHide += HideConfirmationPopup;
        popupUI.OnHide += HidePopup;

        closeGame.onClick.AddListener(CloseGame);
    }

    private void BlockRaycasts()
    {
        blockCount++;
        raycastBlocker.SetActive(true);
    }

    private void UnblockRaycasts()
    {
        blockCount = Mathf.Max(0, blockCount - 1);
        if (blockCount == 0)
            raycastBlocker.SetActive(false);
    }

    public void ShowPopup(string message)
    {
        SoundManager.Instance.PlayErrorSound();
        popupUI.Show(message);
        BlockRaycasts();
    }

    public void HidePopup()
    {
        popupUI.OnHide -= HidePopup; // Prevent recursion
        SoundManager.Instance.PlayPopupCloseClick();
        popupUI.Hide();
        UnblockRaycasts();
        popupUI.OnHide += HidePopup;  // Resubscribe after hiding
    }

    public void ShowBuyQuantityPopup(ItemSlot itemSlot, int maxQty, int currentGoldCoins, int currentWeight, int maxWeight, Action<ItemData, int> confirmCallback)
    {
        SoundManager.Instance.PlayPopupOpenClick(); // Play click sound on popup open
        quantityPopupUI.ShowBuyPopup(itemSlot, maxQty, currentGoldCoins, currentWeight, maxWeight, confirmCallback);
        BlockRaycasts();
    }

    public void ShowSellQuantityPopup(ItemSlot itemSlot, int maxQty, Action<ItemData, int> confirmCallback)
    {
        SoundManager.Instance.PlayPopupOpenClick(); // Play click sound on popup open
        quantityPopupUI.ShowSellPopup(itemSlot, maxQty, confirmCallback);
        BlockRaycasts();
    }

    public void HideQuantityPopup()
    {
        quantityPopupUI.OnHide -= HideQuantityPopup; // Prevent recursion
        quantityPopupUI.Hide();
        UnblockRaycasts();
        quantityPopupUI.OnHide += HideQuantityPopup; // Resubscribe after hiding
    }

    public void ShowConfirmationPopup(string message, Action onYesCallback, Action onNoCallback = null)
    {
        SoundManager.Instance.PlayPopupOpenClick(); // Play click sound on popup open
        quantityPopupUI.OnConfirmPopup -= ShowConfirmationPopup; // Prevent recursion
        confirmationPopupUI.Show(message, onYesCallback, onNoCallback);
        BlockRaycasts();
        quantityPopupUI.OnConfirmPopup += ShowConfirmationPopup; // Resubscribe after hiding
    }

    public void HideConfirmationPopup()
    {
        confirmationPopupUI.OnHide -= HideConfirmationPopup; // Prevent recursion
        confirmationPopupUI.Hide();
        quantityPopupUI.Reactivate();
        UnblockRaycasts();
        confirmationPopupUI.OnHide += HideConfirmationPopup; // Resubscribe after hiding
    }

    public void ShowCurrency(int amount) => coinsText.text = amount.ToString();

    public void ShowToaster(string message) => toasterUI.ShowMessage(message);

    public void CloseGame()
    {
        ShowConfirmationPopup(
            StringConstants.CLOSE_GAME_CONFIRMATION_POPUP,
            () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            },
            null
        );
    }

}