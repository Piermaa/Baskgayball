using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Vuforia;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ObjectPlacer : MonoBehaviour
{
    public enum PlaceableObjectType
    {
        Cube,
        Sphere
    }

    [SerializeField] private GameObject groundPlane;
    [SerializeField] private string cubeChildName = "Cube";
    [SerializeField] private string sphereChildName = "Sphere";
    [SerializeField] private PlaceableObjectType currentObjectType = PlaceableObjectType.Cube;

    private PlaneFinderBehaviour planeFinder;
    private int placedObjectsCount = 0;

    public PlaceableObjectType CurrentObjectType => currentObjectType;

    private void Awake()
    {
        planeFinder = GetComponent<PlaneFinderBehaviour>();
    }

    private void Start()
    {
        if (groundPlane != null)
        {
            VuforiaDebugger.Instance?.AddPrint("Template listo: " + groundPlane.name, Color.green);
        }
        else
        {
            VuforiaDebugger.Instance?.AddPrint("ALERTA: Template groundPlane es NULL!", Color.red);
        }
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += HandleFingerDown;

        if (planeFinder == null)
        {
            planeFinder = GetComponent<PlaneFinderBehaviour>();
        }

        if (planeFinder != null)
        {
            planeFinder.OnInteractiveHitTest.RemoveListener(PlaceObject);
            planeFinder.OnInteractiveHitTest.AddListener(PlaceObject);
            VuforiaDebugger.Instance?.AddPrint("Plane listener registrado", Color.white);
        }
        else
        {
            VuforiaDebugger.Instance?.AddPrint("ERROR: PlaneFinderBehaviour no encontrado!", Color.red);
        }
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= HandleFingerDown;
        EnhancedTouchSupport.Disable();

        if (planeFinder != null)
        {
            planeFinder.OnInteractiveHitTest.RemoveListener(PlaceObject);
        }
    }

    private void HandleFingerDown(Finger finger)
    {
        Vector2 screenPos = finger.screenPosition;
        VuforiaDebugger.Instance?.AddPrint($"Toque pantalla (Touch) en ({screenPos.x:F0}, {screenPos.y:F0})", Color.yellow);

        if (!IsPointerOverUI(screenPos) && planeFinder != null)
        {
            VuforiaDebugger.Instance?.AddPrint("Ejecutando PerformHitTest...", Color.white);
            planeFinder.PerformHitTest(screenPos);
        }
    }



    public void SelectCube()
    {
        SetObjectToPlace(true);
    }

    public void SelectSphere()
    {
        SetObjectToPlace(false);
    }

    public void SetObjectToPlace(bool isCube)
    {
        currentObjectType = isCube ? PlaceableObjectType.Cube : PlaceableObjectType.Sphere;
        VuforiaDebugger.Instance?.AddPrint("Seleccionado: " + (isCube ? "CUBO" : "ESFERA"), Color.cyan);
    }

    public void SetObjectType(int typeIndex)
    {
        currentObjectType = (PlaceableObjectType)typeIndex;
        VuforiaDebugger.Instance?.AddPrint("Seleccionado: " + currentObjectType, Color.cyan);
    }

    public void PlaceObject(HitTestResult testResult)
    {
        VuforiaDebugger.Instance?.AddPrint("Vuforia: Hit test en piso recibido!", Color.white);

        if (groundPlane == null)
        {
            VuforiaDebugger.Instance?.AddPrint("ERROR: groundPlane template es NULL!", Color.red);
            Debug.LogError("[ObjectPlacer] groundPlane is not assigned.");
            return;
        }

        // Instanciar un duplicado del Ground Plane en la posición y rotación detectada
        GameObject newInstance = Instantiate(groundPlane, testResult.Position, testResult.Rotation);
        newInstance.SetActive(true);
        placedObjectsCount++;

        // Buscar los hijos dentro de la nueva instancia
        Transform cubeChild = FindChild(newInstance.transform, cubeChildName, "Cube");
        Transform sphereChild = FindChild(newInstance.transform, sphereChildName, "Sphere");

        bool isCube = currentObjectType == PlaceableObjectType.Cube;

        if (cubeChild != null)
        {
            cubeChild.gameObject.SetActive(isCube);
        }
        else
        {
            VuforiaDebugger.Instance?.AddPrint("AVISO: No se encontro hijo '" + cubeChildName + "'", Color.yellow);
        }

        if (sphereChild != null)
        {
            sphereChild.gameObject.SetActive(!isCube);
        }
        else
        {
            VuforiaDebugger.Instance?.AddPrint("AVISO: No se encontro hijo '" + sphereChildName + "'", Color.yellow);
        }

        string objName = isCube ? "CUBO" : "ESFERA";
        VuforiaDebugger.Instance?.AddPrint($"Instanciado {objName} con exito! Total en escena: {placedObjectsCount}", Color.green);
    }

    private bool IsPointerOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            VuforiaDebugger.Instance?.AddPrint("Toque ignorado: sobre UI (" + results[0].gameObject.name + ")", Color.yellow);
            return true;
        }

        return false;
    }

    private Transform FindChild(Transform parent, string targetName, string altName = null)
    {
        Transform direct = parent.Find(targetName);
        if (direct != null) return direct;

        if (!string.IsNullOrEmpty(altName))
        {
            Transform alt = parent.Find(altName);
            if (alt != null) return alt;
        }

        foreach (Transform child in parent)
        {
            if (string.Equals(child.name, targetName, System.StringComparison.OrdinalIgnoreCase))
                return child;

            if (!string.IsNullOrEmpty(altName) &&
                string.Equals(child.name, altName, System.StringComparison.OrdinalIgnoreCase))
                return child;

            Transform recursive = FindChild(child, targetName, altName);
            if (recursive != null) return recursive;
        }

        return null;
    }
}
