using UnityEngine;

public class PrintedText : MonoBehaviour
{
    [SerializeField] private float SelfDestroyTime = 2.0f;
    private void Start()
    {
        Invoke(nameof(SelfDestroy), SelfDestroyTime);
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
