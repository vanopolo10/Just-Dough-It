using System;
using UnityEngine;

public class ActionGate : ITutorialGate
{
    private readonly Action _action;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public ActionGate(Action action)
    {
        _action = action;
    }

    public void Enter()
    {
        _action?.Invoke();
        Completed?.Invoke();
    }
}