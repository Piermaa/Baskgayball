using BaskgayBall.Audio;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Icon - Setup")]
    [SerializeField] private Sprite buttonSprite = null;
    [SerializeField] private Color colorTint = Color.white;
    [Header("Icon - Internal")]
    [SerializeField] private Image image = null;
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private Button button;
    private void Awake()
    {
        RefreshButtonStyle();
        button?.onClick.AddListener(PlayButtonSound);
    }
    private void PlayButtonSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            AudioManager.Instance?.PlayButtonPress();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        RefreshButtonStyle();
    }

#endif
    protected virtual void RefreshButtonStyle()
    {
        image.sprite = buttonSprite;
        image.color  = colorTint;
    }
}
