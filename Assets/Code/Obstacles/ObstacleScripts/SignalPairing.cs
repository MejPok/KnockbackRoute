using System;
using UnityEngine;
using UnityEngine.Events;

public class SignalPairing : MonoBehaviour
{
    public string possibleSignalId;
    public int assignedId = -1;
    public UnityEvent onInvokeEvents;
    public Action signalActionEvent;

    float timer;
    public float timeUntilGetId = 1f;

    bool waitingForId = true;

    void Update()
    {
        GetSignalId();
        SignalManager.NewSignalTriggered += OnInvoked;
        
    }
    public void OnInvoked(string possible,int id)
    {
        if(id == assignedId && possible == possibleSignalId)
        {
            onInvokeEvents.Invoke();   
        }
    }

    public void InvokeSignal()
    {
        SignalManager.NewSignal(possibleSignalId, assignedId);
    }

    public void GetSignalId()
    {
        SignalIdGiver.WaitForId(this);
        waitingForId = true;
    }

    void OnDestroy()
    {
        SignalIdGiver.WaitingForId.Remove(this);
        SignalIdGiver.signalPairs.Remove(this);
    }
}
