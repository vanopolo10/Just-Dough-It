using UnityEngine;
using UnityEngine.Events;

public class PaidEvent : MonoBehaviour
{
    [SerializeField] private int _price;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failureEvent;
    [SerializeField] private MoneyManager _moneyManager;

    private void Awake()
    {
        if(_moneyManager == null) 
            _moneyManager = FindFirstObjectByType<MoneyManager>();
    }

    public void TryEvent() 
    {
        if (_moneyManager.TrySpendMoney(_price))
            _successEvent?.Invoke();
        else 
            _failureEvent?.Invoke();
    }
}
