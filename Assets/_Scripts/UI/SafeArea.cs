using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;

    private Rect lastSafeArea;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private ScreenOrientation lastOrientation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        ApplySafeArea();
    }

    private void Update()
    {
        if (Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight ||
            Screen.orientation != lastOrientation)
        {
            ApplySafeArea();
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        if (rectTransform != null &&
            (Screen.width != lastScreenWidth ||
             Screen.height != lastScreenHeight ||
             Screen.orientation != lastOrientation))
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if (Screen.width <= 0 || Screen.height <= 0) return;

        Rect safeArea = Screen.safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        lastOrientation = Screen.orientation;

        if (safeArea == lastSafeArea && rectTransform.anchorMin != Vector2.zero)
        {
            return;
        }

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;

        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        lastSafeArea = safeArea;
    }
}
