using UnityEngine;

public class MatchFinishDisplay : MonoBehaviour
{
    [SerializeField] private GameObject diplay = null;
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
        diplay.SetActive(true);
    }
}
