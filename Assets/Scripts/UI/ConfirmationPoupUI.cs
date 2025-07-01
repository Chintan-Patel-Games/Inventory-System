using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopupUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Action onYes;
    private Action onNo;

    private void Awake()
    {
        yesButton.onClick.AddListener(HandleYes);
        noButton.onClick.AddListener(HandleNo);
        panel.SetActive(false);
    }

    public void Show(string message, Action onYesCallback, Action onNoCallback = null)
    {
        onYes = onYesCallback;
        onNo = onNoCallback;

        messageText.text = message;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
        onYes = null;
        onNo = null;
    }

    private void HandleYes()
    {
        onYes?.Invoke();
        Hide();
    }

    private void HandleNo()
    {
        onNo?.Invoke();
        Hide();
    }
}