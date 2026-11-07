using BaskgayBall.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private const string SkinDatabaseResourcePath = "SkinDatabase";

    [Header("Display Settings")]
    [Tooltip("If true, only shows the tutorial when the match was started from the main menu.")]
    [SerializeField] private bool onlyShowWhenStartedFromMenu = true;
    [Tooltip("If true, allows displaying the tutorial when playing the Gameplay scene directly in the Unity Editor.")]
    [SerializeField] private bool showWhenDirectPlayInEditor = true;

    [Header("Configuration")]
    [SerializeField] private SkinDataBase skinDatabase;
    [FormerlySerializedAs("opacity")]
    [SerializeField, Range(0f, 1f)] private float idleOpacity = 0.18f;
    [SerializeField, Range(0f, 1f)] private float pressedOpacity = 0.33f;
    [SerializeField, TextArea] private string tutorialMessage = "Hold to jump and charge the shot and release to throw";

    [Header("Player 1 (Right Half)")]
    [SerializeField] private Image player1Rectangle;
    [SerializeField] private TextMeshProUGUI player1Text;

    [Header("Player 2 (Left Half)")]
    [SerializeField] private Image player2Rectangle;
    [SerializeField] private TextMeshProUGUI player2Text;

    private TouchInputManager _subscribedInputManager;
    private bool _player1HasPressed;
    private bool _player2HasPressed;

    private void Awake()
    {
        if (!ShouldShowTutorial())
        {
            SetTutorialActive(false);
            return;
        }

        ApplyVisuals();
    }

    private void OnEnable()
    {
        if (!ShouldShowTutorial())
        {
            SetTutorialActive(false);
            return;
        }

        TrySubscribeInput();
    }

    private void Start()
    {
        if (!ShouldShowTutorial())
        {
            SetTutorialActive(false);
            return;
        }

        ApplyVisuals();
        TrySubscribeInput();
    }

    private void Update()
    {
        if (_subscribedInputManager == null)
        {
            TrySubscribeInput();
        }
    }

    private void OnDisable()
    {
        UnsubscribeInput();
    }

    private void OnValidate()
    {
        ApplyVisuals();
    }

    public bool ShouldShowTutorial()
    {
        if (!onlyShowWhenStartedFromMenu) return true;
        if (GameManager.Instance == null) return true;

#if UNITY_EDITOR
        if (GameManager.Instance.IsDirectPlayInEditor && showWhenDirectPlayInEditor)
        {
            return true;
        }
#endif

        return GameManager.Instance.StartedFromMenu;
    }

    private void SetTutorialActive(bool active)
    {
        if (player1Rectangle != null) player1Rectangle.gameObject.SetActive(active);
        if (player2Rectangle != null) player2Rectangle.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }

    private void ApplyVisuals()
    {
        // load to use on the editor, but in the build, it will use the one from the GameManager
        SkinDataBase db = skinDatabase;
        if (db == null && GameManager.Instance?.CosmeticsMng != null)
        {
            db = GameManager.Instance.CosmeticsMng.skinDataBase;
        }
#if UNITY_EDITOR
        if (db == null)
        {
            db = Resources.Load<SkinDataBase>(SkinDatabaseResourcePath);
        }
#endif

        float player1Alpha = _player1HasPressed ? pressedOpacity : idleOpacity;
        float player2Alpha = _player2HasPressed ? pressedOpacity : idleOpacity;

        ApplySideVisuals(player1Rectangle, player1Text, EPlayerSide.Player1, db, player1Alpha);
        ApplySideVisuals(player2Rectangle, player2Text, EPlayerSide.Player2, db, player2Alpha);
    }

    private void ApplySideVisuals(Image rectangleImage, TextMeshProUGUI textComponent, EPlayerSide side, SkinDataBase db, float alpha)
    {
        if (rectangleImage != null)
        {
            Color color = db != null ? db.GetPlayerColor(side) : Color.white;
            color.a = alpha;
            rectangleImage.color = color;
            rectangleImage.raycastTarget = false;
        }

        if (textComponent != null)
        {
            if (!string.IsNullOrEmpty(tutorialMessage))
            {
                textComponent.text = tutorialMessage;
            }
            textComponent.raycastTarget = false;
        }
    }

    private static void SetRectangleAlpha(Image rectangleImage, float alpha)
    {
        if (rectangleImage == null) return;

        Color color = rectangleImage.color;
        color.a = alpha;
        rectangleImage.color = color;
    }

    private void TrySubscribeInput()
    {
        if (_subscribedInputManager != null) return;

        TouchInputManager manager = TouchInputManager.Instance;
        if (manager != null)
        {
            _subscribedInputManager = manager;
            if (manager.IsSideActive(EPlayerSide.Player1))
            {
                HandlePressStart(EPlayerSide.Player1);
            }
            if (manager.IsSideActive(EPlayerSide.Player2))
            {
                HandlePressStart(EPlayerSide.Player2);
            }
            manager.OnPlayerPressStart += HandlePressStart;
            manager.OnPlayerPressEnd += HandlePressEnd;
        }
    }

    private void UnsubscribeInput()
    {
        if (_subscribedInputManager != null)
        {
            _subscribedInputManager.OnPlayerPressStart -= HandlePressStart;
            _subscribedInputManager.OnPlayerPressEnd -= HandlePressEnd;
            _subscribedInputManager = null;
        }
    }

    private void HandlePressStart(EPlayerSide side)
    {
        switch (side)
        {
            case EPlayerSide.Player1:
                _player1HasPressed = true;
                SetRectangleAlpha(player1Rectangle, pressedOpacity);
                break;
            case EPlayerSide.Player2:
                _player2HasPressed = true;
                SetRectangleAlpha(player2Rectangle, pressedOpacity);
                break;
        }
    }

    private void HandlePressEnd(EPlayerSide side)
    {
        switch (side)
        {
            case EPlayerSide.Player1:
                if (_player1HasPressed && player1Rectangle != null)
                {
                    player1Rectangle.gameObject.SetActive(false);
                }
                break;
            case EPlayerSide.Player2:
                if (_player2HasPressed && player2Rectangle != null)
                {
                    player2Rectangle.gameObject.SetActive(false);
                }
                break;
        }
    }
}
