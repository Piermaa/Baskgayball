using BaskgayBall.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaskgayBall.PowerUps
{
    public class PowerUpsManager : MonoBehaviour
    {
        [SerializeField] private PowerUpsConfig config;
        [SerializeField] private PowerUpEffectRunner effectRunner;
        [SerializeField] private Transform[] powerUpSpawnPositions;

        private Coroutine _spawnRoutine;
        private PowerUpPickup _activePickup;
        private bool _pickupResolved;
        private WaitUntil _waitForResolved;

        private void Awake()
        {
            _waitForResolved = new WaitUntil(() => _pickupResolved);
        }

        private void Start()
        {
            if (GameManager.Instance.GameModeMng.IsChaosMode)
            {
                StartPowerUpsRoutine();
            }
        }

        private void OnDisable()
        {
            StopPowerUpsRoutine();
        }

        public void ResetForNewRound()
        {
            StopPowerUpsRoutine();
            effectRunner.ClearAll();

            if (GameManager.Instance.GameModeMng.IsChaosMode)
            {
                StartPowerUpsRoutine();
            }
        }

        private void StartPowerUpsRoutine()
        {
            StopPowerUpsRoutine();
            _spawnRoutine = StartCoroutine(SpawnLoop());
        }

        private void StopPowerUpsRoutine()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }

            if (_activePickup != null)
            {
                DetachAndDestroy(_activePickup);
            }
        }

        private IEnumerator SpawnLoop()
        {
            yield return new WaitForSeconds(config.InitialDelay);

            while (true)
            {
                if (!TrySpawnPickup())
                {
                    yield break;
                }

                yield return _waitForResolved;
                yield return new WaitForSeconds(Random.Range(config.DelayRange.x, config.DelayRange.y));
            }
        }

        private bool TrySpawnPickup()
        {
            PowerUpDefinition definition = PickDefinition();
            if (definition == null || powerUpSpawnPositions.Length == 0)
            {
                return false;
            }

            Transform spawnPoint = powerUpSpawnPositions[PickSpawnIndex()];
            PowerUpPickup pickup = Instantiate(config.PickupPrefab, spawnPoint.position, Quaternion.identity);
            pickup.Initialize(definition, config.PickupLifetime);
            pickup.Collected += HandleCollected;
            pickup.Expired += HandleExpired;

            _activePickup = pickup;
            _pickupResolved = false;
            return true;
        }

        private void HandleCollected(PowerUpPickup pickup, EPlayerSide collectorSide)
        {
            PowerUpDefinition definition = pickup.Definition;
            DetachAndDestroy(pickup);
            effectRunner.Activate(definition, collectorSide);
        }

        private void HandleExpired(PowerUpPickup pickup)
        {
            DetachAndDestroy(pickup);
        }

        private void DetachAndDestroy(PowerUpPickup pickup)
        {
            pickup.Collected -= HandleCollected;
            pickup.Expired -= HandleExpired;
            Destroy(pickup.gameObject);

            if (_activePickup == pickup)
            {
                _activePickup = null;
            }

            _pickupResolved = true;
        }

        private PowerUpDefinition PickDefinition()
        {
            EPowerUpPolarity wanted = Random.value < config.PositiveChance
                ? EPowerUpPolarity.Positive
                : EPowerUpPolarity.Negative;

            var pool = new List<PowerUpDefinition>();
            foreach (PowerUpDefinition definition in config.Definitions)
            {
                if (definition.Polarity == wanted)
                {
                    pool.Add(definition);
                }
            }

            if (pool.Count == 0)
            {
                foreach (PowerUpDefinition definition in config.Definitions)
                {
                    pool.Add(definition);
                }
            }

            return pool.Count == 0 ? null : pool[Random.Range(0, pool.Count)];
        }

        private int PickSpawnIndex()
        {
            IReadOnlyList<float> weights = config.SpawnPositionWeights;
            int count = powerUpSpawnPositions.Length;

            if (weights.Count != count)
            {
                return Random.Range(0, count);
            }

            float total = 0f;
            for (int i = 0; i < count; i++)
            {
                total += weights[i];
            }

            float roll = Random.value * total;
            for (int i = 0; i < count; i++)
            {
                roll -= weights[i];
                if (roll <= 0f)
                {
                    return i;
                }
            }

            return count - 1;
        }
    }
}