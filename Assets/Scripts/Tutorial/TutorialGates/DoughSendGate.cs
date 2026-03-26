using System;
using UnityEngine;

public class DoughSendGate : ITutorialGate
{
    private OvenSender _ovenSender;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public DoughSendGate(OvenSender ovenSender, GameObject iconObject = null)
    {
        _ovenSender = ovenSender;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _ovenSender.DoughSent += OnDoughSent;
    }

    private void OnDoughSent()
    {
        _ovenSender.DoughSent -= OnDoughSent;
        Completed?.Invoke();
    }
}