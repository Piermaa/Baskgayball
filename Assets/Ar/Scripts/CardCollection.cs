using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class ArEvents
{
    public static event Action<string> OnCardUnlock;

    public static void InvokeCardUnlock(string cardID) 
    {
        OnCardUnlock?.Invoke(cardID);
    }
}

public class CardCollection : MonoBehaviour
{
    public Vuforia.Image image;
    [SerializeField] private TextMeshProUGUI m_TextMeshPro;
    private HashSet<string> unlockedCards = new();

    private void Start()
    {
        m_TextMeshPro.text = string.Empty;
    }

    private void OnEnable()
    {
        ArEvents.OnCardUnlock += ArEvents_OnCardUnlock;
    }
    private void OnDisable()
    {
        ArEvents.OnCardUnlock -= ArEvents_OnCardUnlock;
    }

    private void ArEvents_OnCardUnlock(string obj)
    {
        VuforiaDebugger.Instance.AddPrint("Collection received unlock try: " + obj, Color.orange);
        
        if (!unlockedCards.Contains(obj))
        {
            VuforiaDebugger.Instance.AddPrint("Unlocked: " + obj, Color.green);
            unlockedCards.Add(obj);
            m_TextMeshPro.text = "Unlocked cards: " + unlockedCards.Count.ToString();
        }
        else
        {
            VuforiaDebugger.Instance.AddPrint("Not unlocked: " + obj, Color.red);
        }
    }

}
