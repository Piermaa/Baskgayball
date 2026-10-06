using UnityEngine;
using UnityEngine.TextCore.Text;
using Vuforia;

public class CardUnlocker : MonoBehaviour
{
    [SerializeField] private GameObject mCharacter;
    private ObserverBehaviour mObserverBehaviour;

    void Start()
    {
        mObserverBehaviour = GetComponent<ObserverBehaviour>();
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    void OnDestroy()
    {
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            string imageName = mObserverBehaviour.TargetName;

            Debug.Log("Imagen detectada: " + imageName);
            mCharacter.SetActive(true);
            DispararEventoGlobal(imageName);
        }
        else 
        {
            mCharacter.SetActive(false);
        }
    }

    private void DispararEventoGlobal(string imageName)
    {
        ArEvents.InvokeCardUnlock(imageName);
    }
}
