using BaskgayBall.Input;
using BaskgayBall.Player;
using System;
using System.Collections;
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

        [Header("VFX")]
        [SerializeField] private ParticleSystem spawnVfxPrefab;
        [SerializeField] private ParticleSystem pickupVfxPrefab;
        [SerializeField][Min(0f)] private float spawnVfxDuration = 1f;

        [Header("Visuals State")]
        [SerializeField] private GameObject visualsRoot;
        [SerializeField] private bool hideVisualsDuringSpawnVfx = true;

        private Collider2D _collider;
        private Coroutine _spawnRoutine;
        private bool _isPickupable;
        private bool _resolved;

        public PowerUpDefinition Definition { get; private set; }
        public ParticleSystem SpawnVfxPrefab => spawnVfxPrefab;
        public ParticleSystem PickupVfxPrefab => pickupVfxPrefab;
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

            ParticleSystem activeSpawnVfx = null;
            if (spawnVfxPrefab != null)
            {
                activeSpawnVfx = Instantiate(spawnVfxPrefab, transform.position, Quaternion.identity, transform);
                ApplyColorToParticleSystem(activeSpawnVfx, polarityColor);

                if (vfxDuration > 0.001f && activeSpawnVfx.main.duration > 0.001f)
                {
                    var main = activeSpawnVfx.main;
                    main.simulationSpeed = activeSpawnVfx.main.duration / vfxDuration;
                }

                activeSpawnVfx.Play();
            }

            if (vfxDuration > 0f)
            {
                yield return new WaitForSeconds(vfxDuration);
            }

            if (_resolved)
            {
                yield break;
            }

            if (activeSpawnVfx != null)
            {
                Destroy(activeSpawnVfx.gameObject);
            }

            SetVisualsActive(true);
            _isPickupable = true;
            if (_collider != null)
            {
                _collider.enabled = true;
            }

            yield return LifetimeRoutine(lifetime);
        }

        private IEnumerator LifetimeRoutine(float lifetime)
        {
            yield return new WaitForSeconds(lifetime);

            if (_resolved)
            {
                yield break;
            }

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
            PlayPickupVfx();
            Collected?.Invoke(this, modifiers.Side);
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

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = active;
            }
        }
    }
}
