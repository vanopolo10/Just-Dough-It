using System;
using UnityEngine;

public class OvenGate : ITutorialGate
{
    private Oven _oven;
    private bool _isPowerGate;
    private int _firePowerGate;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public OvenGate(Oven oven, bool isPowerGate, int firePowerGate = 0, GameObject iconObject = null)
    {
        _oven = oven;
        _isPowerGate = isPowerGate;
        _firePowerGate = firePowerGate;
        IconObject = iconObject;
    }

    public void Enter()
    {
        if (_isPowerGate == false)
            _oven.WoodAdded += OnWoodAdded;
        else
            _oven.FirePowerChanged += OnPowerChanged;
    }

    private void OnWoodAdded()
    {
        _oven.WoodAdded -= OnWoodAdded;
        Completed?.Invoke();
    }

    private void OnPowerChanged(int firePower)
    {
        if (_firePowerGate != firePower) return;
        
        _oven.FirePowerChanged -= OnPowerChanged;
        Completed?.Invoke();
    }
}