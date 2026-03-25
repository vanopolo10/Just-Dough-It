using System;
using UnityEngine;

public class PutFillingGate : ITutorialGate
{
    private DoughBucket _doughBucket;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public PutFillingGate(DoughBucket doughBucket, GameObject iconObject = null)
    {
        _doughBucket = doughBucket;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _doughBucket.FillingChanged += OnFillingChanged;
    }

    private void OnFillingChanged(FillingType fillingType)
    {
        if ((fillingType == FillingType.Jam | fillingType == FillingType.Meat) == false) return;
        
        _doughBucket.FillingChanged -= OnFillingChanged;
        Completed?.Invoke();
    }
}