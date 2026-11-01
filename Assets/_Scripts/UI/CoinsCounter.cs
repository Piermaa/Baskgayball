using TMPro;
using UnityEngine;

public class CoinsCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;

    private void Start()
    {
        GameManager.Instance.CoinsMng.OnCoinsChanged += CoinsMng_OnCoinsChanged;
        CoinsMng_OnCoinsChanged(GameManager.Instance.CoinsMng.Coins);
    }

    private void OnDestroy()
    {
        GameManager.Instance.CoinsMng.OnCoinsChanged -= CoinsMng_OnCoinsChanged;
    }

    private void CoinsMng_OnCoinsChanged(int obj)
    {
        coinsText.text = obj.ToString();
    }
}
