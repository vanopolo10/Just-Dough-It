using JustDough;
using UnityEngine;

public class CuttingStarterZone : MonoBehaviour
{
    [SerializeField] private bool _isAltZone = false;
    [SerializeField] private CuttingZone _associatedCuttingZone;
    private DoughController _doughController;
    private void Awake()
    {
        _doughController = transform.parent.GetComponentInParent<DoughController>();
    }
    public void StartCutting(Knife knife)
    {
        if (_associatedCuttingZone != null && _doughController != null)
        {
            if (_isAltZone)
                _doughController.ApplyAction(DoughCraftAction.BeginAltCutting);
            else
                _doughController.ApplyAction(DoughCraftAction.BeginCutting);

            _associatedCuttingZone.StartCutting(knife);
        }
    }
}
