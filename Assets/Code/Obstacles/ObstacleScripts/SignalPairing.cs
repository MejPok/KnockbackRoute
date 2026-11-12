using System;
using UnityEngine;
using UnityEngine.Events;

public class SignalPairing : MonoBehaviour
{
    public string possibleSignalId;
    public int assignedId = -1;
    public UnityEvent onInvokeEvents;
    public Action signalActionEvent;

    public bool waitingForId = false;

    private void OnEnable()
    {
        SignalManager.NewSignalTriggered += OnInvoked;

        if (assignedId == -1)
            GetSignalId();
    }

    private void OnDisable()
    {
        SignalManager.NewSignalTriggered -= OnInvoked;
    }

    public void OnInvoked(string possible, int id)
    {
        if (id == assignedId && possible == possibleSignalId)
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
        if (assignedId != -1 || waitingForId) return;

        SignalIdGiver.WaitForId(this);
        waitingForId = true;
    }

    private void OnDestroy()
    {
        SignalManager.NewSignalTriggered -= OnInvoked;
        SignalIdGiver.WaitingForId.Remove(this);
        SignalIdGiver.signalPairs.Remove(this);
    }
}