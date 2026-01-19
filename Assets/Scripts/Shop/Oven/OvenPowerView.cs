using UnityEngine;

public class OvenPowerView : MonoBehaviour
{
    [SerializeField] private Oven _oven;
    
    private TMPro.TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _oven.FirePowerChanged += OnFirePowerChanged;
    }

    private void OnDisable()
    {
        _oven.FirePowerChanged -= OnFirePowerChanged;
    }

    private void OnFirePowerChanged(int power)
    {
        _text.text = power.ToString();
    }
}
