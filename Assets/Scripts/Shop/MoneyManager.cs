using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private int _money;
    [SerializeField] private TextMeshProUGUI _display;
    
    public int Money => _money;

    private void Start()
    {
        UpdDisplay();
    }
    
    private void UpdDisplay()
    { 
        _display.text = _money.ToString() + "ð";
    }
    
    public void AddMoney(int amount)
    {
        _money += amount;
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
}
