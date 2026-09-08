using UnityEngine;
public class RoundManager : MonoBehaviour
{
    [Header("Ball")]
    [SerializeField] private GameObject[] ballPrefabs;
    [SerializeField] private Transform ballSpawnPosition;

    private void Awake()
    {
        Instantiate(BasketHelpers.GetRandomElement(ballPrefabs), ballSpawnPosition);
    }
}
