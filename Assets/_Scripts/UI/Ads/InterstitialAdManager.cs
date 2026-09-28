using UnityEngine;

public class InterstitialAdManager : MonoBehaviour
{
    [SerializeField] private GameObject adContainer;
    private void OnEnable()
    {
        GlobalEvents.OnFinishMatch += GlobalEvents_OnFinishMatch;
    }

    private void OnDisable()
    {
        GlobalEvents.OnFinishMatch -= GlobalEvents_OnFinishMatch;
    }
    private void GlobalEvents_OnFinishMatch()
    {
        adContainer.SetActive(true);
    }
}
