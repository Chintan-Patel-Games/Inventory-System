using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        panel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);
    }

    public void Show(string message)
    {
        if (messageText == null) return;

        messageText.text = message;
        panel.SetActive(true);
    }

    public void ClosePopup() => panel.SetActive(false);
}