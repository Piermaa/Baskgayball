using BaskgayBall.Input;
using BaskgayBall.Persistence;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameModeManager gameModeMng = new();
    [SerializeField] private CosmeticsManager cosmeticsMng = new();
    private CoinsManager coinsMng = new();
    private ISaveService saveService;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public GameModeManager GameModeMng => gameModeMng;
    public CoinsManager CoinsMng => coinsMng;
    public CosmeticsManager CosmeticsMng => cosmeticsMng;

    public bool StartedFromMenu { get; private set; } = true;
    public bool IsDirectPlayInEditor { get; private set; } = false;

    private string lastLoadedSceneName = string.Empty;
    
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
        if (saveService == null) return;

        saveService.Save(new SaveData
        {
            coins = coinsMng.Coins,
            skinSelectedPlayer1 = cosmeticsMng.GetPlayerSelectedSkinIndex(EPlayerSide.Player1),
            skinSelectedPlayer2 = cosmeticsMng.GetPlayerSelectedSkinIndex(EPlayerSide.Player2),
            unlockedSkins = new List<int>(cosmeticsMng.UnlockedSkins)
        });
    }

    public void SetStartedFromMenu(bool value)
    {
        StartedFromMenu = value;
        IsDirectPlayInEditor = false;
    }

    private void OnInit()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Scene activeScene = SceneManager.GetActiveScene();
        lastLoadedSceneName = activeScene.name;
        if (activeScene.name == mainMenuSceneName)
        {
            StartedFromMenu = true;
            IsDirectPlayInEditor = false;
        }
        else
        {
            IsDirectPlayInEditor = true;
            StartedFromMenu = true;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        saveService = new JsonFileSaveService();

        SaveData data = saveService.Load();
        coinsMng.SetCoins(data.coins);
        cosmeticsMng.InitUnlockedSkins(data.unlockedSkins);
        cosmeticsMng.SetPlayerSelectedSkin(EPlayerSide.Player1, data.skinSelectedPlayer1);
        cosmeticsMng.SetPlayerSelectedSkin(EPlayerSide.Player2, data.skinSelectedPlayer2);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsDirectPlayInEditor && scene.name == lastLoadedSceneName)
        {
            return;
        }

        if (scene.name == mainMenuSceneName)
        {
            StartedFromMenu = true;
            IsDirectPlayInEditor = false;
        }
        else if (lastLoadedSceneName == mainMenuSceneName)
        {
            StartedFromMenu = true;
            IsDirectPlayInEditor = false;
        }
        else
        {
            StartedFromMenu = false;
            IsDirectPlayInEditor = false;
        }

        lastLoadedSceneName = scene.name;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}

[System.Serializable]
public class GameModeManager
{
    public bool IsChaosMode { get { return isChaosMode; } }
    [SerializeField] private bool isChaosMode;

    public void SetMode(bool newChaosMode)
    {
        isChaosMode = newChaosMode;
    }
}

[System.Serializable]
public class CosmeticsManager
{
    public event Action OnCosmeticsChanged;

    public SkinDataBase skinDataBase;
    [SerializeField] private int skinSelectedPlayer1 = 0;
    [SerializeField] private int skinSelectedPlayer2 = 0;
    private HashSet<int> unlockedSkins = new() { 0 };

    public IReadOnlyCollection<int> UnlockedSkins => unlockedSkins;

    public void InitUnlockedSkins(List<int> loadedUnlocked)
    {
        unlockedSkins = new HashSet<int> { 0 };
        if (loadedUnlocked != null)
        {
            foreach (int index in loadedUnlocked)
            {
                unlockedSkins.Add(index);
            }
        }
    }

    public bool IsSkinUnlocked(int skinIndex)
    {
        if (skinIndex == 0) return true;
        return unlockedSkins.Contains(skinIndex);
    }

    public bool TryBuySkin(int skinIndex)
    {
        if (skinDataBase == null || skinDataBase.skins == null) return false;
        if (skinIndex < 0 || skinIndex >= skinDataBase.skins.Length) return false;
        if (IsSkinUnlocked(skinIndex)) return true;

        SkinData skin = skinDataBase.skins[skinIndex];
        if (skin == null) return false;

        if (GameManager.Instance.CoinsMng.Coins >= skin.cost)
        {
            GameManager.Instance.CoinsMng.RemoveCoins(skin.cost);
            unlockedSkins.Add(skinIndex);
            GameManager.Instance.SaveGame();
            OnCosmeticsChanged?.Invoke();
            return true;
        }

        return false;
    }

    public SkinData GetPlayerSkinData(EPlayerSide playerSide)
    {
        int index = GetPlayerSelectedSkinIndex(playerSide);
        if (skinDataBase != null && skinDataBase.skins != null && index >= 0 && index < skinDataBase.skins.Length)
        {
            return skinDataBase.skins[index];
        }
        return null;
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
        if (!IsSkinUnlocked(selected)) return;

        switch (player)
        {
            case EPlayerSide.Player1:
                skinSelectedPlayer1 = selected;
                break;
            case EPlayerSide.Player2:
                skinSelectedPlayer2 = selected;
                break;
        }

        GameManager.Instance?.SaveGame();
        OnCosmeticsChanged?.Invoke();
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
