using BaskgayBall.Input;
using System;
using UnityEngine;


public static class GlobalEvents
{
    /// <summary>
    /// (int player1, int player2, EPlayerSide scoringPlayer) ||||||
    /// Dispatched by game manager to inform score changes.
    /// </summary>
    public static event Action<int, int, EPlayerSide> OnGameScoreChange;

    public static void DispatchScoreChange(int player1Score, int player2Score, EPlayerSide scoringPlayer)
    {
        OnGameScoreChange(player1Score, player2Score, scoringPlayer);
    }
}