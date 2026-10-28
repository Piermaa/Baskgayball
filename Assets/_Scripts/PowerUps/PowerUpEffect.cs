using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public abstract class PowerUpEffect : ScriptableObject
    {
        public abstract void Apply(in PowerUpContext context);
        public abstract void Remove(in PowerUpContext context);
    }
}