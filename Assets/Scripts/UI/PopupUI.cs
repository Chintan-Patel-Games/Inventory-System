using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public static PopupUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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