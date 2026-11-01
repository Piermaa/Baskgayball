using BaskgayBall.Input;
using BaskgayBall.Persistence;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameModeManager gameModeMng = new();
    [SerializeField] private CosmeticsManager cosmeticsMng = new();
    private CoinsManager coinsMng = new();
    private ISaveService saveService;

    public GameModeManager GameModeMng => gameModeMng;
    public CoinsManager CoinsMng => coinsMng;
    public CosmeticsManager CosmeticsMng => cosmeticsMng;
    
    private void Awake()
    {
        if (Instance == null)
        {
            OnInit();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        saveService.Save(new SaveData
        {
            coins = coinsMng.Coins,
            skinSelectedPlayer1 = cosmeticsMng.GetPlayerSelectedSkinIndex(EPlayerSide.Player1),
            skinSelectedPlayer2 = cosmeticsMng.GetPlayerSelectedSkinIndex(EPlayerSide.Player2)
        });
    }

    private void OnInit()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveService = new JsonFileSaveService();

        SaveData data = saveService.Load();
        coinsMng.SetCoins(data.coins);
        cosmeticsMng.SetPlayerSelectedSkin(EPlayerSide.Player1, data.skinSelectedPlayer1);
        cosmeticsMng.SetPlayerSelectedSkin(EPlayerSide.Player2, data.skinSelectedPlayer2);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveGame();
    }
}

[System.Serializable]
public class GameModeManager
{
    public bool IsChaosMode { get { return isChaosMode; } }
    [SerializeField]private bool isChaosMode;

    public void SetMode(bool newChaosMode)
    {
        isChaosMode = newChaosMode;
    }
}
[System.Serializable]
public class CosmeticsManager
{
    public SkinDataBase skinDataBase;
    [SerializeField] private int skinSelectedPlayer1 = 0;
    [SerializeField] private int skinSelectedPlayer2 = 0;

    public SkinData GetPlayerSkinData(EPlayerSide playerSide)
    {
        return skinDataBase.skins[GetPlayerSelectedSkinIndex(playerSide)];
    }

    public int GetPlayerSelectedSkinIndex(EPlayerSide player)
    {
        switch (player)
        {
            case EPlayerSide.Player1:
                return skinSelectedPlayer1;
            case EPlayerSide.Player2:
                return skinSelectedPlayer2;
        }

        return 0;
    }

    public void SetPlayerSelectedSkin(EPlayerSide player, int selected)
    {
        switch (player)
        {
            case EPlayerSide.Player1:
                skinSelectedPlayer1 = selected;
                break;
            case EPlayerSide.Player2:
                skinSelectedPlayer2 = selected;
                break;
        }
    }
}

public class CoinsManager
{
    public event Action<int> OnCoinsChanged;
    private int coins = 0;
    public int Coins => coins;
    public void RemoveCoins(int amount)
    {
        var temp = coins - amount;
        temp = Mathf.Max(temp, 0);
        SetCoins(temp);
    }


    public void AddCoins(int amount)
    {
        SetCoins(coins + amount);
    }
    public void SetCoins(int amount)
    {
        coins = amount;
        OnCoinsChanged?.Invoke(coins);
    }
}
