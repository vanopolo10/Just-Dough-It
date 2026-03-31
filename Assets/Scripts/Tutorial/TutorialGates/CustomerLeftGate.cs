using System;
using UnityEngine;

public class CustomerLeftGate : ITutorialGate
{
    private readonly CustomerRouteMover _routeMover;

    public event Action Completed;

    public GameObject IconObject { get; }

    public CustomerLeftGate(CustomerRouteMover routeMover, GameObject iconObject = null)
    {
        _routeMover = routeMover;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _routeMover.LeftCafe += OnLeft;
    }

    private void OnLeft()
    {
        Completed?.Invoke();
    }
}