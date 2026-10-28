using System.Collections.Generic;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    [CreateAssetMenu(fileName = "PowerUpsConfig", menuName = "BaskgayBall/PowerUps/Config")]
    public class PowerUpsConfig : ScriptableObject
    {
        [SerializeField] private PowerUpPickup pickupPrefab;
        [SerializeField] private List<PowerUpDefinition> definitions = new List<PowerUpDefinition>();

        [Header("Timing")]
        [SerializeField][Min(0f)] private float initialDelay = 1f;
        [SerializeField] private Vector2 delayRange = new Vector2(3f, 6f);
        [SerializeField][Min(0.5f)] private float pickupLifetime = 5f;

        [Header("Odds")]
        [SerializeField][Range(0f, 1f)] private float positiveChance = 0.5f;
        [SerializeField] private float[] spawnPositionWeights = { 1f, 1f, 1f };

        public PowerUpPickup PickupPrefab => pickupPrefab;
        public IReadOnlyList<PowerUpDefinition> Definitions => definitions;
        public float InitialDelay => initialDelay;
        public Vector2 DelayRange => delayRange;
        public float PickupLifetime => pickupLifetime;
        public float PositiveChance => positiveChance;
        public IReadOnlyList<float> SpawnPositionWeights => spawnPositionWeights;
    }
}