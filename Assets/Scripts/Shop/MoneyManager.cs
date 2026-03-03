using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private int _money;
    [SerializeField] private TextMeshProUGUI _display;
    [SerializeField] private Canvas _questCanvas;
    [SerializeField] private MoneyPopUp _popUpPrefab;

    public int Money => _money;

    private void Start()
    {
        UpdDisplay();
    }
    
    public void AddMoney(int amount, bool playPopUp = true)
    {
        _money += amount;

        if (playPopUp)
            Instantiate(_popUpPrefab, _questCanvas.transform).Initialize(amount);
        
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
    }
}
