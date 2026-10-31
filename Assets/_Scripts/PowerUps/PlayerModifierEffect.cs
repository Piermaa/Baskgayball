using BaskgayBall.Player;
using System.Collections.Generic;
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
            IReadOnlyList<PlayerModifiers> team = PlayerModifiers.GetTeam(context.TargetSide);
            for (int i = 0; i < team.Count; i++)
            {
                team[i].Add(context.Definition, kind, value);
            }
        }
        public override void Remove(in PowerUpContext context)
        {
            IReadOnlyList<PlayerModifiers> team = PlayerModifiers.GetTeam(context.TargetSide);
            for (int i = 0; i < team.Count; i++)
            {
                team[i].RemoveAll(context.Definition);
            }
        }
    }
}