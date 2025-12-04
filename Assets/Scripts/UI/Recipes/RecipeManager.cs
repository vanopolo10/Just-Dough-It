using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class RecipeManager : MonoBehaviour
{
    [Serializable]
    protected struct RecipeState
    {
        public ProductType product;
        public bool active;
        public RecipeState(ProductType product, bool active)
        {
            this.product = product;
            this.active = active;
        }
        public void SetActive(bool newAct) { active = newAct; }
    }
    [Serializable]
    protected struct PageSprite
    {
        [SerializeField] private Sprite active, inactive;
        public Sprite GetSprite(bool isActive)
        {
            if (isActive)
                return active;
            return inactive;
        }
    }



    [SerializeField] private int _firstPageIndex = 1;
    [SerializeField] private List<RecipeState> _recipeStates;
    [SerializeField] private List<PageSprite> _pageSprites;
    [SerializeField] private ProductType _currentRecipeType;
    [SerializeField] private Book _book;

    public UnityEvent OnActiveRecipeChanged = null;
    public ProductType CurrentRecipeType => _currentRecipeType;
    
    public void Start()
    {
        if (_book == null)
            _book = FindFirstObjectByType<Book>();

        RefreshPages();
    }

    public void RefreshPages()
    {
        for (int i = 0; i < _recipeStates.Count; i++)
        {
            _book.bookPages[i + _firstPageIndex] = _pageSprites[i].GetSprite( _recipeStates[i].active );
        }

        _book.CustomRefreshSprites();
    }

    public void SetActiveRecipe(int index) 
    {
        for (int i = 0; i < _recipeStates.Count; i++)
        {
            RecipeState state = _recipeStates[i];
            if (i == index)
                _recipeStates[i] = new RecipeState(state.product, true);
            else 
                _recipeStates[i] = new RecipeState(state.product, false);
        }

        RefreshPages();
    }

    public void ProcessButtonPress(bool isLeftButton)
    {
        int index = _book.currentPage - _firstPageIndex - (isLeftButton ? 1 : 0);
        Debug.Log(index);

        if (index < 0)
            return;

        if (!_recipeStates[index].active)
        {
            SetActiveRecipe(index);
            Debug.Log("set active recipe to " + _recipeStates[index].product);
            _currentRecipeType = _recipeStates[index].product;
            OnActiveRecipeChanged.Invoke();
        }
        else
        {
            SetActiveRecipe(-1);
            Debug.Log("removed active recipe");
            _currentRecipeType = ProductType.None;
            OnActiveRecipeChanged.Invoke();
        }
    }
}
