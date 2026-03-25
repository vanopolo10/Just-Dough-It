using UnityEngine;
using UnityEngine.UI;

public class Thermometer : MonoBehaviour
{
    [SerializeField] private Button _addWoodButton;
    [SerializeField] private Oven _oven;

    private bool _canAddWood;
    
    private void OnEnable()
    {
        _addWoodButton.onClick.AddListener(TryAddWood);
    }

    public void SetCanAddWood(bool canAdd)
    {
        _addWoodButton.interactable = canAdd;
        _canAddWood = canAdd;
    }
    
    private void TryAddWood()
    {
        if(_canAddWood)
            _oven.AddWood();
    }
}
