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
        private bool finishedMatch;
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
            GlobalEvents.OnFinishMatch += GlobalEvents_OnFinishMatch;
            GlobalEvents.OnStartMatch += GlobalEvents_OnStartMatch;
        }

        private void GlobalEvents_OnFinishMatch()
        {
            finishedMatch = true;
        }

        private void GlobalEvents_OnStartMatch()
        {
            finishedMatch = false;  
        }

        private void OnDisable()
        {
            var manager = TouchInputManager.Instance;
            if (manager == null) return;

            manager.OnPlayerPressStart -= HandlePressStart;
            manager.OnPlayerPressEnd -= HandlePressEnd;
            GlobalEvents.OnFinishMatch -= GlobalEvents_OnFinishMatch;
            GlobalEvents.OnStartMatch -= GlobalEvents_OnStartMatch;
        }

        private void HandlePressStart(EPlayerSide pressedSide)
        {
            if (pressedSide != side) return;
            if (finishedMatch) return;

            bodyMovement.Jump();
            bodyMovement.SetHolding(true);
            arm.BeginCharge();
        }

        private void HandlePressEnd(EPlayerSide pressedSide)
        {
            if (pressedSide != side) return;
            if (finishedMatch) return;

            bodyMovement.SetHolding(false);
            Vector2 throwVelocity = arm.EndChargeAndGetThrowVelocity();

            if (hand.IsHoldingBall)
            {
                hand.Throw(throwVelocity);
            }
        }
    }
}