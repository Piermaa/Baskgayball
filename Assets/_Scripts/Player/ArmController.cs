using BaskgayBall.Input;
using UnityEngine;

namespace BaskgayBall.Player
{
    public class ArmController : MonoBehaviour, IPrefabSubLogic
    {
        [Header("")]
        [SerializeField] private float restAngle = -20f;
        [SerializeField] private float maxChargeAngle = 90f;
        [SerializeField] private float chargeSpeedDegPerSec = 220f;
        [SerializeField] private float minThrowForce = 4f;
        [SerializeField] private float maxThrowForce = 14f;
        [SerializeField] private float maxChargeTimeForFullForce = 0.8f;
        [SerializeField] private Transform handTransform;
        [SerializeField] private float verticalBoost = 0.5f;

        [SerializeField] private Transform hoopTarget;
        [SerializeField] private AnimationCurve xWeightByDistance = AnimationCurve.Linear(2f, 0.4f, 10f, 0.9f);
        [SerializeField] private float aimRandomnessDegrees = 4f;
        [SerializeField] private float aimRandomnessForceMultiplier = 0.08f;
        [SerializeField] private Transform prefabRoot;


        [Header("Trajectory Gizmo")]
        [SerializeField] private bool showTrajectoryGizmo = true;
        [SerializeField] private int trajectorySteps = 30;
        [SerializeField] private float trajectoryTimeStep = 0.05f;
        [SerializeField] private float ballGravityScale = 1f;
        [SerializeField] private Color trajectoryGizmoColor = Color.red;

        private float _currentAngle;
        private float _chargeElapsed;
        private bool _isCharging;

        public Transform PrefabRoot => prefabRoot;

        private void Start()
        {
            _currentAngle = restAngle;
            ApplyRotation();

            if (prefabRoot.TryGetComponent<ISided>(out var sided))
            {
                hoopTarget = sided.Side == EPlayerSide.Player1
                    ? RoundManager.Instance.Player2Hoop.transform
                    : RoundManager.Instance.Player1Hoop.transform;
            }

            Debug.DrawLine(transform.position, hoopTarget.position, Color.yellow, 10f);
        }

        private void Update()
        {
            if (_isCharging)
            {
                _chargeElapsed += Time.deltaTime;
                _currentAngle = Mathf.MoveTowards(_currentAngle, maxChargeAngle, chargeSpeedDegPerSec * Time.deltaTime);
            }
            else if (!Mathf.Approximately(_currentAngle, restAngle))
            {
                _currentAngle = Mathf.MoveTowards(_currentAngle, restAngle, chargeSpeedDegPerSec * Time.deltaTime);
            }

            ApplyRotation();
        }

        public void BeginCharge()
        {
            _isCharging = true;
            _chargeElapsed = 0f;
        }

        public Vector2 EndChargeAndGetThrowVelocity()
        {
            _isCharging = false;

            float chargeRatio = Mathf.Clamp01(_chargeElapsed / maxChargeTimeForFullForce);
            float baseForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeRatio);
            float randomizedForce = baseForce * (1f + Random.Range(-aimRandomnessForceMultiplier, aimRandomnessForceMultiplier));

            Vector2 direction = ComputeAimDirection(true);

            return direction * randomizedForce;
        }

        private Vector2 ComputeAimDirection(bool applyRandomness)
        {
            float horizontalDelta = hoopTarget != null
                ? hoopTarget.position.x - transform.position.x
                : handTransform.position.x - transform.position.x;

            float facingSign = Mathf.Sign(horizontalDelta);
            float horizontalDistance = Mathf.Abs(horizontalDelta);

            float xWeight = Mathf.Clamp01(xWeightByDistance.Evaluate(horizontalDistance));
            float yWeight = Mathf.Sqrt(Mathf.Max(0f, 1f - xWeight * xWeight)) + verticalBoost;

            Vector2 baseDirection = new Vector2(xWeight * facingSign, yWeight);

            if (!applyRandomness)
            {
                return baseDirection.normalized;
            }

            float randomAngleOffsetDeg = Random.Range(-aimRandomnessDegrees, aimRandomnessDegrees);
            Vector2 aimedDirection = Quaternion.Euler(0f, 0f, randomAngleOffsetDeg) * baseDirection;

            return aimedDirection.normalized;
        }

        private void ApplyRotation()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
        }

        private void OnDrawGizmos()
        {
            if (!showTrajectoryGizmo || !Application.isPlaying || handTransform == null)
            {
                return;
            }

            float chargeRatio = Mathf.Clamp01(_chargeElapsed / maxChargeTimeForFullForce);
            float previewChargeRatio = _isCharging ? chargeRatio : 1f;
            float force = Mathf.Lerp(minThrowForce, maxThrowForce, previewChargeRatio);
            Vector2 velocity = ComputeAimDirection(false) * force;

            Vector2 origin = handTransform.position;
            Vector2 gravity = Physics2D.gravity * ballGravityScale;
            Vector2 previousPoint = origin;

            Gizmos.color = trajectoryGizmoColor;

            for (int step = 1; step <= trajectorySteps; step++)
            {
                float time = step * trajectoryTimeStep;
                Vector2 point = origin + velocity * time + 0.5f * gravity * time * time;
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }
    }
}