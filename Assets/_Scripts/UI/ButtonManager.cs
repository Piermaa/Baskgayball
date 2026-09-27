using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Icon - Setup")]
    [SerializeField] private Sprite buttonSprite = null;
    [SerializeField] private Color colorTint = Color.white;
    [Header("Icon - Internal")]
    [SerializeField] private Image image = null;

    private void Awake()
    {
        RefreshButtonStyle();
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
