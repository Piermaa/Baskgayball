using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BaskgayBall.Input;
using BaskgayBall.Audio;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("El panel o ventana del menu de pausa que se mostrara al pausar.")]
    [SerializeField] private GameObject pausePanel;

    [Header("HUD Buttons (Opcional si los enlazas por OnClick)")]
    [Tooltip("Boton visible durante la partida para pausar el juego.")]
    [SerializeField] private Button pauseHUDButton;

    [Header("Menu Buttons (Opcional si los enlazas por OnClick)")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Config")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool disableTouchInputOnPause = true;

    public bool IsPaused => isPaused;
    private bool isPaused = false;

    private void Awake()
    {
        // Aseguramos que el panel inicie oculto
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Si se asignaron los botones en el inspector, suscribimos los eventos automaticamente
        if (pauseHUDButton != null) pauseHUDButton.onClick.AddListener(Pause);
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (restartButton != null) restartButton.onClick.AddListener(Restart);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        // Atajo util para probar en PC/Editor con la tecla Escape o P
        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }
#endif
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (isPaused) return;

        AudioManager.Instance?.PlayButtonPress();
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (pauseHUDButton != null)
        {
            pauseHUDButton.gameObject.SetActive(false);
        }

        if (disableTouchInputOnPause && TouchInputManager.Instance != null)
        {
            TouchInputManager.Instance.enabled = false;
        }
    }

    public void Resume()
    {
        if (!isPaused) return;

        AudioManager.Instance?.PlayButtonPress();
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseHUDButton != null)
        {
            pauseHUDButton.gameObject.SetActive(true);
        }

        if (disableTouchInputOnPause && TouchInputManager.Instance != null)
        {
            TouchInputManager.Instance.enabled = true;
        }
    }

    public void Restart()
    {
        AudioManager.Instance?.PlayButtonPress();
        // Restauramos el tiempo SIEMPRE antes de recargar
        Time.timeScale = 1f;

        if (disableTouchInputOnPause && TouchInputManager.Instance != null)
        {
            TouchInputManager.Instance.enabled = true;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void ReturnToMainMenu()
    {
        AudioManager.Instance?.PlayButtonPress();
        // Restauramos el tiempo SIEMPRE antes de salir al menu
        Time.timeScale = 1f;

        if (disableTouchInputOnPause && TouchInputManager.Instance != null)
        {
            TouchInputManager.Instance.enabled = true;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        // Seguridad: si la escena se descarga o se destruye estando pausado, devolver timeScale a 1
        if (isPaused)
        {
            Time.timeScale = 1f;
        }
    }
}
