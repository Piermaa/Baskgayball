using UnityEngine;
using Vuforia;

public class ObjectPlacer : MonoBehaviour
{
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
    }

    private void OnDisable()
    {
        GetComponent<PlaneFinderBehaviour>().OnInteractiveHitTest.RemoveListener(PlaceObject);
    }

    public void PlaceObject(HitTestResult testResult)
    {
        Instantiate(SelectObject(), testResult.Position, testResult.Rotation);
    }

    private GameObject SelectObject()
    {
        return placeA ? objectToPlaceA : objectToPlaceB;
    }
}
