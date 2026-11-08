using BaskgayBall.PowerUps;
using System.Collections.Generic;
using UnityEngine;

namespace BaskgayBall.Player
{
    [DisallowMultipleComponent]
    public class PlayerPowerUpVisuals : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerModifiers modifiers;
        [SerializeField] private Transform feetPoint;
        [SerializeField] private Transform bodyPoint;
        [SerializeField] private Transform headPoint;
        [SerializeField] private SpriteRenderer[] characterRenderers;

        [Header("Feet Visuals (SuperJump)")]
        [SerializeField] private Sprite bootsSprite;
        [SerializeField] private GameObject bootsPrefab;
        [SerializeField] private Vector3 bootsOffset = Vector3.zero;
        [SerializeField] private Vector3 bootsScale = Vector3.one;

        [Header("Feet Visuals (NoJump & BlockRivalJump)")]
        [SerializeField] private Sprite chainsSprite;
        [SerializeField] private GameObject chainsPrefab;
        [SerializeField] private Vector3 chainsOffset = Vector3.zero;
        [SerializeField] private Vector3 chainsScale = Vector3.one;

        [Header("Feet Sorting")]
        [SerializeField] private string feetSortingLayerName = "Player_Body";
        [SerializeField] private int feetSortingOrder = 10;

        [Header("Cold Effects (Slow & FreezeRival)")]
        [SerializeField] private Color coldTintColor = new Color(0.5f, 0.8f, 1f, 1f);
        [SerializeField] private ParticleSystem snowflakesVfxPrefab;
        [SerializeField] private Vector3 snowflakesOffset = Vector3.zero;

        [Header("Invert Jump Effect")]
        [SerializeField] private GameObject invertJumpVfxPrefab;
        [SerializeField] private Vector3 invertJumpOffset = new Vector3(0f, 0.8f, 0f);

        // Runtime spawned instances
        private GameObject _bootsInstance;
        private GameObject _chainsInstance;
        private ParticleSystem _snowflakesInstance;
        private GameObject _invertJumpInstance;

        private readonly Dictionary<SpriteRenderer, Color> _originalColors =
            new Dictionary<SpriteRenderer, Color>();
        private bool _isColdActive;

        public Sprite BootsSprite { get => bootsSprite; set => bootsSprite = value; }
        public GameObject BootsPrefab { get => bootsPrefab; set => bootsPrefab = value; }
        public Sprite ChainsSprite { get => chainsSprite; set => chainsSprite = value; }
        public GameObject ChainsPrefab { get => chainsPrefab; set => chainsPrefab = value; }
        public ParticleSystem SnowflakesVfxPrefab { get => snowflakesVfxPrefab; set => snowflakesVfxPrefab = value; }
        public GameObject InvertJumpVfxPrefab { get => invertJumpVfxPrefab; set => invertJumpVfxPrefab = value; }
        public Color ColdTintColor { get => coldTintColor; set => coldTintColor = value; }

        private void Awake()
        {
            if (modifiers == null)
            {
                modifiers = GetComponent<PlayerModifiers>();
            }

            ResolvePoints();
            CacheCharacterRenderers();
        }

        private void OnEnable()
        {
            if (modifiers != null)
            {
                modifiers.Changed += Refresh;
            }
            Refresh();
        }

        private void OnDisable()
        {
            if (modifiers != null)
            {
                modifiers.Changed -= Refresh;
            }
            ClearAll();
        }

        public void Refresh()
        {
            if (modifiers == null)
            {
                return;
            }

            // 1. SuperJump: Boots at feet
            bool hasSuperJump = modifiers.JumpForceMultiplier > 1.05f;
            SetBootsVisible(hasSuperJump);

            // 2. NoJump & BlockRivalJump: Chains at feet
            bool hasJumpBlocked = modifiers.IsJumpBlocked;
            SetChainsVisible(hasJumpBlocked);

            // 3. Slow & FreezeRival: Tint character sprites to light blue + snowflakes on body
            bool hasColdEffect = modifiers.IsFrozen || modifiers.TimeScale < 0.95f;
            SetColdEffectVisible(hasColdEffect);

            // 4. InvertJump: Invert jump VFX
            bool hasInvertJump = modifiers.IsJumpDirectionInverted;
            SetInvertJumpVfxVisible(hasInvertJump);
        }

