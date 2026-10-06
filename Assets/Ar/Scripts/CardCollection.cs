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
    private HashSet<string> unlockedCards;

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
        if (unlockedCards.Add(obj))
        {
            m_TextMeshPro.text = unlockedCards.Count.ToString();
        }
    }

}
