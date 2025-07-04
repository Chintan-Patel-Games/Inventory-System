using System;
using TMPro;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;

    public Action OnHide;

    private void Awake() => panel.SetActive(false);

    public void Show(string message)
    {
        if (messageText == null) return;

        messageText.text = message;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
        OnHide?.Invoke(); // Notify UIManager
    }
}