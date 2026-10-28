using BaskgayBall.Input;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameModeManager gameModeMng = new();
    [SerializeField] private CosmeticsManager cosmeticsMng = new();
    private CoinsManager coinsMng = new();

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

    private void OnInit()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameModeMng = new GameModeManager();
        coinsMng = new CoinsManager();
        cosmeticsMng = new CosmeticsManager();
    }
}


public class GameModeManager
{
    public bool IsChaosMode { get { return isChaosMode; } }
    private bool isChaosMode;

    public void SetMode(bool newChaosMode)
    {
        isChaosMode = newChaosMode;
    }
}
[System.Serializable]
public class CosmeticsManager
{
    public SkinDataBase skinDataBase;
    private int skinSelectedPlayer1;
    private int skinSelectedPlayer2;

    public int GetPlayerSelectedSkin(EPlayerSide player)
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
    private int coins = 0;
    public int Coins => coins;
    public void RemoveCoins(int amount)
    {
        coins -= amount;
        coins = Mathf.Max(coins, 0);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }
}
