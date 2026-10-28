using UnityEngine;

namespace BaskgayBall.PowerUps
{
    [CreateAssetMenu(fileName = "Effect_HoopShield", menuName = "BaskgayBall/PowerUps/Effects/Hoop Shield")]
    public class HoopShieldEffect : PowerUpEffect
    {
        public override void Apply(in PowerUpContext context)
        {
            if (HoopShield.TryGet(context.TargetSide, out HoopShield shield))
            {
                shield.Acquire();
            }
        }

        public override void Remove(in PowerUpContext context)
        {
            if (HoopShield.TryGet(context.TargetSide, out HoopShield shield))
            {
                shield.Release();
            }
        }
    }
}