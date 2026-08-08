using UnityEngine;
using BaskgayBall.Ball;

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
            if (other.TryGetComponent(out Ball.Ball ball))
            {
                _ballInRange = ball;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Ball.Ball ball) && ball == _ballInRange)
            {
                _ballInRange = null;
            }
        }

        /// <summary>
        /// Intenta agarrar la pelota que esté en rango y libre. No hace nada
        /// si ya se está sosteniendo una o si no hay ninguna disponible.
        /// </summary>
        public bool TryGrab()
        {
            if (_heldBall != null) return false;
            if (_ballInRange == null || _ballInRange.State != BallState.Free) return false;

            _heldBall = _ballInRange;
            _heldBall.OnGrabbed(transform);
            return true;
        }

        /// <summary>Tira la pelota sostenida con la velocidad indicada.</summary>
        public bool Throw(Vector2 velocity)
        {
            if (_heldBall == null) return false;

            _heldBall.OnThrown(velocity);
            _heldBall = null;
            return true;
        }
    }
}
