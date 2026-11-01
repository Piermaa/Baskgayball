using UnityEngine;

namespace BaskgayBall.Stage
{
    [CreateAssetMenu(fileName = "SurfaceProfile", menuName = "BaskgayBall/Surface Profile")]
    public class SurfaceProfile : ScriptableObject
    {
        [Header("Physics")]
        [SerializeField] private PhysicsMaterial2D material;

        [Header("Ragdoll multipliers")]
        [SerializeField] private float uprightSpringMultiplier = 1f;
        [SerializeField] private float uprightDampingMultiplier = 1f;
        [SerializeField] private float landingWobbleMultiplier = 1f;
        [SerializeField] private float groundHorizontalDrag = 0f;

        public PhysicsMaterial2D Material => material;
        public float UprightSpringMultiplier => uprightSpringMultiplier;
        public float UprightDampingMultiplier => uprightDampingMultiplier;
        public float LandingWobbleMultiplier => landingWobbleMultiplier;
        public float GroundHorizontalDrag => groundHorizontalDrag;
    }
}