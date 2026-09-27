using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string mainMenuSceneName = "RandomBasket";
    public void StartGame()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
