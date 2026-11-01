using BaskgayBall.Core;
using Unity.Collections;
using UnityEngine;

namespace BaskgayBall.Stage
{
    public class SurfaceController : MonoBehaviour
    {
        [SerializeField] private SurfaceProfile[] profiles;
        [SerializeField] private Collider2D[] floorColliders;

        public SurfaceProfile Current 
        { 
            get => currentProfile; 
            private set 
            { currentProfile = value; } 
        }
        [SerializeField] private SurfaceProfile currentProfile;



        public void Roll()
        {
            Current = BasketHelpers.GetRandomElement(profiles);

            for (int i = 0; i < floorColliders.Length; i++)
            {
                floorColliders[i].sharedMaterial = Current.Material;
            }
        }
    }
}