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

    private void OnDrawGizmos()
    {
        RefreshButtonStyle();
    }

    protected virtual void RefreshButtonStyle()
    {
        image.sprite = buttonSprite;
        image.color  = colorTint;
    }
}
