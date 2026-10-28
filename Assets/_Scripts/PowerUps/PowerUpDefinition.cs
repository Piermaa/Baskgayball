using UnityEngine;

namespace BaskgayBall.PowerUps
{
    [CreateAssetMenu(fileName = "PowerUp_", menuName = "BaskgayBall/PowerUps/Definition")]
    public class PowerUpDefinition : ScriptableObject
    {
        [SerializeField] private EPowerUpPolarity polarity;
        [SerializeField] private EPowerUpTarget target;
        [SerializeField] private PowerUpEffect effect;
        [SerializeField][Min(0.1f)] private float duration = 5f;
        [SerializeField] private Sprite icon;

        public EPowerUpPolarity Polarity => polarity;
        public EPowerUpTarget Target => target;
        public PowerUpEffect Effect => effect;
        public float Duration => duration;
        public Sprite Icon => icon;
    }
}