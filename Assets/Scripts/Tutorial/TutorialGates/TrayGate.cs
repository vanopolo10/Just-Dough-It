using System;
using UnityEngine;

public class TrayGate : ITutorialGate
{
    private Tray _tray;
    private bool _isMoveOrPastryGate;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public TrayGate(Tray tray, bool isMoveOrPastryGate, GameObject iconObject = null)
    {
        _tray = tray;
        _isMoveOrPastryGate = isMoveOrPastryGate;
        IconObject = iconObject;
    }

    public void Enter()
    {
        if (_isMoveOrPastryGate)
            _tray.MovedToOven += OnTrayMoved;
        else
            _tray.PastryRemoved += OnPastryRemoved;
    }

    private void OnPastryRemoved()
    {
        _tray.PastryRemoved -= OnPastryRemoved;
        Completed?.Invoke();
    }

    private void OnTrayMoved(bool toOven)
    {
        if (!toOven) return;
        
        _tray.MovedToOven -= OnTrayMoved;
        Completed?.Invoke();
    }
}