        public void SetBootsVisible(bool visible)
        {
            if (visible)
            {
                if (_bootsInstance == null && (bootsPrefab != null || bootsSprite != null))
                {
                    _bootsInstance = SpawnFeetVisual("BootsVisual", bootsPrefab, bootsSprite, bootsOffset, bootsScale);
                }
            }
            else
            {
                if (_bootsInstance != null)
                {
                    Destroy(_bootsInstance);
                    _bootsInstance = null;
                }
            }
        }

        public void SetChainsVisible(bool visible)
        {
            if (visible)
            {
                if (_chainsInstance == null && (chainsPrefab != null || chainsSprite != null))
                {
                    _chainsInstance = SpawnFeetVisual("ChainsVisual", chainsPrefab, chainsSprite, chainsOffset, chainsScale);
                }
            }
            else
            {
                if (_chainsInstance != null)
                {
                    Destroy(_chainsInstance);
                    _chainsInstance = null;
                }
            }
        }

        public void SetColdEffectVisible(bool visible)
        {
            if (visible == _isColdActive)
            {
                return;
            }

            _isColdActive = visible;

            // Tint character sprites to light blue
            foreach (var kvp in _originalColors)
            {
                SpriteRenderer renderer = kvp.Key;
                if (renderer != null)
                {
                    renderer.color = visible ? coldTintColor : kvp.Value;
                }
            }

            // Snowflakes particle system over body
            if (visible)
            {
                if (_snowflakesInstance == null && snowflakesVfxPrefab != null)
                {
                    Transform parent = bodyPoint != null ? bodyPoint : transform;
                    _snowflakesInstance = Instantiate(snowflakesVfxPrefab, parent);
                    _snowflakesInstance.transform.localPosition = snowflakesOffset;
                    _snowflakesInstance.transform.localRotation = Quaternion.identity;
                    _snowflakesInstance.Play();
                }
            }
            else
            {
                if (_snowflakesInstance != null)
                {
                    Destroy(_snowflakesInstance.gameObject);
                    _snowflakesInstance = null;
                }
            }
        }

        public void SetInvertJumpVfxVisible(bool visible)
        {
            if (visible)
            {
                if (_invertJumpInstance == null && invertJumpVfxPrefab != null)
                {
                    Transform parent = headPoint != null ? headPoint : (bodyPoint != null ? bodyPoint : transform);
                    _invertJumpInstance = Instantiate(invertJumpVfxPrefab, parent);
                    _invertJumpInstance.transform.localPosition = invertJumpOffset;
                    _invertJumpInstance.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                if (_invertJumpInstance != null)
                {
                    Destroy(_invertJumpInstance);
                    _invertJumpInstance = null;
                }
            }
        }

        private GameObject SpawnFeetVisual(string objectName, GameObject prefab, Sprite sprite, Vector3 offset, Vector3 scale)
        {
            Transform parent = feetPoint != null ? feetPoint : transform;
            GameObject instance;

            if (prefab != null)
            {
                instance = Instantiate(prefab, parent);
            }
            else
            {
                instance = new GameObject(objectName);
                instance.transform.SetParent(parent, false);

                SpriteRenderer sr = instance.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                if (!string.IsNullOrEmpty(feetSortingLayerName))
                {
                    sr.sortingLayerName = feetSortingLayerName;
                }
                sr.sortingOrder = feetSortingOrder;
            }

            instance.transform.localPosition = offset;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = scale;
            return instance;
        }

        public void ClearAll()
        {
            SetBootsVisible(false);
            SetChainsVisible(false);
            SetColdEffectVisible(false);
            SetInvertJumpVfxVisible(false);
        }

        private void ResolvePoints()
        {
            if (feetPoint == null)
            {
                Transform gc = transform.Find("Body/GroundCheck");
                feetPoint = gc != null ? gc : transform.Find("GroundCheck");
            }

            if (bodyPoint == null)
            {
                bodyPoint = transform.Find("Body");
            }

            if (headPoint == null)
            {
                headPoint = transform.Find("Head");
            }
        }

        private void CacheCharacterRenderers()
        {
            _originalColors.Clear();

            if (characterRenderers == null || characterRenderers.Length == 0)
            {
                characterRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            }

            for (int i = 0; i < characterRenderers.Length; i++)
            {
                SpriteRenderer sr = characterRenderers[i];
                if (sr != null && !_originalColors.ContainsKey(sr))
                {
                    _originalColors[sr] = sr.color;
                }
            }
        }
    }
}
