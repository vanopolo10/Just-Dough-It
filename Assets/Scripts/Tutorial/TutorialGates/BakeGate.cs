using System;
using UnityEngine;

public class BakeGate : ITutorialGate
{
    private Tray _tray;
    private BakeState _bakeState;
    private BakeManager _bakeManager;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public BakeGate(Tray tray, BakeState bakeState, GameObject iconObject = null)
    {
        _tray = tray;
        _bakeState = bakeState;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _bakeManager = _tray.GetFirstBakeManager();
        
        switch (_bakeState)
        {
            case BakeState.Rare:
                _bakeManager.Rare += OnBakeState;
                break;
            case BakeState.Done:
                _bakeManager.Done += OnBakeState;
                break;
            case BakeState.Burn:
                _bakeManager.Burned += OnBakeState;
                break;
            case BakeState.FullBurn:
                _bakeManager.FullBurned += OnBakeState;
                break;
        }
    }

    private void OnBakeState()
    {
        switch (_bakeState)
        {
            case BakeState.Rare:
                _bakeManager.Rare -= OnBakeState;
                break;
            case BakeState.Done:
                _bakeManager.Done -= OnBakeState;
                break;
            case BakeState.Burn:
                _bakeManager.Burned -= OnBakeState;
                break;
            case BakeState.FullBurn:
                _bakeManager.FullBurned -= OnBakeState;
                break;
        }
        
        Completed?.Invoke();
    }
}