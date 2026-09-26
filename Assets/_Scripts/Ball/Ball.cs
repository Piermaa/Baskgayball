using BaskgayBall.Input;
using UnityEditor;
using UnityEngine;

namespace BaskgayBall.Ball
{
    public enum BallState
    {
        Free,
        Held,
        InFlight
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Ball : MonoBehaviour, ISided
    {
        public BallState State { get; private set; } = BallState.Free;
        public EPlayerSide Side => owningSide;
        [SerializeField] private TrailRenderer trailRenderer;
        private Rigidbody2D _rigidbody;
        private EPlayerSide owningSide;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void OnGrabbed(Transform holder, EPlayerSide p_ownerSide)
        {
            State = BallState.Held;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            transform.SetParent(holder);
            transform.localPosition = Vector3.zero;
            owningSide = p_ownerSide;
        }

        public void OnThrown(Vector2 velocity)
        {
            transform.SetParent(null);
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.linearVelocity = velocity;
            State = BallState.InFlight;
        }

        public void OnSettled()
        {
            State = BallState.Free;
        }

        private void FixedUpdate()
        {
            if (RoundManager.Instance.IsBallOutOfBounds(transform))
            {
                RoundManager.Instance.EndRound(ERoundEndReason.OOB, owningSide);
            }
        }

        private void OnDrawGizmos()
        {
            Handles.Label(transform.position+ new Vector3(0, .5f, 0), "State: " + State);
        }
    }
}
