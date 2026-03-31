using System;
using UnityEngine;

public class HatchGate : ITutorialGate
{
    private Hatch _hatch;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public HatchGate(Hatch hatch, GameObject iconObject = null)
    {
        _hatch = hatch;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _hatch.Moved += OnMoved;
    }

    private void OnMoved()
    {
        Completed?.Invoke();
    }
}