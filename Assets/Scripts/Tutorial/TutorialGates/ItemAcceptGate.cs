using System;
using UnityEngine;

public class ItemAcceptGate : ITutorialGate
{
    private Customer _customer;
    private bool _isMoveOrPastryGate;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public ItemAcceptGate(Customer customer, GameObject iconObject = null)
    {
        _customer = customer;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _customer.ProductAccepted += OnProductAccepted;
    }

    private void OnProductAccepted(GameObject _)
    {
        Completed?.Invoke();
    }
}