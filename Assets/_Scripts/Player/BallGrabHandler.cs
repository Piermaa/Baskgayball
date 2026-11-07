using BaskgayBall.Ball;
using BaskgayBall.Core;
using BaskgayBall.Input;
using UnityEngine;

namespace BaskgayBall.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class BallGrabHandler : MonoBehaviour, IPrefabSubLogic, IResettable
    {
        Transform IPrefabSubLogic.PrefabRoot => prefabRoot;
        [SerializeField] private Transform prefabRoot = null;

        private ISided _sided;
        private Ball.Ball _ballInRange;
        private Ball.Ball _heldBall;

        public bool IsHoldingBall => _heldBall != null;

        private void Awake()
        {
            _sided = prefabRoot.GetComponent<ISided>();
        }

        private void OnEnable()
        {
            GlobalEvents.OnFinishRound += ResetState;
        }

        private void OnDisable()
        {
            GlobalEvents.OnFinishRound -= ResetState;
        }

        private void FixedUpdate()
        {
            if (_heldBall == null && _ballInRange != null)
            {
                TryGrab();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Ball.Ball ball)) return;

            _ballInRange = ball;
            TryGrab();
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
            if (_ballInRange == null) return false;

            // Delegates the "who is allowed to catch this ball right now" decision to the ball
            // itself, so the same-team grab-immunity window (see Ball.CanBeGrabbedBy) is honored
            // both here and from any other place that ends up calling TryGrab.
            if (!_ballInRange.CanBeGrabbedBy(_sided.Side)) return false;

            _heldBall = _ballInRange;
            _heldBall.OnGrabbed(transform, _sided.Side);
            return true;
        }

        public bool Throw(Vector2 velocity)
        {
            if (_heldBall == null) return false;

            _heldBall.OnThrown(velocity);
            _heldBall = null;
            return true;
        }

        public void ResetState()
        {
            _heldBall = null;
            _ballInRange = null;
        }
    }
}
