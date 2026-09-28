using BaskgayBall.Core;
using BaskgayBall.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ERoundEndReason
{
    None,
    Goal,
    OOB,
    GameFinish
}

public class RoundManager : MonoBehaviour, IResettable
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
    [SerializeField] private float yMinBoundary = -15f;

    [Header("Score")]
    [SerializeField] private int scoreToWin = 5;
    [SerializeField] private float onScoreTimeScale = 0.1f;
    [SerializeField] private float onScoreResolveDelay = 2;
    public int player1Score = 0;
    public int player2Score = 0;
    

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

        StartMatch();
    }

    private void StartRound()
    {
        GlobalEvents.DispatchResetRound(true);
        Instantiate(BasketHelpers.GetRandomElement(ballPrefabs), ballSpawnPosition);
    }

    public bool AddScore(EPlayerSide playerSide)
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
        print($"Player 1 score: {player1Score}, Player 2 score: {player2Score}");
        return player1Won || player2Won;
    }

    public bool IsBallOutOfBounds(Transform ballTrans)
    {
        float ballX = ballTrans.position.x;
        float leftBoundaryX = leftBoundary.position.x;
        float rightBoundaryX = rightBoundary.position.x;
        bool isBelowYMin = ballTrans.position.y < yMinBoundary;

        return ballX < leftBoundaryX || ballX > rightBoundaryX || isBelowYMin;
    }

    public void EndRound(ERoundEndReason endReason, EPlayerSide playerSide)
    {
        switch (endReason)
        {
            case ERoundEndReason.None:
                break;
            case ERoundEndReason.Goal:
                bool isGameFinished = AddScore(playerSide);
                StartCoroutine(ResolveScoreChange(isGameFinished));
                break;
            case ERoundEndReason.OOB:
                ResetState();
                break;
        }
    }

    private IEnumerator ResolveScoreChange(bool gameFinished)
    {
        Time.timeScale = onScoreTimeScale;
        yield return new WaitForSecondsRealtime(onScoreResolveDelay);

        if (gameFinished)
        {
            FinishMatch();
        }
        {
            ResetState();
        }

        Time.timeScale = 1;
        yield break;
    }


    public void StartMatch()
    {
        player1Score = 0;
        player2Score = 0;
        GlobalEvents.DispatchScoreChange(0, 0, EPlayerSide.None);
        GlobalEvents.DispatchStartMatch();
        StartRound();
    }

    public void ResetState()
    {
        ResetRound();
    }
    private void ResetRound()
    {
        GlobalEvents.DispatchResetRound(false);
        StartRound();
    }

    private void FinishMatch()
    {
        GlobalEvents.DispatchFinishMatch();
    }
}
