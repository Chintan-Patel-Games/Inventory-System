using System.Collections;
using TMPro;
using UnityEngine;

public class ToasterUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float displayDuration = 3f;

    private Coroutine currentCoroutine;

    private void Awake() => panel.SetActive(false);

    public void ShowMessage(string message)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        messageText.text = message;
        panel.SetActive(true);
        currentCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        panel.SetActive(false);
        currentCoroutine = null;
    }
}