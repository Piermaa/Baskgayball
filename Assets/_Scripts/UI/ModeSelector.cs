using UnityEngine;

public class ModeSelector : MonoBehaviour
{
    public void SelectGamemode(bool isChaos)
    {
        GameManager.Instance.GameModeMng.SetMode(isChaos);
    }
}
