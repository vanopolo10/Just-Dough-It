using System;
using UnityEngine;

public class DoughPutGate : ITutorialGate
{
    private DoughBucket _doughBucket;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public DoughPutGate(DoughBucket doughBucket, GameObject iconObject = null)
    {
        _doughBucket = doughBucket;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _doughBucket.DoughSpawned += OnDoughRemoved;
    }

    private void OnDoughRemoved()
    {
        _doughBucket.Disable();
        _doughBucket.DoughSpawned -= OnDoughRemoved;
        Completed?.Invoke();
    }
}