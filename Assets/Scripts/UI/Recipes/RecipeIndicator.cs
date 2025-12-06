using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeIndicator : MonoBehaviour
{
    [Serializable]
    protected struct IndicatorGroup 
    {
        public ProductType Type;
        public List<GameObject> Indicators;
    }

    [SerializeField] private List<IndicatorGroup> _indicatorGroups;
    private RecipeManager _manager;
    
    private void OnEnable()
    {
        if (_manager == null)
        {
            _manager = FindFirstObjectByType<RecipeManager>();
            _manager.OnActiveRecipeChanged.AddListener(UpdateVisibility);
        }
        
        UpdateVisibility();
    }

    private void UpdateVisibility() 
    {
        var allIndicators = new HashSet<GameObject>();
        var indicatorsToShow = new HashSet<GameObject>();

        foreach (IndicatorGroup group in _indicatorGroups)
        {
            foreach (var indicator in group.Indicators.Where(indicator => indicator != null))
            {
                allIndicators.Add(indicator);

                if (group.Type == _manager.CurrentRecipeType)
                    indicatorsToShow.Add(indicator);
            }
        }

        foreach (var indicator in allIndicators.Where(indicator => indicator != null))
            indicator.SetActive(indicatorsToShow.Contains(indicator));
    }
}