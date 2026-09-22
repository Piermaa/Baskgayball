using UnityEngine;
public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    public GameObject Player1Hoop => player1Hoop;
    public GameObject Player2Hoop => player2Hoop;

    [Header("Ball")]
    [SerializeField] private GameObject[] ballPrefabs;
    [SerializeField] private Transform ballSpawnPosition;

    [Header("Hoops")]
    [SerializeField] private GameObject player1Hoop;
    [SerializeField] private GameObject player2Hoop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        Instantiate(BasketHelpers.GetRandomElement(ballPrefabs), ballSpawnPosition);
    }
}
