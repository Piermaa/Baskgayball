using UnityEngine;
using Vuforia;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject groundPlane;
    [SerializeField] private GameObject objectToPlaceA;
    [SerializeField] private GameObject objectToPlaceB;

    private bool placeA;

    public void SetObjectToPlace(bool newPlaceA)
    {
        placeA = newPlaceA;
    }

    private void OnEnable()
    {
        GetComponent<PlaneFinderBehaviour>().OnInteractiveHitTest.AddListener(PlaceObject);
        VuforiaDebugger.Instance.AddPrint("Added listener", Color.white);
    }

    private void OnDisable()
    {
        GetComponent<PlaneFinderBehaviour>().OnInteractiveHitTest.RemoveListener(PlaceObject);
       // VuforiaDebugger.Instance.AddPrint("Removed listener", Color.yellow);
    }

    public void PlaceObject(HitTestResult testResult)
    {
        Instantiate(SelectObject(), testResult.Position, testResult.Rotation, transform);
    }

    private GameObject SelectObject()
    {
        return placeA ? objectToPlaceA : objectToPlaceB;
    }
}
