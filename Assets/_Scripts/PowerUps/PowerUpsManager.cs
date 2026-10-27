using UnityEngine;

public class PowerUpsManager : MonoBehaviour
{
    [SerializeField] private Transform[] powerUpSpawnPositions;
    [SerializeField] private float initialPowerUpsDelay = 1.0f;
    [SerializeField] private Vector2 powerUpsDelayRange;
    private void Start()
    {
        if (GameManager.Instance.GameModeMng.IsChaosMode)
        {
            StartPowerUpsRoutine();
        }
    }

    private void StartPowerUpsRoutine()
    {
        
    }
}
