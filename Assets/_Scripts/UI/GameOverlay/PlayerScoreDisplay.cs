using BaskgayBall.Input;
using TMPro;
using UnityEngine;

public class PlayerScoreDisplay : MonoBehaviour, ISided
{
    public EPlayerSide Side => playerSide;
    [SerializeField] private EPlayerSide playerSide = EPlayerSide.Player1;
    [SerializeField] private TextMeshProUGUI text = null;
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
            switch (scoringPlayer)
            {
                case EPlayerSide.Player1:
                    text.text = player1Score.ToString();
                    break;
                case EPlayerSide.Player2:
                    text.text = player2Score.ToString();
                    break;
            }
        }
    }
}
