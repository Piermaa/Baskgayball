using BaskgayBall.Input;
using BaskgayBall.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    [RequireComponent(typeof(Collider2D))]
    public class PowerUpPickup : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;

        [Header("Renderers")]
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private SpriteRenderer iconBgRenderer;

        [Header("VFX - Inward Spawn")]
        [SerializeField] private ParticleSystem spawnVfxPrefab;
        [SerializeField][Min(0f)] private float spawnVfxDuration = 1.2f;
        [Tooltip("How long before the power-up appears to stop spawning inward particles, allowing existing ones to reach center.")]
        [SerializeField][Min(0f)] private float stopSpawningLeadTime = 0.5f;

        [Header("VFX - Radial Spawn Burst")]
        [Tooltip("Burst VFX spawned when the power up finishes materializing. Falls back to pickupVfxPrefab if null.")]
        [SerializeField] private ParticleSystem spawnRadialBurstVfxPrefab;

        [Header("VFX - Pickup Explosion")]
        [SerializeField] private ParticleSystem pickupVfxPrefab;

        [Header("Visuals State")]
        [SerializeField] private GameObject visualsRoot;
        [SerializeField] private bool hideVisualsDuringSpawnVfx = true;

        [Header("Despawn Pulse Warning")]
        [SerializeField][Min(0f)] private float despawnWarningDuration = 1.8f;
        [SerializeField] private float despawnPulseSpeed = 8f;
        [SerializeField] private float despawnPulseScaleAmount = 0.2f;
        [SerializeField] private bool despawnAcceleratePulse = true;
        [SerializeField] private bool despawnPulseAlpha = true;
        [SerializeField] private float despawnMinAlpha = 0.35f;

        private Collider2D _collider;
        private Coroutine _spawnRoutine;
        private bool _isPickupable;
        private bool _resolved;
        private Vector3 _initialScale;
        private ParticleSystem _activeSpawnVfx;

        private SpriteRenderer[] _cachedRenderers;
        private readonly Dictionary<SpriteRenderer, Color> _baseRendererColors =
            new Dictionary<SpriteRenderer, Color>();

        public PowerUpDefinition Definition { get; private set; }
        public ParticleSystem SpawnVfxPrefab => spawnVfxPrefab;
        public ParticleSystem PickupVfxPrefab => pickupVfxPrefab;
        public ParticleSystem SpawnRadialBurstVfxPrefab => spawnRadialBurstVfxPrefab;
        public float SpawnVfxDuration => spawnVfxDuration;
        public bool IsPickupable => _isPickupable;

        public event Action<PowerUpPickup, EPlayerSide> Collected;
        public event Action<PowerUpPickup> Expired;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _isPickupable = false;
            if (_collider != null)
            {
                _collider.enabled = false;
            }

            _initialScale = transform.localScale;
            CacheRendererColors();
        }

        private void OnDestroy()
        {
            if (_activeSpawnVfx != null)
            {
                Destroy(_activeSpawnVfx.gameObject);
            }
        }

        public void Initialize(PowerUpDefinition definition, float lifetime, float? overrideSpawnVfxDuration = null)
        {
            Definition = definition;
            if (iconRenderer != null)
            {
                iconRenderer.sprite = definition.Icon;
            }

            Color polarityColor = definition.Polarity == EPowerUpPolarity.Positive ? positiveColor : negativeColor;
            if (iconBgRenderer != null)
            {
                iconBgRenderer.color = polarityColor;
            }

            // Update base colors for alpha blending
            CacheRendererColors();

            float duration = overrideSpawnVfxDuration ?? spawnVfxDuration;

            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
            }
            _spawnRoutine = StartCoroutine(SpawnSequenceRoutine(lifetime, polarityColor, duration));
        }

        private IEnumerator SpawnSequenceRoutine(float lifetime, Color polarityColor, float vfxDuration)
        {
            _isPickupable = false;
            if (_collider != null)
            {
                _collider.enabled = false;
            }

            if (hideVisualsDuringSpawnVfx)
            {
                SetVisualsActive(false);
            }

            _activeSpawnVfx = null;
            if (spawnVfxPrefab != null)
            {
                _activeSpawnVfx = Instantiate(spawnVfxPrefab, transform.position, Quaternion.identity, transform);
                ApplyColorToParticleSystem(_activeSpawnVfx, polarityColor);

                if (vfxDuration > 0.001f && _activeSpawnVfx.main.duration > 0.001f)
                {
                    var main = _activeSpawnVfx.main;
                    main.simulationSpeed = _activeSpawnVfx.main.duration / vfxDuration;
                }

                _activeSpawnVfx.Play();
            }

            if (vfxDuration > 0f)
            {
                // Stop spawning new particles before spawn so particles in flight finish their journey
                float leadTime = Mathf.Clamp(stopSpawningLeadTime, 0f, vfxDuration * 0.7f);
                float activeEmittingTime = vfxDuration - leadTime;

                if (activeEmittingTime > 0f)
                {
                    yield return new WaitForSeconds(activeEmittingTime);
                }

                if (_resolved)
                {
                    yield break;
                }

                // Stop emitting new particles
                if (_activeSpawnVfx != null)
                {
                    _activeSpawnVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }

                if (leadTime > 0f)
                {
                    yield return new WaitForSeconds(leadTime);
                }
            }

            if (_resolved)
            {
                yield break;
            }

            // Detach active spawn VFX so lingering particles reach center naturally without popping
            if (_activeSpawnVfx != null)
            {
                _activeSpawnVfx.transform.SetParent(null);
                Destroy(_activeSpawnVfx.gameObject, 1.2f);
                _activeSpawnVfx = null;
            }

            // Radial sparks burst right as power-up materializes
            PlayRadialSpawnBurst(polarityColor);

            // Reveal power-up visuals and enable collider
            SetVisualsActive(true);
            SetVisualsAlpha(1f);
            transform.localScale = _initialScale;
            _isPickupable = true;
            if (_collider != null)
            {
                _collider.enabled = true;
            }

            yield return LifetimeRoutine(lifetime);
        }

        private IEnumerator LifetimeRoutine(float lifetime)
        {
            float warningDuration = Mathf.Clamp(despawnWarningDuration, 0f, lifetime * 0.8f);
            float normalLifetime = lifetime - warningDuration;

            if (normalLifetime > 0f)
            {
                yield return new WaitForSeconds(normalLifetime);
            }

            if (_resolved)
            {
                yield break;
            }

            // Despawn pulse warning phase
            float elapsed = 0f;
            while (elapsed < warningDuration)
            {
                if (_resolved)
                {
                    yield break;
                }

                elapsed += Time.deltaTime;
                float progress = warningDuration > 0.001f ? Mathf.Clamp01(elapsed / warningDuration) : 1f;

                float speedMultiplier = despawnAcceleratePulse ? Mathf.Lerp(1f, 2.5f, progress) : 1f;
                float currentSpeed = despawnPulseSpeed * speedMultiplier;

                // Scale pulse
                float pulse = Mathf.Sin(elapsed * currentSpeed);
                float scaleFactor = 1f + pulse * despawnPulseScaleAmount;
                transform.localScale = _initialScale * scaleFactor;

                // Alpha pulse
                if (despawnPulseAlpha)
                {
                    float alphaT = (pulse + 1f) * 0.5f;
                    float alpha = Mathf.Lerp(despawnMinAlpha, 1f, alphaT);
                    SetVisualsAlpha(alpha);
                }

                yield return null;
            }

            if (_resolved)
            {
                yield break;
            }

            transform.localScale = _initialScale;
            SetVisualsAlpha(1f);

            _resolved = true;
            Expired?.Invoke(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryHandlePickup(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryHandlePickup(other);
        }

        private void TryHandlePickup(Collider2D other)
        {
            if (_resolved || !_isPickupable)
            {
                return;
            }

            Rigidbody2D body = other.attachedRigidbody;
            if (body == null || !body.TryGetComponent<BodyMovement>(out _))
            {
                return;
            }

            PlayerModifiers modifiers = body.GetComponentInParent<PlayerModifiers>();
            if (modifiers == null)
            {
                return;
            }

            _resolved = true;
            transform.localScale = _initialScale;
            SetVisualsAlpha(1f);
            PlayPickupVfx();
            Collected?.Invoke(this, modifiers.Side);
        }

        private void PlayRadialSpawnBurst(Color polarityColor)
        {
            ParticleSystem prefabToUse = spawnRadialBurstVfxPrefab != null ? spawnRadialBurstVfxPrefab : pickupVfxPrefab;
            if (prefabToUse == null)
            {
                return;
            }

            ParticleSystem burst = Instantiate(prefabToUse, transform.position, Quaternion.identity);
            ApplyColorToParticleSystem(burst, polarityColor);
            burst.Play();

            float destroyDelay = burst.main.duration + burst.main.startLifetime.constantMax + 0.5f;
            Destroy(burst.gameObject, destroyDelay);
        }

        private void PlayPickupVfx()
        {
            if (pickupVfxPrefab == null)
            {
                return;
            }

            ParticleSystem vfx = Instantiate(pickupVfxPrefab, transform.position, Quaternion.identity);
            Color polarityColor = Definition != null && Definition.Polarity == EPowerUpPolarity.Positive
                ? positiveColor
                : negativeColor;
            ApplyColorToParticleSystem(vfx, polarityColor);
            vfx.Play();

            float destroyDelay = vfx.main.duration + vfx.main.startLifetime.constantMax + 0.5f;
            Destroy(vfx.gameObject, destroyDelay);
        }

        private void ApplyColorToParticleSystem(ParticleSystem ps, Color color)
        {
            if (ps == null)
            {
                return;
            }

            var main = ps.main;
            main.startColor = color;

            ParticleSystem[] children = ps.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < children.Length; i++)
            {
                var childMain = children[i].main;
                childMain.startColor = color;
            }
        }

        private void SetVisualsActive(bool active)
        {
            if (visualsRoot != null)
            {
                visualsRoot.SetActive(active);
                return;
            }

            if (iconRenderer != null)
            {
                iconRenderer.enabled = active;
            }

            if (iconBgRenderer != null)
            {
                iconBgRenderer.enabled = active;
            }

            if (_cachedRenderers == null || _cachedRenderers.Length == 0)
            {
                _cachedRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            }

            for (int i = 0; i < _cachedRenderers.Length; i++)
            {
                if (_cachedRenderers[i] != null)
                {
                    _cachedRenderers[i].enabled = active;
                }
            }
        }

        private void SetVisualsAlpha(float alpha)
        {
            if (_cachedRenderers == null)
            {
                return;
            }

            for (int i = 0; i < _cachedRenderers.Length; i++)
            {
                SpriteRenderer sr = _cachedRenderers[i];
                if (sr != null && _baseRendererColors.TryGetValue(sr, out Color baseColor))
                {
                    Color c = baseColor;
                    c.a = baseColor.a * alpha;
                    sr.color = c;
                }
            }
        }

        private void CacheRendererColors()
        {
            _baseRendererColors.Clear();
            _cachedRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            for (int i = 0; i < _cachedRenderers.Length; i++)
            {
                SpriteRenderer sr = _cachedRenderers[i];
                if (sr != null && !_baseRendererColors.ContainsKey(sr))
                {
                    _baseRendererColors[sr] = sr.color;
                }
            }
        }
    }
}
