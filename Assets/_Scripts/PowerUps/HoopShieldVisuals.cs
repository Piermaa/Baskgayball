using BaskgayBall.Ball;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public class HoopShieldVisuals : MonoBehaviour
    {
        [Header("Energy Pulse")]
        [SerializeField] private SpriteRenderer shieldRenderer;
        [SerializeField] private bool enablePulse = true;
        [SerializeField] private float pulseSpeed = 4f;
        [SerializeField] private float minAlpha = 0.65f;
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private bool enableScalePulse = true;
        [SerializeField] private float scalePulseAmount = 0.05f;

        [Header("Impact VFX")]
        [SerializeField] private ParticleSystem impactSparksPrefab;
        [SerializeField] private Color impactFlashColor = Color.white;
        [SerializeField] private float flashDuration = 0.12f;

        [Header("Activation VFX")]
        [SerializeField] private ParticleSystem activationVfxPrefab;

        private Color _baseColor = Color.cyan;
        private float _flashTimer;
        private Vector3 _originalScale;

        public ParticleSystem ImpactSparksPrefab { get => impactSparksPrefab; set => impactSparksPrefab = value; }
        public ParticleSystem ActivationVfxPrefab { get => activationVfxPrefab; set => activationVfxPrefab = value; }

        private void Awake()
        {
            if (shieldRenderer == null)
            {
                shieldRenderer = GetComponent<SpriteRenderer>();
            }

            if (shieldRenderer != null)
            {
                _baseColor = shieldRenderer.color;
            }

            _originalScale = transform.localScale;
        }

        private void OnEnable()
        {
            if (shieldRenderer != null)
            {
                shieldRenderer.color = _baseColor;
            }

            transform.localScale = _originalScale;

            if (activationVfxPrefab != null)
            {
                ParticleSystem fx = Instantiate(activationVfxPrefab, transform.position, Quaternion.identity);
                Destroy(fx.gameObject, 1.5f);
            }
        }

        private void OnDisable()
        {
            if (shieldRenderer != null)
            {
                shieldRenderer.color = _baseColor;
            }

            transform.localScale = _originalScale;
            _flashTimer = 0f;
        }

        private void Update()
        {
            if (shieldRenderer == null)
            {
                return;
            }

            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
                float flashT = Mathf.Clamp01(_flashTimer / flashDuration);
                shieldRenderer.color = Color.Lerp(_baseColor, impactFlashColor, flashT);
                return;
            }

            if (enablePulse)
            {
                float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
                Color c = _baseColor;
                c.a = alpha;
                shieldRenderer.color = c;

                if (enableScalePulse)
                {
                    float scaleFactor = 1f + (t - 0.5f) * scalePulseAmount;
                    transform.localScale = new Vector3(_originalScale.x * scaleFactor, _originalScale.y * scaleFactor, _originalScale.z);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Flash shield upon impact
            _flashTimer = flashDuration;

            if (impactSparksPrefab != null && collision.contactCount > 0)
            {
                ContactPoint2D contact = collision.GetContact(0);
                ParticleSystem sparks = Instantiate(impactSparksPrefab, contact.point, Quaternion.identity);
                var main = sparks.main;
                main.startColor = _baseColor;
                sparks.Play();
                Destroy(sparks.gameObject, 1.2f);
            }
        }
    }
}
