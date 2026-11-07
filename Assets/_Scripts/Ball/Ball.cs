using BaskgayBall.Audio;
using BaskgayBall.Input;
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
        public int ScoreAmount => scoreValue;
        public BallState State { get; private set; } = BallState.Free;
        public EPlayerSide Side => owningSide;

        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private int scoreValue = 1;

        [Header("Grab Immunity")]
        [Tooltip("How long, in seconds, after a throw the ball refuses to be grabbed by the same side that threw it. Prevents a teammate standing nearby from instantly swallowing a fresh pass or shot before it has visibly left the thrower's hand. Does not affect the opposing side, which can intercept immediately.")]
        [SerializeField] private float sameSideGrabImmunityDuration = 0.2f;

        [Header("Bounce Audio Settings")]
        [Tooltip("Minimum collision speed required to trigger a bounce sound.")]
        [SerializeField] private float minBounceSpeed = 0.8f;
        [Tooltip("Collision speed at which the bounce sound plays at full volume.")]
        [SerializeField] private float maxBounceReferenceSpeed = 10f;

        private Rigidbody2D _rigidbody;
        private EPlayerSide owningSide;
        private float _thrownAtTime = float.NegativeInfinity;
        private bool _outOfBoundsTriggered;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            GlobalEvents.OnFinishRound += ResetState;
        }

        private void OnDisable()
        {
            GlobalEvents.OnFinishRound -= ResetState;
        }

        private void ResetState()
        {
            Destroy(gameObject);
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
            _thrownAtTime = Time.time;
            AudioManager.Instance?.PlayBallThrow();
        }

        public void OnSettled()
        {
            State = BallState.Free;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (State == BallState.Held) return;

            float impactSpeed = collision.relativeVelocity.magnitude;
            if (impactSpeed >= minBounceSpeed)
            {
                float volumeScale = Mathf.Clamp01(impactSpeed / maxBounceReferenceSpeed);
                AudioManager.Instance?.PlayBallBounce(volumeScale);
            }
        }

        // Single source of truth for "is this ball allowed to be grabbed by this side right now".
        // Free balls are always grabbable, Held balls never are. InFlight balls are grabbable by
        // anyone EXCEPT the side that just threw them, and only for a short immunity window right
        // after release -- this is what stops a teammate (or the thrower's own overlapping collider)
        // from catching the ball the instant it leaves the hand, while still allowing a normal
        // same-team pass to be picked up once the window has elapsed.
        public bool CanBeGrabbedBy(EPlayerSide side)
        {
            switch (State)
            {
                case BallState.Free:
                    return true;
                case BallState.Held:
                    return false;
                case BallState.InFlight:
                    bool isSameSideAsThrower = side == owningSide;
                    bool withinImmunityWindow = Time.time - _thrownAtTime < sameSideGrabImmunityDuration;
                    return !(isSameSideAsThrower && withinImmunityWindow);
                default:
                    return false;
            }
        }

        private void FixedUpdate()
        {
            if (_outOfBoundsTriggered || RoundManager.Instance == null)
            {
                return;
            }

            if (RoundManager.Instance.IsBallOutOfBounds(transform))
            {
                _outOfBoundsTriggered = true;
                RoundManager.Instance.EndRound(ERoundEndReason.OOB, owningSide);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.position + new Vector3(0, .5f, 0), "State: " + State);
        }
#endif
    }
}
