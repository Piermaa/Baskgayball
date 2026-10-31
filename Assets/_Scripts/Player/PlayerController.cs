using BaskgayBall.Input;
using BaskgayBall.Player;
using BaskgayBall.PowerUps;
using UnityEngine;

namespace BaskgayBall.Player
{
    [RequireComponent(typeof(PlayerModifiers))]
    public class PlayerController : MonoBehaviour, ISided
    {
        public bool FacesRight => facesRight;

        [SerializeField] private EPlayerSide side;
        [SerializeField] private ArmController arm;
        [SerializeField] private BallGrabHandler hand;

        [SerializeField] private bool facesRight = true;

        [SerializeField] private BodyMovement bodyMovement;
        private PlayerModifiers _modifiers;
        private bool finishedMatch;
        public EPlayerSide Side => side;


        private void Awake()
        {
            _modifiers = GetComponent<PlayerModifiers>();
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
            if (_modifiers.IsFrozen) return;

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

            if (_modifiers.IsFrozen) return;

            if (hand.IsHoldingBall)
            {
                hand.Throw(throwVelocity);
            }
        }
    }
}