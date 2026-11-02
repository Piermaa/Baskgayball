using BaskgayBall.Core;
using BaskgayBall.PowerUps;
using BaskgayBall.Stage;
using UnityEngine;

namespace BaskgayBall.Player
{
    enum EPlayerBodyRadgollState
    {
        Start,
        JustLanded,
        Wobblinng,
        Stable,
        OnAir
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class BodyMovement : MonoBehaviour, IPrefabSubLogic, IResettable
    {
        public Transform PrefabRoot => prefabRoot;

        [Header("Jump and falling")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.12f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Wobbling sprint movement")]
        [SerializeField] private float uprightSpringStrength = 40f;
        [SerializeField] private float uprightSpringDamping = 4f;
        [SerializeField] private float landingWobbleTorque = 8f;
        [SerializeField] private float landingReferenceSpeed = 12f;

        [Header("Start Wobble")]
        [SerializeField] private float startWobbleTorque = 8f;

        [Header("Lean Detection")]
        [Tooltip("Rotation magnitude, in degrees, below which the body is considered upright with no established lean yet (e.g. right at round start). Below this, wobble direction falls back to spin, then facing.")]
        [SerializeField] private float uprightDeadZoneDegrees = 2f;

        [Header("Setup")]
        [SerializeField] private Transform prefabRoot;

        private Rigidbody2D _rigidbody;
        private bool _isHolding;
        private bool _wasGrounded;
        private EPlayerBodyRadgollState ragdollState;
        private float _previousVerticalVelocity;
        public bool IsGrounded { get; private set; }

        private PlayerModifiers _modifiers;
        private float _baseGravityScale;
        private float _baseLinearDamping;
        private float _baseAngularDamping;

        private SurfaceProfile _surface => RoundManager.Instance != null ? RoundManager.Instance.CurrentSurface : null;

        private float SpringStrength => uprightSpringStrength * (_surface != null ? _surface.UprightSpringMultiplier : 1f);
        private float SpringDamping => uprightSpringDamping * (_surface != null ? _surface.UprightDampingMultiplier : 1f);
        private float LandingWobble => landingWobbleTorque * (_surface != null ? _surface.LandingWobbleMultiplier : 1f);

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.centerOfMass = groundCheck.localPosition;

            _baseGravityScale = _rigidbody.gravityScale;
            _baseLinearDamping = _rigidbody.linearDamping;
            _baseAngularDamping = _rigidbody.angularDamping;
            prefabRoot.TryGetComponent(out _modifiers);
            ResetState();
        }

        private float GetTimeScale()
        {
            return _modifiers != null ? _modifiers.TimeScale : 1f;
        }

        private void ApplyTimeScaleToBody(float timeScale)
        {
            _rigidbody.gravityScale = _baseGravityScale * timeScale * timeScale;
            _rigidbody.linearDamping = _baseLinearDamping * timeScale;
            _rigidbody.angularDamping = _baseAngularDamping * timeScale;
        }

        private void FixedUpdate()
        {
            float timeScale = GetTimeScale();
            float accelerationScale = timeScale * timeScale;
            ApplyTimeScaleToBody(timeScale);

            bool groundedNow = groundCheck != null &&
                Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (!_wasGrounded && groundedNow)
            {
                ragdollState = EPlayerBodyRadgollState.JustLanded;
                ApplyLandingWobble(timeScale);
            }
            if (!groundedNow)
            {
                ragdollState = EPlayerBodyRadgollState.OnAir;
            }

            _wasGrounded = groundedNow;
            IsGrounded = groundedNow;

            if (IsGrounded && _surface != null && _surface.GroundHorizontalDrag > 0f)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                velocity.x *= Mathf.Max(0f, 1f - _surface.GroundHorizontalDrag * Time.fixedDeltaTime);
                _rigidbody.linearVelocity = velocity;
            }

            if (_rigidbody.linearVelocity.y < 0f)
            {
                _rigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * accelerationScale * Time.fixedDeltaTime;
            }
            else if (_rigidbody.linearVelocity.y > 0f && !_isHolding)
            {
                _rigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * accelerationScale * Time.fixedDeltaTime;
            }

            switch (ragdollState)
            {
                case EPlayerBodyRadgollState.JustLanded:
                case EPlayerBodyRadgollState.Wobblinng:
                case EPlayerBodyRadgollState.Start:
                    if (IsGrounded)
                    {
                        ApplyUprightSpring(timeScale);
                    }
                    break;
                case EPlayerBodyRadgollState.Stable:
                    break;
                case EPlayerBodyRadgollState.OnAir:
                    _rigidbody.angularVelocity = 0f;
                    break;
            }

            _previousVerticalVelocity = _rigidbody.linearVelocity.y;
        }


