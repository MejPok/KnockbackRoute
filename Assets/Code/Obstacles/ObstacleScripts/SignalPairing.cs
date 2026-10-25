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


    void Update()
    {
        if(assignedId != -1)
        {
            return;
        }

        timer += Time.deltaTime;
        if(timer >= timeUntilGetId)
        {
            GetSignalId();
            SignalManager.NewSignalTriggered += OnInvoked;

        }
    }
    public void OnInvoked(int id)
    {
        if(id == assignedId)
        {
            onInvokeEvents.Invoke();   
            
        }
    }

    public void InvokeSignal()
    {
        SignalManager.NewSignal(assignedId);
    }

    public void GetSignalId()
    {
        assignedId = SignalIdGiver.GetId(possibleSignalId);
    }
}
