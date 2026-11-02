using UnityEngine;
using UnityEngine.UI;

namespace BaskgayBall.Audio
{
    [RequireComponent(typeof(Button))]
    public class UIButtonSound : MonoBehaviour
    {
        [Tooltip("Si esta activado, reproduce el sonido de boton global del AudioManager.")]
        [SerializeField] private bool useDefaultSound = true;

        [Tooltip("Sonido personalizado para este boton si useDefaultSound esta desactivado.")]
        [SerializeField] private SoundEffect customSound;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleClick);
            }
        }

        private void HandleClick()
        {
            if (useDefaultSound)
            {
                AudioManager.Instance?.PlayButtonPress();
            }
            else if (customSound != null)
            {
                AudioManager.Instance?.PlaySound(customSound);
            }
        }
    }
}

