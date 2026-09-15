using UnityEditor;
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
    public class BodyMovement : MonoBehaviour, IPrefabSubLogic
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

        [Header("Setup")]
        [SerializeField] private Transform prefabRoot;

        private Rigidbody2D _rigidbody;
        private bool _isHolding;
        private bool _wasGrounded;
        private EPlayerBodyRadgollState ragdollState;

        public bool IsGrounded { get; private set; }


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.centerOfMass = groundCheck.localPosition;
            ragdollState = EPlayerBodyRadgollState.Start;
            IsGrounded = true;
            ApplyStartingWobble();
        }

        private void FixedUpdate()
        {
            bool groundedNow = groundCheck != null &&
                Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (!_wasGrounded && groundedNow)
            {
                ragdollState = EPlayerBodyRadgollState.JustLanded;
                ApplyLandingWobble();
            }
            if (!groundedNow)
            {
                ragdollState = EPlayerBodyRadgollState.OnAir;    
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


            switch (ragdollState)
            {
                case EPlayerBodyRadgollState.JustLanded:
                case EPlayerBodyRadgollState.Wobblinng:
                case EPlayerBodyRadgollState.Start:
                    if (IsGrounded)
                    {
                        ApplyUprightSpring();
                    }
                    break;
                case EPlayerBodyRadgollState.Stable:
                    break;
                case EPlayerBodyRadgollState.OnAir:
                _rigidbody.angularVelocity = 0f;

                    break;
            }
        }
        private void ApplyStartingWobble()
        {
            float wobbleToApply = startWobbleTorque;
            bool invertWobble = prefabRoot.GetComponent<PlayerController>().FacesRight;
            wobbleToApply = invertWobble ? -wobbleToApply : wobbleToApply;
            wobbleToApply *= Random.Range(.5f, 1.5f);
            _rigidbody.AddTorque(wobbleToApply, ForceMode2D.Impulse);
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
            float angleRad = Mathf.DeltaAngle(0f, _rigidbody.rotation) * Mathf.Deg2Rad;
            float angularVelocityRad = _rigidbody.angularVelocity * Mathf.Deg2Rad;
            float torque = -uprightSpringStrength * angleRad - uprightSpringDamping * angularVelocityRad;
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
        //private void OnDrawGizmos()
        //{
        //    Handles.Label(
        //        transform.position + new Vector3(0, 1, 0),
        //        ragdollState.ToString());
        //}
    }
}