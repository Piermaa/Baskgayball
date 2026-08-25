using UnityEngine;
using TMPro;

public class TextButtonManager : ButtonManager
{
    [Header("Setup - Text")]
    [SerializeField] private string displayText = null;
    [SerializeField] private float fontSize = 30;
    [Header("Internal - Text")]
    [SerializeField] private TextMeshProUGUI buttonText = null;

    protected override void RefreshButtonStyle()
    {
        base.RefreshButtonStyle();
        buttonText.text = displayText;
        buttonText.fontSize = fontSize;
    }
}