        private void ApplyStartingWobble()
        {
            float wobbleToApply = startWobbleTorque * GetLeanSign() * GetTimeScale();
            wobbleToApply *= Random.Range(.5f, 1.5f);
            _rigidbody.AddTorque(wobbleToApply, ForceMode2D.Impulse);
        }

        private void ApplyLandingWobble(float timeScale)
        {
            float impactSpeed = Mathf.Abs(_previousVerticalVelocity);
            float wobbleAmount = Mathf.Clamp01(impactSpeed / (landingReferenceSpeed * timeScale));
            _rigidbody.AddTorque(LandingWobble * wobbleAmount * GetLeanSign() * timeScale, ForceMode2D.Impulse);
        }

        // Picks which way a wobble impulse should push. It reinforces whatever lean the body
        // already has (or, if it is not visibly tilted yet, whatever spin it already has), instead
        // of picking a direction unrelated to the current physical state. That mismatch was what
        // made landing wobbles randomly fight the ragdoll's existing tilt and cancel it out,
        // freezing the character stiff right after landing. Facing direction is kept only as the
        // last-resort fallback, for the one case with no lean and no spin at all: round start.
        private float GetLeanSign()
        {
            float tiltAngleDeg = Mathf.DeltaAngle(0f, _rigidbody.rotation);

            if (Mathf.Abs(tiltAngleDeg) > uprightDeadZoneDegrees)
            {
                return Mathf.Sign(tiltAngleDeg);
            }

            if (Mathf.Abs(_rigidbody.angularVelocity) > 0.01f)
            {
                return Mathf.Sign(_rigidbody.angularVelocity);
            }

            bool facesRight = prefabRoot.GetComponent<PlayerController>().FacesRight;
            return facesRight ? -1f : 1f;
        }

        private void ApplyUprightSpring(float timeScale)
        {
            float angleRad = Mathf.DeltaAngle(0f, _rigidbody.rotation) * Mathf.Deg2Rad;
            float angularVelocityRad = _rigidbody.angularVelocity * Mathf.Deg2Rad;
            float torque = -SpringStrength * timeScale * timeScale * angleRad
                           -SpringDamping * timeScale * angularVelocityRad;
            _rigidbody.AddTorque(torque);
        }
        public void Jump()
        {
            if (!IsGrounded) return;
            if (_modifiers != null && _modifiers.IsJumpBlocked) return;

            Vector2 direction = transform.up;
            if (_modifiers != null && _modifiers.IsJumpDirectionInverted)
            {
                print("Direction inverted;");
                direction = new Vector2(-direction.x, direction.y);
            }

            float multiplier = _modifiers != null ? _modifiers.JumpForceMultiplier * _modifiers.TimeScale : 1f;

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(direction * jumpForce * multiplier, ForceMode2D.Impulse);
        }

        public void SetHolding(bool isHolding)
        {
            _isHolding = isHolding;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;

            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        }

        public void ResetState()
        {
            ragdollState = EPlayerBodyRadgollState.Start;
            IsGrounded = true;
            ApplyStartingWobble();
        }
    }
}