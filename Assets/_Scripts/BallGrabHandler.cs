using BaskgayBall.Ball;
using UnityEngine;

namespace BaskgayBall.Player
{

    [RequireComponent(typeof(Collider2D))]
    public class BallGrabHandler : MonoBehaviour
    {
        private Ball.Ball _ballInRange;
        private Ball.Ball _heldBall;

        public bool IsHoldingBall => _heldBall != null;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Ball.Ball ball)) return;

            _ballInRange = ball;

         
            if (ball.State == BallState.Free)
            {
                TryGrab();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Ball.Ball ball) && ball == _ballInRange)
            {
                _ballInRange = null;
            }
        }

        public bool TryGrab()
        {
            if (_heldBall != null) return false;
            if (_ballInRange == null || _ballInRange.State != BallState.Free) return false;

            _heldBall = _ballInRange;
            _heldBall.OnGrabbed(transform);
            return true;
        }
        public bool Throw(Vector2 velocity)
        {
            if (_heldBall == null) return false;

            _heldBall.OnThrown(velocity);
            _heldBall = null;
            return true;
        }
    }
}