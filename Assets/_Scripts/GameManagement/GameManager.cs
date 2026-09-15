using BaskgayBall.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string gameScene;
    [SerializeField] private string menuScene;
    [SerializeField] private int scoreToWin = 5;
    public static GameManager Instance;
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
        print("Game finished");
    }


    public void AddScore(EPlayerSide playerSide)
    {
        Handheld.Vibrate();
        switch (playerSide)
        {
            case EPlayerSide.Player1:
                player1Score++;
                break;
            case EPlayerSide.Player2:
                player2Score++;
                break;
            default:
                break;
        }

        GlobalEvents.DispatchScoreChange(player1Score, player2Score, playerSide);

        bool player1Won = player1Score >= scoreToWin;
        bool player2Won = player2Score >= scoreToWin;

        if (player1Won || player2Won)
        {
            FinishGame();
        }
    }
}
