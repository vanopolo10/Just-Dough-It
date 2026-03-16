using System;
using UnityEngine;

public class DoughStateGate : ITutorialGate
{
    private DoughBucket _bucket;
    private DoughState _target;

    public event Action Completed;
    
    public GameObject IconObject { get; }

    public DoughStateGate(DoughBucket bucket, DoughState target, GameObject iconObject = null)
    {
        _bucket = bucket;
        _target = target;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _bucket.DoughStateChanged += OnStateChanged;
    }

    private void OnStateChanged(DoughState state)
    {
        if (state != _target)
            return;

        _bucket.DoughStateChanged -= OnStateChanged;
        Completed?.Invoke();
    }
}