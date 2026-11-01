using TMPro;
using UnityEngine;

public class GoalUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        GlobalEvents.OnGameScoreChange += GlobalEvents_OnGameScoreChange;
        GlobalEvents.OnStartRound += GlobalEvents_OnStartRound;
    }

    private void GlobalEvents_OnStartRound()
    {
       text.gameObject.SetActive(false);
    }

    private void GlobalEvents_OnGameScoreChange(int arg1, int arg2, BaskgayBall.Input.EPlayerSide arg3)
    {
        text.color = GameManager.Instance.CosmeticsMng.skinDataBase.GetPlayerColor(arg3);
        text.gameObject.SetActive(true);
    }
}
