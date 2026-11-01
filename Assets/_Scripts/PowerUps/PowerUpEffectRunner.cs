using System;
using System.Collections;
using System.Collections.Generic;
using BaskgayBall.Input;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public sealed class ActivePowerUp
    {
        internal Coroutine Routine;

        public PowerUpContext Context { get; }
        public float EndTime { get; internal set; }

        public PowerUpDefinition Definition => Context.Definition;
        public float RemainingTime => Mathf.Max(0f, EndTime - Time.time);

        public float NormalizedRemaining =>
            Definition.Duration > 0f ? Mathf.Clamp01(RemainingTime / Definition.Duration) : 0f;

        internal ActivePowerUp(PowerUpContext context, float endTime)
        {
            Context = context;
            EndTime = endTime;
        }
    }

    public class PowerUpEffectRunner : MonoBehaviour
    {
        private readonly Dictionary<(PowerUpDefinition, EPlayerSide), ActivePowerUp> _active =
            new Dictionary<(PowerUpDefinition, EPlayerSide), ActivePowerUp>();

        public IReadOnlyCollection<ActivePowerUp> Active => _active.Values;

        public event Action<ActivePowerUp> EffectStarted;
        public event Action<ActivePowerUp> EffectEnded;

        public void Activate(PowerUpDefinition definition, EPlayerSide collectorSide)
        {
            EPlayerSide targetSide = definition.Target == EPowerUpTarget.Collector
                ? collectorSide
                : GetOpposite(collectorSide);

            var key = (definition, targetSide);

            if (_active.TryGetValue(key, out ActivePowerUp running))
            {
                StopCoroutine(running.Routine);
                running.EndTime = Time.time + definition.Duration;
                running.Routine = StartCoroutine(ExpireRoutine(key, definition.Duration));
                return;
            }

            var context = new PowerUpContext(definition, collectorSide, targetSide);
            definition.Effect.Apply(context);

            var entry = new ActivePowerUp(context, Time.time + definition.Duration);
            entry.Routine = StartCoroutine(ExpireRoutine(key, definition.Duration));
            _active[key] = entry;

            EffectStarted?.Invoke(entry);
        }

        public void ClearAll()
        {
            var snapshot = new List<ActivePowerUp>(_active.Values);
            StopAllCoroutines();
            _active.Clear();

            for (int i = 0; i < snapshot.Count; i++)
            {
                PowerUpContext context = snapshot[i].Context;
                context.Definition.Effect.Remove(context);
                EffectEnded?.Invoke(snapshot[i]);
            }
        }

        private IEnumerator ExpireRoutine((PowerUpDefinition, EPlayerSide) key, float duration)
        {
            yield return new WaitForSeconds(duration);

            ActivePowerUp entry = _active[key];
            _active.Remove(key);
            entry.Definition.Effect.Remove(entry.Context);
            EffectEnded?.Invoke(entry);
        }

        private static EPlayerSide GetOpposite(EPlayerSide side)
        {
            return side == EPlayerSide.Player1 ? EPlayerSide.Player2 : EPlayerSide.Player1;
        }
    }
}