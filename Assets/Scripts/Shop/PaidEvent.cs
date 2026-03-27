using UnityEngine;
using UnityEngine.Events;

public class PaidEvent : MonoBehaviour
{
    [SerializeField] private int _price;
    [SerializeField] private UnityEvent _successEvent;
    [SerializeField] private UnityEvent _failureEvent;
    private MoneyManager _moneyManager;
    public int Price => _price;

    private void Awake()
    {
        if(_moneyManager == null) 
            _moneyManager = FindFirstObjectByType<MoneyManager>();
    }
    public void SetPrice(int price) {  _price = price; }
    public void TryEvent() 
    {
        if (_moneyManager.TrySpendMoney(_price))
            _successEvent?.Invoke();
        else 
            _failureEvent?.Invoke();
    }
}
