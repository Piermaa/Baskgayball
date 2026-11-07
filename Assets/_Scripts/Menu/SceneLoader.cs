using BaskgayBall.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private string gameScene = "Gameplay";
    private string menuScene = "MainMenu";

    public void StartGame()
    {
        AudioManager.Instance?.PlayButtonPress();
        bool fromMenu = SceneManager.GetActiveScene().name == menuScene;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetStartedFromMenu(fromMenu);
        }
        LoadScene(gameScene);
    }

    public void ReturnToMainMenu()
    {
        AudioManager.Instance?.PlayButtonPress();
        LoadScene(menuScene);
    }

    public void LoadScene(string sceneName)
    {
        AudioManager.Instance?.PlayButtonPress();
        SceneManager.LoadScene(sceneName);
    }
}
