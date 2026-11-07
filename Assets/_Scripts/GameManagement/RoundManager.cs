using BaskgayBall.Audio;
using BaskgayBall.Core;
using BaskgayBall.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using BaskgayBall.Stage;
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

    [Header("Coins")]
    [SerializeField] private int coinsToAddWhenMatchEnds = 50;
    public int player1Score = 0;
    public int player2Score = 0;

    [Header("Sound")]
    [SerializeField] private AudioSource goalSound;

    [Header("Stage")]
    [SerializeField] private SurfaceController surfaceController;
    public SurfaceProfile CurrentSurface => surfaceController != null ? surfaceController.Current : null;

    private bool _isRoundEnding;

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
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
            Instance = null;
        }
    }

    private void Start()
    {
        StartMatch();
    }

    private void StartRound()
    {
        _isRoundEnding = false;
        if (surfaceController != null) surfaceController.Roll();
        GlobalEvents.DispatchResetRound(true);
        Instantiate(BasketHelpers.GetRandomElement(ballPrefabs), ballSpawnPosition);
    }
    public bool AddScore(EPlayerSide playerSide, int scoreAmount = 1)
    {
        Handheld.Vibrate();
        switch (playerSide)
        {
            case EPlayerSide.Player1:
                player1Score += scoreAmount;
                break;
            case EPlayerSide.Player2:
                player2Score += scoreAmount;
                break;
            default:
                break;
        }

        if (goalSound != null)
        {
            goalSound.Play();
        }
        AudioManager.Instance?.PlayGoal();

        GlobalEvents.DispatchScoreChange(player1Score, player2Score, playerSide);

        bool player1Won = player1Score >= scoreToWin;
        bool player2Won = player2Score >= scoreToWin;
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

    public void EndRound(ERoundEndReason endReason, EPlayerSide playerSide, int amount = 1)
    {
        if (_isRoundEnding || endReason == ERoundEndReason.None)
        {
            return;
        }

        switch (endReason)
        {
            case ERoundEndReason.Goal:
                _isRoundEnding = true;
                bool isGameFinished = AddScore(playerSide, amount);
                StartCoroutine(ResolveScoreChange(isGameFinished));
                break;
            case ERoundEndReason.OOB:
                _isRoundEnding = true;
                ResetState();
                break;
        }
    }

    private IEnumerator ResolveScoreChange(bool gameFinished)
    {
        Time.timeScale = onScoreTimeScale;
        yield return new WaitForSecondsRealtime(onScoreResolveDelay);

        Time.timeScale = 1f;

        if (gameFinished)
        {
            FinishMatch();
        }
        else
        {
            ResetState();
        }
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
        GameManager.Instance.CoinsMng.AddCoins(coinsToAddWhenMatchEnds);
        GameManager.Instance.SaveGame();
        GlobalEvents.DispatchFinishMatch();
    }
}
