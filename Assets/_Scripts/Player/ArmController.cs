using UnityEngine;

namespace BaskgayBall.Player
{
    public class ArmController : MonoBehaviour
    {
        [SerializeField] private float restAngle = -20f;
        [SerializeField] private float maxChargeAngle = 90f;
        [SerializeField] private float chargeSpeedDegPerSec = 220f;
        [SerializeField] private float minThrowForce = 4f;
        [SerializeField] private float maxThrowForce = 14f;
        [SerializeField] private float maxChargeTimeForFullForce = 0.8f;
        [SerializeField] private Transform handTransform;
        [SerializeField] private float verticalBoost = 0.5f;

        private float _currentAngle;
        private float _chargeElapsed;
        private bool _isCharging;

        private void Awake()
        {
            _currentAngle = restAngle;
            ApplyRotation();
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
            float force = Mathf.Lerp(minThrowForce, maxThrowForce, chargeRatio);

            Vector2 rawDirection = (Vector2)handTransform.position - (Vector2)transform.position;
            rawDirection.y += verticalBoost;
            Vector2 direction = rawDirection.normalized;
            direction.x *= -1;
            return direction * force;
        }

        private void ApplyRotation()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
        }
    }
}