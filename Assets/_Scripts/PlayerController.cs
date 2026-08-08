using UnityEngine;
using BaskgayBall.Input;

namespace BaskgayBall.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerSide side;
        [SerializeField] private ArmController arm;
        [SerializeField] private BallGrabHandler hand;

        [SerializeField] private MonoBehaviour bodyMovement;

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
            // TODO: JUMP
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
