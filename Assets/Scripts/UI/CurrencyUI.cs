using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;

    public void SetCoinsText(int amount) => coinsText.text = amount.ToString();
}