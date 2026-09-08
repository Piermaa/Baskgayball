using UnityEngine;

namespace BaskgayBall.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BodyMovement : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.12f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Wobble (peso en la base)")]
        [SerializeField] private float uprightSpringStrength = 40f;
        [SerializeField] private float uprightSpringDamping = 4f;
        [SerializeField] private float landingWobbleTorque = 8f;
        [SerializeField] private float landingReferenceSpeed = 12f;

        private Rigidbody2D _rigidbody;
        private bool _isHolding;
        private bool _wasGrounded;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            bool groundedNow = groundCheck != null &&
                Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (!_wasGrounded && groundedNow)
            {
                ApplyLandingWobble();
            }

            _wasGrounded = groundedNow;
            IsGrounded = groundedNow;

            if (_rigidbody.linearVelocity.y < 0f)
            {
                _rigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
            }
            else if (_rigidbody.linearVelocity.y > 0f && !_isHolding)
            {
                _rigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
            }


            if (IsGrounded)
            {
                ApplyUprightSpring();
            }
            else
            {
                _rigidbody.angularVelocity = 0;
            }
        }

        private void ApplyLandingWobble()
        {
            float impactSpeed = Mathf.Abs(_rigidbody.linearVelocity.y);
            float direction = Mathf.Abs(_rigidbody.linearVelocity.x) > 0.01f
                ? Mathf.Sign(_rigidbody.linearVelocity.x)
                : (Random.value > 0.5f ? 1f : -1f);

            float wobbleAmount = Mathf.Clamp01(impactSpeed / landingReferenceSpeed);
            _rigidbody.AddTorque(direction * landingWobbleTorque * wobbleAmount, ForceMode2D.Impulse);
        }

        private void ApplyUprightSpring()
        {
            float angle = Mathf.DeltaAngle(0f, _rigidbody.rotation);
            float torque = -uprightSpringStrength * angle - uprightSpringDamping * _rigidbody.angularVelocity;
            _rigidbody.AddTorque(torque);
        }

        public void Jump()
        {
            if (!IsGrounded) return;

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
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
    }
}