using BaskgayBall.Player;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    [CreateAssetMenu(fileName = "Effect_Modifier", menuName = "BaskgayBall/PowerUps/Effects/Player Modifier")]
    public class PlayerModifierEffect : PowerUpEffect
    {
        [SerializeField] private EPlayerModifierKind kind;
        [SerializeField] private float value = 1f;

        public override void Apply(in PowerUpContext context)
        {
            if (PlayerModifiers.TryGet(context.TargetSide, out PlayerModifiers modifiers))
            {
                modifiers.Add(context.Definition, kind, value);
            }
        }

        public override void Remove(in PowerUpContext context)
        {
            if (PlayerModifiers.TryGet(context.TargetSide, out PlayerModifiers modifiers))
            {
                modifiers.RemoveAll(context.Definition);
            }
        }
    }
}