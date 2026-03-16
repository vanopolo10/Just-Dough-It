using System;
using UnityEngine;

public class RecipeGate : ITutorialGate
{
    private RecipeManager _recipeManager;
    private ProductType _target;

    public event Action Completed;
    
    public GameObject IconObject { get; }

    public RecipeGate(RecipeManager recipeManager, ProductType target, GameObject iconObject = null)
    {
        _recipeManager = recipeManager;
        _target = target;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _recipeManager.ActiveRecipeChanged += OnStateChanged;
    }

    private void OnStateChanged(ProductType productType)
    {
        if (productType != _target)
            return;

        _recipeManager.ActiveRecipeChanged -= OnStateChanged;
        Completed?.Invoke();
    }
}