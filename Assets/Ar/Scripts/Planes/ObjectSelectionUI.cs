using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionUI : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private ObjectPlacer objectPlacer;

    [Header("Buttons")]
    [SerializeField] private Button cubeButton;
    [SerializeField] private Button sphereButton;

    [Header("Visual Feedback")]
    [SerializeField] private Color selectedColor = new Color(0.3f, 0.85f, 0.4f, 1f);
    [SerializeField] private Color defaultColor = Color.white;

    private void Awake()
    {
        if (objectPlacer == null)
        {
            objectPlacer = FindFirstObjectByType<ObjectPlacer>();
        }
    }

    private void Start()
    {
        if (cubeButton != null)
        {
            cubeButton.onClick.AddListener(SelectCube);
        }

        if (sphereButton != null)
        {
            sphereButton.onClick.AddListener(SelectSphere);
        }

        UpdateVisuals(objectPlacer != null && objectPlacer.CurrentObjectType == ObjectPlacer.PlaceableObjectType.Cube);
    }

    public void SelectCube()
    {
        if (objectPlacer != null)
        {
            objectPlacer.SelectCube();
        }
        UpdateVisuals(true);
    }

    public void SelectSphere()
    {
        if (objectPlacer != null)
        {
            objectPlacer.SelectSphere();
        }
        UpdateVisuals(false);
    }

    private void UpdateVisuals(bool isCubeSelected)
    {
        if (cubeButton != null && cubeButton.targetGraphic != null)
        {
            cubeButton.targetGraphic.color = isCubeSelected ? selectedColor : defaultColor;
        }

        if (sphereButton != null && sphereButton.targetGraphic != null)
        {
            sphereButton.targetGraphic.color = !isCubeSelected ? selectedColor : defaultColor;
        }
    }

    private void OnDestroy()
    {
        if (cubeButton != null)
        {
            cubeButton.onClick.RemoveListener(SelectCube);
        }

        if (sphereButton != null)
        {
            sphereButton.onClick.RemoveListener(SelectSphere);
        }
    }
}
