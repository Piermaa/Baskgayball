using BaskgayBall.Input;
using BaskgayBall.Player;
using UnityEngine;

namespace BaskgayBall.Player
{
    public class PlayerController : MonoBehaviour, ISided
    {
        public bool FacesRight => facesRight;

        [SerializeField] private EPlayerSide side;
        [SerializeField] private ArmController arm;
        [SerializeField] private BallGrabHandler hand;

        [SerializeField] private bool facesRight = true;

        [SerializeField] private BodyMovement bodyMovement;

        public EPlayerSide Side => side;

        private void Awake()
        {
            BasketHelpers.ApplyFacing(transform, facesRight);
        }

        private void OnDrawGizmosSelected()
        {
            BasketHelpers.ApplyFacing(transform, facesRight);
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

        private void HandlePressStart(EPlayerSide pressedSide)
        {
            if (pressedSide != side) return;

            bodyMovement.Jump();
            bodyMovement.SetHolding(true);
            arm.BeginCharge();
        }

        private void HandlePressEnd(EPlayerSide pressedSide)
        {
            if (pressedSide != side) return;

            bodyMovement.SetHolding(false);
            Vector2 throwVelocity = arm.EndChargeAndGetThrowVelocity();

            if (hand.IsHoldingBall)
            {
                hand.Throw(throwVelocity);
            }
        }
    }
}