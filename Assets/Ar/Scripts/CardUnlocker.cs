using UnityEngine;
using UnityEngine.TextCore.Text;
using Vuforia;

public class CardUnlocker : MonoBehaviour
{
    [SerializeField] private GameObject mCharacter;
    private ObserverBehaviour mObserverBehaviour;

    void OnEnable()
    {
        mObserverBehaviour = GetComponent<ObserverBehaviour>();
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
            VuforiaDebugger.Instance.AddPrint("Started listener for tracker", Color.white);
        }
        else
        {
            VuforiaDebugger.Instance.AddPrint("Not found observerBehavior", Color.red);
        }
    }

    void OnDestroy()
    {
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
            VuforiaDebugger.Instance.AddPrint("Destroyed", Color.yellow);
        }
    }

    public void UnlockCard()
    {
        string imageName = mObserverBehaviour.TargetName;

        VuforiaDebugger.Instance.AddPrint("Recognized card: " + imageName, Color.white);
        mCharacter.SetActive(true);
        DispararEventoGlobal(imageName);
    }

    public void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            UnlockCard();
        }
        else 
        {
            mCharacter.SetActive(false);
            VuforiaDebugger.Instance.AddPrint("Lost target", Color.white);

        }
    }

    private void DispararEventoGlobal(string imageName)
    {
        ArEvents.InvokeCardUnlock(imageName);
    }
}
