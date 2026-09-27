using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string gameScene = "RandomBasket";
    public string menuScene = "MainMenu";
    public void StartGame()
    {
        LoadScene(gameScene);
    }

    public void ReturnToMainMenu()
    {
        LoadScene(menuScene);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
