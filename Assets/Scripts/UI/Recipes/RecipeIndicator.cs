using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeIndicator : MonoBehaviour
{
    [Serializable]
    protected struct IndicatorGroup 
    {
        public ProductType type;
        public List<GameObject> indicators;
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

    public void UpdateVisibility() 
    {
        foreach (IndicatorGroup group in _indicatorGroups)
        { 
            bool groupVisibility = false;
            if(group.type == _manager.CurrentRecipeType)
                groupVisibility = true;

            foreach (GameObject indicator in group.indicators)
                indicator.SetActive(groupVisibility);

            Debug.Log("visibility of type " + group.type + " updated to " + groupVisibility);
        }
    }
}
