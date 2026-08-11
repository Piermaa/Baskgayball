using BaskgayBall.Input;
using BaskgayBall.Player;
using UnityEngine;

namespace BaskgayBall.Player
{

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerSide side;
        [SerializeField] private ArmController arm;
        [SerializeField] private BallGrabHandler hand;

      
        [SerializeField] private bool facesRight = true;

        [SerializeField] private MonoBehaviour bodyMovement;

        private void Awake()
        {
            ApplyFacing();
        }

        private void ApplyFacing()
        {
            float xSign = facesRight ? 1f : -1f;
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(Mathf.Abs(scale.x) * xSign, scale.y, scale.z);

            arm.SetFacing(facesRight);
        }

        private void OnEnable()
        {
            var manager = TouchInputManager.Instance;
            if (manager == null) return;

            manager.OnPlayerPressStart += HandlePressStart;
            manager.OnPlayerPressEnd += HandlePressEnd;
        }

        private void OnDisable()
        {
            var manager = TouchInputManager.Instance;
            if (manager == null) return;

            manager.OnPlayerPressStart -= HandlePressStart;
            manager.OnPlayerPressEnd -= HandlePressEnd;
        }

        private void HandlePressStart(PlayerSide pressedSide)
        {
            if (pressedSide != side) return;

            arm.BeginCharge();
            // TODO: llamar al salto del Body acá cuando el script de movimiento exista.
        }

        private void HandlePressEnd(PlayerSide pressedSide)
        {
            if (pressedSide != side) return;

            Vector2 throwVelocity = arm.EndChargeAndGetThrowVelocity();

            if (hand.IsHoldingBall)
            {
                hand.Throw(throwVelocity);
            }
        }
    }
}