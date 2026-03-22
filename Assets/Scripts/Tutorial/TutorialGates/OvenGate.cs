using System;
using UnityEngine;

public class OvenGate : ITutorialGate
{
    private Oven _oven;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public OvenGate(Oven oven, GameObject iconObject = null)
    {
        _oven = oven;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _oven.WoodAdded += OnWoodAdded;
    }

    private void OnWoodAdded()
    {
        Completed?.Invoke();
    }
}