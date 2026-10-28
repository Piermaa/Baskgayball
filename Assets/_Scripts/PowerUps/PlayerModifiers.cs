using BaskgayBall.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public class PlayerModifiers : MonoBehaviour
    {
        private static readonly Dictionary<EPlayerSide, PlayerModifiers> Registry =
            new Dictionary<EPlayerSide, PlayerModifiers>();

        private struct Entry
        {
            public object Source;
            public EPlayerModifierKind Kind;
            public float Value;
        }

        private readonly List<Entry> _entries = new List<Entry>(4);

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

        public static bool TryGet(EPlayerSide side, out PlayerModifiers modifiers)
        {
            return Registry.TryGetValue(side, out modifiers);
        }

        private void Awake()
        {
            Side = GetComponentInParent<ISided>().Side;
        }

        private void OnEnable()
        {
            Registry[Side] = this;
        }

        private void OnDisable()
        {
            if (Registry.TryGetValue(Side, out PlayerModifiers current) && current == this)
            {
                Registry.Remove(Side);
            }
        }

        public void Add(object source, EPlayerModifierKind kind, float value = 1f)
        {
            _entries.Add(new Entry { Source = source, Kind = kind, Value = value });
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