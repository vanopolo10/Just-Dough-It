using UnityEngine;
using UnityEngine.Events;

public class PaidEvent : MonoBehaviour
{
    [SerializeField] private int price;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failureEvent;
    [SerializeField] private MoneyManager moneyManager;

    private void Awake()
    {
        if(moneyManager == null) 
            moneyManager = FindFirstObjectByType<MoneyManager>();
    }

    public void TryEvent() 
    {
        if (moneyManager.TrySpendMoney(price))
            _successEvent?.Invoke();
        else 
            _failureEvent?.Invoke();
    }
}
