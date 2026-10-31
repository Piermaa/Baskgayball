using BaskgayBall.PowerUps;
using UnityEngine;

namespace BaskgayBall.Player
{
    public class PlayerFreezeHandler : MonoBehaviour
    {
        [SerializeField] private PlayerModifiers modifiers;
        [SerializeField] private Rigidbody2D[] bodies;

        private RigidbodyConstraints2D[] _originalConstraints;
        private bool _isFrozen;

        private void Awake()
        {
            _originalConstraints = new RigidbodyConstraints2D[bodies.Length];
        }

        private void OnEnable()
        {
            modifiers.Changed += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            modifiers.Changed -= Refresh;
            SetFrozen(false);
        }

        private void Refresh()
        {
            SetFrozen(modifiers.IsFrozen);
        }

        private void SetFrozen(bool frozen)
        {
            if (frozen == _isFrozen)
            {
                return;
            }

            _isFrozen = frozen;

            for (int i = 0; i < bodies.Length; i++)
            {
                Rigidbody2D body = bodies[i];
                if (body == null)
                {
                    continue;
                }

                if (frozen)
                {
                    _originalConstraints[i] = body.constraints;
                    body.linearVelocity = Vector2.zero;
                    body.angularVelocity = 0f;
                    body.constraints = RigidbodyConstraints2D.FreezeAll;
                }
                else
                {
                    body.constraints = _originalConstraints[i];
                    body.WakeUp();
                }
            }
        }
    }
}