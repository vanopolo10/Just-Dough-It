using System;
using UnityEngine;

public class DoughRemovedGate : ITutorialGate
{
    private DoughBucket _doughBucket;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public DoughRemovedGate(DoughBucket doughBucket, GameObject iconObject = null)
    {
        _doughBucket = doughBucket;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _doughBucket.DoughRemoved += OnDoughRemoved;
    }

    private void OnDoughRemoved()
    {
        _doughBucket.DoughRemoved -= OnDoughRemoved;
        Completed?.Invoke();
    }
}