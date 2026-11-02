using TMPro;
using UnityEngine;
using BaskgayBall.Input;

public class GoalUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [Tooltip("Frases aleatorias que se mostraran cuando se anote un punto.")]
    [SerializeField] private string[] goalPhrases = new string[]
    {
        "GOAL!",
        "SCORE!",
        "NICE SHOT!",
        "SWISH!",
        "BOOM!"
    };

    private void OnEnable()
    {
        GlobalEvents.OnGameScoreChange += GlobalEvents_OnGameScoreChange;
        GlobalEvents.OnStartRound += GlobalEvents_OnStartRound;
    }

    private void OnDisable()
    {
        GlobalEvents.OnGameScoreChange -= GlobalEvents_OnGameScoreChange;
        GlobalEvents.OnStartRound -= GlobalEvents_OnStartRound;
    }

    private void GlobalEvents_OnStartRound()
    {
        if (text != null)
        {
            text.gameObject.SetActive(false);
        }
    }

    private void GlobalEvents_OnGameScoreChange(int arg1, int arg2, EPlayerSide scoringPlayer)
    {
        // Si no es un jugador que anoto (por ej. reinicio de partida en None), no mostramos nada
        if (scoringPlayer == EPlayerSide.None) return;

        if (text == null) return;

        // Seleccionar una frase aleatoria
        if (goalPhrases != null && goalPhrases.Length > 0)
        {
            int randomIndex = Random.Range(0, goalPhrases.Length);
            text.text = goalPhrases[randomIndex];
        }

        if (GameManager.Instance?.CosmeticsMng?.skinDataBase != null)
        {
            text.color = GameManager.Instance.CosmeticsMng.skinDataBase.GetPlayerColor(scoringPlayer);
        }

        text.gameObject.SetActive(true);
    }
}
