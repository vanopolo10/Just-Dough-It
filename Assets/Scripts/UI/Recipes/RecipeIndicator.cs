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
    private Book _book;

    private void OnEnable()
    {
        FindBook();
        UpdateVisibility(_book.CurrentSelectedProduct);

        if (_book != null)
            _book.RecipeChanged += UpdateVisibility;
    }

    private void OnDisable()
    {
        if (_book != null)
            _book.RecipeChanged -= UpdateVisibility;
    }

    private void FindBook()
    {
        if (_book == null)
            _book = FindFirstObjectByType<Book>();
    }

    private void UpdateVisibility(ProductType productType)
    {
        var activeGroup = _indicatorGroups.FirstOrDefault(g => g.Type == productType);

        foreach (var indicator in _indicatorGroups.SelectMany(group =>
                     group.Indicators.Where(indicator => indicator != null)))
            indicator.SetActive(false);

        if (activeGroup.Indicators == null) return;

        foreach (var indicator in activeGroup.Indicators.Where(indicator => indicator != null))
            indicator.SetActive(true);
    }
}