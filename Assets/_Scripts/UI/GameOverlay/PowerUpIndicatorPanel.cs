using System.Collections.Generic;
using BaskgayBall.Input;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public class PowerUpIndicatorPanel : MonoBehaviour
    {
        [SerializeField] private EPlayerSide side;
        [SerializeField] private PowerUpEffectRunner effectRunner;
        [SerializeField] private PowerUpIndicatorItem itemPrefab;
        [SerializeField] private RectTransform container;

        private readonly Dictionary<ActivePowerUp, PowerUpIndicatorItem> _items =
            new Dictionary<ActivePowerUp, PowerUpIndicatorItem>();

        private readonly Stack<PowerUpIndicatorItem> _pool = new Stack<PowerUpIndicatorItem>();

        private void OnEnable()
        {
            effectRunner.EffectStarted += HandleEffectStarted;
            effectRunner.EffectEnded += HandleEffectEnded;

            foreach (ActivePowerUp active in effectRunner.Active)
            {
                HandleEffectStarted(active);
            }
        }

        private void OnDisable()
        {
            effectRunner.EffectStarted -= HandleEffectStarted;
            effectRunner.EffectEnded -= HandleEffectEnded;

            foreach (PowerUpIndicatorItem item in _items.Values)
            {
                Release(item);
            }

            _items.Clear();
        }

        private void HandleEffectStarted(ActivePowerUp active)
        {
            if (active.Context.TargetSide != side || _items.ContainsKey(active))
            {
                return;
            }

            PowerUpIndicatorItem item = Rent();
            item.Bind(active);
            _items[active] = item;
        }

        private void HandleEffectEnded(ActivePowerUp active)
        {
            if (_items.Remove(active, out PowerUpIndicatorItem item))
            {
                Release(item);
            }
        }

        private PowerUpIndicatorItem Rent()
        {
            PowerUpIndicatorItem item = _pool.Count > 0
                ? _pool.Pop()
                : Instantiate(itemPrefab, container);

            item.transform.SetAsLastSibling();
            return item;
        }

        private void Release(PowerUpIndicatorItem item)
        {
            item.Unbind();
            _pool.Push(item);
        }
    }
}