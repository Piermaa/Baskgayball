using BaskgayBall.Input;
using BaskgayBall.Player;
using UnityEngine;

namespace BaskgayBall.Goal
{
    public class Goal : MonoBehaviour, ISided
    {
        public PlayerSide Side => ownerSide;

        [SerializeField] private PlayerSide ownerSide;
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
