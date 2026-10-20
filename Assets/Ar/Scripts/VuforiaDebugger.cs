using TMPro;
using UnityEngine;

public class VuforiaDebugger : MonoBehaviour
{
    public static VuforiaDebugger Instance;
    public GameObject printPrefab;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    public void AddPrint(string printText, Color color)
    {
        var print = Instantiate(printPrefab, transform).GetComponent<TextMeshProUGUI>();
        print.text = printText;
        print.color = color;
    }
}
