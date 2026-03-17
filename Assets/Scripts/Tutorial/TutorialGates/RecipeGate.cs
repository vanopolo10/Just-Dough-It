using System;
using UnityEngine;

public class RecipeGate : ITutorialGate
{
    private Book _book;
    private ProductType _target;

    public event Action Completed;
    
    public GameObject IconObject { get; }

    public RecipeGate(Book book, ProductType target, GameObject iconObject = null)
    {
        _book = book;
        _target = target;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _book.RecipeChanged += OnStateChanged;
    }

    private void OnStateChanged(ProductType productType)
    {
        if (productType != _target)
            return;

        _book.RecipeChanged -= OnStateChanged;
        Completed?.Invoke();
    }
}