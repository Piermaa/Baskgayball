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
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private SpriteRenderer iconBgRenderer;

        private bool _resolved;

        public PowerUpDefinition Definition { get; private set; }

        public event Action<PowerUpPickup, EPlayerSide> Collected;
        public event Action<PowerUpPickup> Expired;

        public void Initialize(PowerUpDefinition definition, float lifetime)
        {
            Definition = definition;
            iconRenderer.sprite = definition.Icon;
            iconBgRenderer.color = definition.Polarity == EPowerUpPolarity.Positive ? positiveColor : negativeColor;
            StartCoroutine(LifetimeRoutine(lifetime));
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
            if (_resolved)
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
            Collected?.Invoke(this, modifiers.Side);
        }
    }
}