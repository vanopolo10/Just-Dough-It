using System;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private int _money;
    [SerializeField] private TextMeshProUGUI _display;
    [SerializeField] private Canvas _popUpPrefab;
    public event Action OnBalanceChanged;

    public int Money => _money;

    private void Start()
    {
        UpdDisplay();
    }

    public void AddMoney(int amount, bool playPopUp = true)
    {
        _money += amount;

        if (playPopUp)
            Instantiate(_popUpPrefab).GetComponentInChildren<MoneyPopUp>().Initialize(amount);

        UpdDisplay();
    }

    public bool TrySpendMoney(int amount) 
    {
        if (_money < amount) 
            return false;

        _money -= amount;
        UpdDisplay();
        return true;
    }
    
    private void UpdDisplay()
    { 
        _display.text = _money + "ð";
        OnBalanceChanged?.Invoke();
    }
}
