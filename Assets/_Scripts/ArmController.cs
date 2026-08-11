using UnityEngine;

namespace BaskgayBall.Player
{
   
    public class ArmController : MonoBehaviour
    {
        [Header("Ángulos (grados, 0 = horizontal)")]
        [SerializeField] private float restAngle = -20f;
        [SerializeField] private float maxChargeAngle = 90f;

        [Header("Velocidad de carga")]
        [SerializeField] private float chargeSpeedDegPerSec = 220f;

        [Header("Tiro")]
        [SerializeField] private float minThrowForce = 4f;
        [SerializeField] private float maxThrowForce = 14f;
        [SerializeField] private float maxChargeTimeForFullForce = 0.8f;

        private float _currentAngle;
        private float _chargeElapsed;
        private bool _isCharging;
        private float _facingSign = 1f; // 1 = mira a la derecha, -1 = mira a la izquierda

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

    
        public void SetFacing(bool facesRight)
        {
            _facingSign = -1;
        }

        public Vector2 EndChargeAndGetThrowVelocity()
        {
            _isCharging = false;

            float chargeRatio = Mathf.Clamp01(_chargeElapsed / maxChargeTimeForFullForce);
            float force = Mathf.Lerp(minThrowForce, maxThrowForce, chargeRatio);

            float angleRad = _currentAngle * Mathf.Deg2Rad;
            Vector2 localDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Vector2 direction = transform.parent != null
                ? transform.parent.TransformDirection(localDirection)
                : localDirection;

            direction.x *= _facingSign;

            return direction.normalized * force;
        }

        private void ApplyRotation()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
        }
    }
}