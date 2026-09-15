using BaskgayBall.Input;
using UnityEngine;

public class PlayerScoreDisplay : MonoBehaviour, ISided
{
    public EPlayerSide Side => playerSide;
    [SerializeField] private EPlayerSide playerSide = EPlayerSide.Player1;

    private void OnEnable()
    {
        GlobalEvents.OnGameScoreChange += OnScoreChange;
    }

    private void OnDisable()
    {
        GlobalEvents.OnGameScoreChange -= OnScoreChange;
    }

    private void OnScoreChange(int player1Score, int player2Score, EPlayerSide scoringPlayer)
    {
        if (scoringPlayer == playerSide)
        {
            print($"{scoringPlayer} scored. Player1 score {player1Score}, Player2 score: {player2Score}");
        }
    }
}
