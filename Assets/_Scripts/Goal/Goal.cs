using BaskgayBall.Input;
using BaskgayBall.Player;
using UnityEngine;

namespace BaskgayBall.Goal
{
    public class Goal : MonoBehaviour, ISided
    {
        public EPlayerSide Side => ownerSide;

        [SerializeField] private EPlayerSide ownerSide;
        [SerializeField] private bool facesRight;
        private void Awake()
        {
            BasketHelpers.ApplyFacing(transform, facesRight);

        }

        private void OnDrawGizmosSelected()
        {
            BasketHelpers.ApplyFacing(transform, facesRight);
        }
    }
}
