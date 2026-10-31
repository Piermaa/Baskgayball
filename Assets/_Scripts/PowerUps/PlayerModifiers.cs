using System;
using System.Collections.Generic;
using BaskgayBall.Input;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public class PlayerModifiers : MonoBehaviour
    {
        private static readonly Dictionary<EPlayerSide, List<PlayerModifiers>> Registry =
            new Dictionary<EPlayerSide, List<PlayerModifiers>>();

        private static readonly List<PlayerModifiers> EmptyTeam = new List<PlayerModifiers>();

        [Serializable]
        private struct ModifierEntry
        {
            [NonSerialized] public object Source;
            public string SourceName;
            public EPlayerModifierKind Kind;
            public float Value;
        }

        [SerializeField] private List<ModifierEntry> _entries = new List<ModifierEntry>(4);

        public EPlayerSide Side { get; private set; }

        public event Action Changed;

        public bool IsJumpBlocked => Has(EPlayerModifierKind.JumpBlocked);
        public bool IsFrozen => Has(EPlayerModifierKind.Frozen);
        public bool IsJumpDirectionInverted => Has(EPlayerModifierKind.InvertedJumpDirection);
        public float JumpForceMultiplier => Product(EPlayerModifierKind.JumpForceMultiplier);
        public float TimeScale => Product(EPlayerModifierKind.TimeScale);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Registry.Clear();
        }

        public static IReadOnlyList<PlayerModifiers> GetTeam(EPlayerSide side)
        {
            return Registry.TryGetValue(side, out List<PlayerModifiers> team) ? team : EmptyTeam;
        }

        private void Awake()
        {
            Side = GetComponentInParent<ISided>().Side;
        }

        private void OnEnable()
        {
            if (!Registry.TryGetValue(Side, out List<PlayerModifiers> team))
            {
                team = new List<PlayerModifiers>(2);
                Registry[Side] = team;
            }

            if (!team.Contains(this))
            {
                team.Add(this);
            }
        }

        private void OnDisable()
        {
            if (Registry.TryGetValue(Side, out List<PlayerModifiers> team))
            {
                team.Remove(this);
            }
        }

        public void Add(object source, EPlayerModifierKind kind, float value = 1f)
        {
            _entries.Add(new ModifierEntry
            {
                Source = source,
                SourceName = GetSourceName(source),
                Kind = kind,
                Value = value
            });

            Changed?.Invoke();
        }

        public void RemoveAll(object source)
        {
            int removed = _entries.RemoveAll(entry => entry.Source == source);
            if (removed > 0)
            {
                Changed?.Invoke();
            }
        }

        public void Clear()
        {
            if (_entries.Count == 0)
            {
                return;
            }

            _entries.Clear();
            Changed?.Invoke();
        }

        private static string GetSourceName(object source)
        {
            if (source is UnityEngine.Object unityObject)
            {
                return unityObject.name;
            }

            return source != null ? source.ToString() : "null";
        }

        private bool Has(EPlayerModifierKind kind)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].Kind == kind)
                {
                    return true;
                }
            }

            return false;
        }

        private float Product(EPlayerModifierKind kind)
        {
            float result = 1f;
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].Kind == kind)
                {
                    result *= _entries[i].Value;
                }
            }

            return result;
        }
    }
}