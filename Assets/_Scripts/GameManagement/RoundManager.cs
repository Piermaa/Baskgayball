using BaskgayBall.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ERoundEndReason
{
    None,
    Goal,
    OOB,
    GameFinish
}

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    public GameObject Player1Hoop => player1Hoop;
    public GameObject Player2Hoop => player2Hoop;

    [Header("Ball")]
    [SerializeField] private GameObject[] ballPrefabs;
    [SerializeField] private Transform ballSpawnPosition;

    [Header("Hoops")]
    [SerializeField] private GameObject player1Hoop;
    [SerializeField] private GameObject player2Hoop;

    [Header("Boundaries")]
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;

    [Header("Score")]
    [SerializeField] private int scoreToWin = 5;
    public int player1Score = 0;
    public int player2Score = 0;

    public bool AddScore(EPlayerSide playerSide)
    {
        //Handheld.Vibrate();
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

        return player1Won || player2Won;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        Instantiate(BasketHelpers.GetRandomElement(ballPrefabs), ballSpawnPosition);
    }

    public bool IsBallOutOfBounds(Transform ballTrans)
    {
        float ballX = ballTrans.position.x;
        float leftBoundaryX = leftBoundary.position.x;
        float rightBoundaryX = rightBoundary.position.x;

        return ballX < leftBoundaryX || ballX > rightBoundaryX;
    }

    public void EndRound(ERoundEndReason endReason, EPlayerSide playerSide)
    {
        switch (endReason)
        {
            case ERoundEndReason.None:
                break;
            case ERoundEndReason.Goal:
                AddScore(playerSide); 
                break;
            case ERoundEndReason.OOB:
                break;
        }
    }

    public void StartMatch()
    {
        player1Score = 0;
        player2Score = 0;

    }
  
}
