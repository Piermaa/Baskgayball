using BaskgayBall.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public class PowerUpEffectRunner : MonoBehaviour
    {
        private class ActiveEffect
        {
            public PowerUpContext Context;
            public Coroutine Routine;
        }

        private readonly Dictionary<(PowerUpDefinition, EPlayerSide), ActiveEffect> _active =
            new Dictionary<(PowerUpDefinition, EPlayerSide), ActiveEffect>();

        public void Activate(PowerUpDefinition definition, EPlayerSide collectorSide)
        {
            EPlayerSide targetSide = definition.Target == EPowerUpTarget.Collector
                ? collectorSide
                : GetOpposite(collectorSide);

            var key = (definition, targetSide);

            if (_active.TryGetValue(key, out ActiveEffect running))
            {
                StopCoroutine(running.Routine);
                running.Routine = StartCoroutine(ExpireRoutine(key, definition.Duration));
                return;
            }

            var context = new PowerUpContext(definition, collectorSide, targetSide);
            definition.Effect.Apply(context);

            var entry = new ActiveEffect { Context = context };
            entry.Routine = StartCoroutine(ExpireRoutine(key, definition.Duration));
            _active[key] = entry;
        }

        public void ClearAll()
        {
            var snapshot = new List<ActiveEffect>(_active.Values);
            StopAllCoroutines();
            _active.Clear();

            for (int i = 0; i < snapshot.Count; i++)
            {
                PowerUpContext context = snapshot[i].Context;
                context.Definition.Effect.Remove(context);
            }
        }

        private IEnumerator ExpireRoutine((PowerUpDefinition, EPlayerSide) key, float duration)
        {
            yield return new WaitForSeconds(duration);

            ActiveEffect entry = _active[key];
            _active.Remove(key);
            entry.Context.Definition.Effect.Remove(entry.Context);
        }

        private static EPlayerSide GetOpposite(EPlayerSide side)
        {
            return side == EPlayerSide.Player1 ? EPlayerSide.Player2 : EPlayerSide.Player1;
        }
    }
}