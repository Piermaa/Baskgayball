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
    public class Ball : MonoBehaviour
    {
        public BallState State { get; private set; } = BallState.Free;

        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void OnGrabbed(Transform holder)
        {
            State = BallState.Held;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            transform.SetParent(holder);
            transform.localPosition = Vector3.zero;
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
    }
}
