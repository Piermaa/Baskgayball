using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BaskgayBall.Input;

public class ShopItemUI : MonoBehaviour
{
    [Header("Skin Info")]
    [SerializeField] private Image skinPreviewImage;
    [SerializeField] private TextMeshProUGUI skinNameText;

    [Header("Buy Controls")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("Equip Controls")]
    [SerializeField] private GameObject equipContainer;
    [SerializeField] private Button equipP1Button;
    [SerializeField] private TextMeshProUGUI equipP1Text;
    [SerializeField] private Button equipP2Button;
    [SerializeField] private TextMeshProUGUI equipP2Text;

    private int skinIndex;
    private SkinData skinData;
    private System.Action onActionCompleted;

    private CosmeticsManager CosmeticsMng => GameManager.Instance?.CosmeticsMng;
    private CoinsManager CoinsMng => GameManager.Instance?.CoinsMng;

    private void Awake()
    {
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyClicked);
        if (equipP1Button != null) equipP1Button.onClick.AddListener(OnEquipP1Clicked);
        if (equipP2Button != null) equipP2Button.onClick.AddListener(OnEquipP2Clicked);
    }

    public void Setup(int index, SkinData data, System.Action onActionCallback)
    {
        skinIndex = index;
        skinData = data;
        onActionCompleted = onActionCallback;

        // Visuales fijos de la skin
        if (skinNameText != null)
        {
            skinNameText.text = string.IsNullOrEmpty(data.displayName) ? $"Skin #{index + 1}" : data.displayName;
        }

        if (skinPreviewImage != null)
        {
            Sprite spriteToUse = data.fillSprite != null ? data.fillSprite : data.headSprite;
            skinPreviewImage.sprite = spriteToUse;
            skinPreviewImage.enabled = spriteToUse != null;
        }

        RefreshState();
    }

    public void RefreshState()
    {
        if (CosmeticsMng == null || skinData == null) return;

        bool isUnlocked = CosmeticsMng.IsSkinUnlocked(skinIndex);

        if (!isUnlocked)
        {
            // Bloqueada: mostrar botón de comprar con precio
            if (buyButton != null)
            {
                buyButton.gameObject.SetActive(true);
                int currentCoins = CoinsMng != null ? CoinsMng.Coins : 0;
                buyButton.interactable = currentCoins >= skinData.cost;
            }

            if (priceText != null)
            {
                priceText.text = $"${skinData.cost}";
            }

            if (equipContainer != null) equipContainer.SetActive(false);
            if (equipP1Button != null) equipP1Button.gameObject.SetActive(false);
            if (equipP2Button != null) equipP2Button.gameObject.SetActive(false);
        }
        else
        {
            // Desbloqueada: ocultar compra y mostrar opciones para equipar
            if (buyButton != null) buyButton.gameObject.SetActive(false);
            if (equipContainer != null) equipContainer.SetActive(true);

            int p1Selected = CosmeticsMng.GetPlayerSelectedSkinIndex(EPlayerSide.Player1);
            int p2Selected = CosmeticsMng.GetPlayerSelectedSkinIndex(EPlayerSide.Player2);

            if (equipP1Button != null)
            {
                equipP1Button.gameObject.SetActive(true);
                bool isP1 = (p1Selected == skinIndex);
                equipP1Button.interactable = !isP1;
                if (equipP1Text != null) equipP1Text.text = isP1 ? "Equipped" : "Equip P1";
            }

            if (equipP2Button != null)
            {
                equipP2Button.gameObject.SetActive(true);
                bool isP2 = (p2Selected == skinIndex);
                equipP2Button.interactable = !isP2;
                if (equipP2Text != null) equipP2Text.text = isP2 ? "Equipped" : "Equip P2";
            }
        }
    }

    private void OnBuyClicked()
    {
        if (CosmeticsMng == null) return;
        if (CosmeticsMng.TryBuySkin(skinIndex))
        {
            onActionCompleted?.Invoke();
        }
    }

    private void OnEquipP1Clicked()
    {
        CosmeticsMng?.SetPlayerSelectedSkin(EPlayerSide.Player1, skinIndex);
        onActionCompleted?.Invoke();
    }

    private void OnEquipP2Clicked()
    {
        CosmeticsMng?.SetPlayerSelectedSkin(EPlayerSide.Player2, skinIndex);
        onActionCompleted?.Invoke();
    }
}

