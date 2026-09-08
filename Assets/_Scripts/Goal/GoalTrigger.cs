using BaskgayBall.Ball;
using BaskgayBall.Goal;
using UnityEngine;
using UnityEngine.UIElements;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private Goal goalOwner = null;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Ball>(out var ball))
        {
            if (ball.TryGetComponent<Rigidbody2D>(out var rb))
            {
                if (rb.linearVelocityY < 0)
                {
                    
                }
            }
        }
    }
}
