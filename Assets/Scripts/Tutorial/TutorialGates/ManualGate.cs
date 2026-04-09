using System;
using UnityEngine;

public class ManualGate : ITutorialGate
{
    private Manual _manual;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public ManualGate(Manual manual, GameObject iconObject = null)
    {
        _manual = manual;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _manual.Opened += OnManualOpened;
    }

    private void OnManualOpened()
    {
        Completed?.Invoke();
    }
}