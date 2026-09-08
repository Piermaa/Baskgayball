using BaskgayBall.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMAnager : MonoBehaviour
{
    [SerializeField] private string gameScene;
    [SerializeField] private string menuScene;
    [SerializeField] private int scoreToWin = 5;
    public static GameMAnager Instance;
    public int player1Score = 0;
    public int player2Score = 0;


    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);    
        DontDestroyOnLoad(this);
    }

    public void StartMatch()
    {
        player1Score = 0;
        player2Score = 0;

        NextRound();
    }
    public void NextRound()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void FinishGame()
    {
        
    }


    public void AddScore(PlayerSide playerSide)
    {
        
    }
}
