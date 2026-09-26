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

    /// <summary>
    /// (int player1, int player2, EPlayerSide scoringPlayer) ||||||
    /// Dispatched by game manager to inform score changes.
    /// </summary>
    public static event Action<ERoundEndReason, EPlayerSide> OnEndRound;

    public static event Action OnResetRound;


    public static void DispatchScoreChange(int player1Score, int player2Score, EPlayerSide scoringPlayer)
    {
        OnGameScoreChange?.Invoke(player1Score, player2Score, scoringPlayer);
    }

    public static void DispatchRoundEnd(ERoundEndReason roundEndReason, EPlayerSide playerSide)
    {
        OnEndRound?.Invoke(roundEndReason, playerSide);
    }

    public static void DispatchResetRound()
    {
        OnResetRound?.Invoke();
    }
}