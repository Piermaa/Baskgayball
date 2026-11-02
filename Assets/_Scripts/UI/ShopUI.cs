using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button openShopButton;
    [SerializeField] private Button closeShopButton;

    [Header("Grid Setup")]
    [Tooltip("El objeto contenedor que tiene el GridLayoutGroup donde apareceran las tarjetas.")]
    [SerializeField] private Transform itemsContainer;

    [Tooltip("El prefab del cuadradito/tarjeta que tiene el script ShopItemUI.")]
    [SerializeField] private ShopItemUI itemPrefab;

    [Header("Coins Display (Opcional)")]
    [SerializeField] private TextMeshProUGUI coinsText;

    private readonly List<ShopItemUI> spawnedItems = new List<ShopItemUI>();
    private bool isInitialized = false;

    private CosmeticsManager CosmeticsMng => GameManager.Instance?.CosmeticsMng;
    private CoinsManager CoinsMng => GameManager.Instance?.CoinsMng;

    private void Awake()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        if (openShopButton != null) openShopButton.onClick.AddListener(OpenShop);
        if (closeShopButton != null) closeShopButton.onClick.AddListener(CloseShop);
    }

    private void OnEnable()
    {
        if (CoinsMng != null) CoinsMng.OnCoinsChanged += HandleCoinsChanged;
        if (CosmeticsMng != null) CosmeticsMng.OnCosmeticsChanged += RefreshAllItems;
    }

    private void OnDisable()
    {
        if (CoinsMng != null) CoinsMng.OnCoinsChanged -= HandleCoinsChanged;
        if (CosmeticsMng != null) CosmeticsMng.OnCosmeticsChanged -= RefreshAllItems;
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        InitializeGridIfNeeded();
        RefreshAllItems();
        UpdateCoinsDisplay();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    private void InitializeGridIfNeeded()
    {
        if (isInitialized) return;
        if (CosmeticsMng == null || CosmeticsMng.skinDataBase == null || itemsContainer == null || itemPrefab == null)
            return;

        // Limpiamos cualquier hijo que haya quedado de prueba en el contenedor
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
        spawnedItems.Clear();

        SkinData[] skins = CosmeticsMng.skinDataBase.skins;
        for (int i = 0; i < skins.Length; i++)
        {
            SkinData skin = skins[i];
            if (skin == null) continue;

            ShopItemUI item = Instantiate(itemPrefab, itemsContainer);
            item.Setup(i, skin, OnItemActionCompleted);
            spawnedItems.Add(item);
        }

        isInitialized = true;
    }

    private void OnItemActionCompleted()
    {
        RefreshAllItems();
        UpdateCoinsDisplay();
    }

    public void RefreshAllItems()
    {
        foreach (ShopItemUI item in spawnedItems)
        {
            if (item != null)
            {
                item.RefreshState();
            }
        }
    }

    private void HandleCoinsChanged(int newCoins)
    {
        UpdateCoinsDisplay();
        RefreshAllItems();
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsText != null && CoinsMng != null)
        {
            coinsText.text = CoinsMng.Coins.ToString();
        }
    }
}
