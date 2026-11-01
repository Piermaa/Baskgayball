using BaskgayBall.Core;
using Unity.Collections;
using UnityEngine;

namespace BaskgayBall.Stage
{
    public class SurfaceController : MonoBehaviour
    {
        [SerializeField] private SurfaceProfile[] profiles;
        [SerializeField] private Collider2D[] floorColliders;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SurfaceProfile currentProfile;
        public SurfaceProfile Current 
        { 
            get => currentProfile; 
            private set 
            { currentProfile = value; } 
        }

        public void Roll()
        {
            Current = BasketHelpers.GetRandomElement(profiles);

            for (int i = 0; i < floorColliders.Length; i++)
            {
                floorColliders[i].sharedMaterial = Current.Material;
            }

            Color tempColor = Current.GroundColor;
            tempColor.a = 0.1f;
            spriteRenderer.color = tempColor;
        }
    }
}