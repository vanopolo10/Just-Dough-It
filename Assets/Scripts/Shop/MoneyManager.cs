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

    private void Start() => UpdateText();

    public void AddMoney(int amount, bool playPopUp = true)
    {
        _money += amount;
        
        if (playPopUp)
            Instantiate(_popUpPrefab).GetComponentInChildren<MoneyPopUp>().Initialize(amount);

        UpdateText();
        OnBalanceChanged?.Invoke();
    }

    public void SetMoney(int amout)
    {
        _money = Math.Clamp(amout, 0, int.MaxValue);
        UpdateText();
    }

    public bool TrySpendMoney(int amount) 
    {
        if (_money < amount) 
            return false;

        _money -= amount;
        UpdateText();
        OnBalanceChanged?.Invoke();
        return true;
    }
    
    private void UpdateText() => _display.text = _money + "ð";
}
