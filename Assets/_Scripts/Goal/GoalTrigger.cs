using BaskgayBall.Ball;
using BaskgayBall.Goal;
using UnityEngine;
using UnityEngine.UIElements;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private Goal goalOwner = null;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Ball>(out var ball))
        {
            if (ball.TryGetComponent<Rigidbody2D>(out var rb))
            {
                if (rb.linearVelocityY < 0)
                {
                    if (goalOwner.Side != ball.Side)
                    {
                        RoundManager.Instance.EndRound(ERoundEndReason.Goal, ball.Side);
                    }
                }
            }
        }
    }
}
