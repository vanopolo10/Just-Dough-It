using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    [SerializeField] private List<Page> _pages;
    [SerializeField] private Image _imageLeft;
    [SerializeField] private Image _imageRight;
    [SerializeField] private TMP_Text _textLeft;
    [SerializeField] private TMP_Text _textRight;
    [SerializeField] private Button _navigationButtonLeft;
    [SerializeField] private Button _navigationButtonRight;
    [SerializeField] private Button _recipeButtonLeft;
    [SerializeField] private Button _recipeButtonRight;
    
    private int _id;
    private Page _currentPageLeft;
    private Page _currentPageRight;

    public event Action<ProductType> RecipeChanged;

    public ProductType CurrentSelectedProduct { get; private set; }
    
    private void Start()
    {
        _id = 0;
        CurrentSelectedProduct = ProductType.None;
        SetSpritesAndText();
        UpdateNavigationButtons();
    }

    public void NextPage()
    {
        bool canGoToNextPage = (_id + 1) * 2 < _pages.Count;
        bool isLastPageWithOnlyLeft = _pages.Count % 2 != 0 && _id * 2 + 1 == _pages.Count - 1;
        
        if (canGoToNextPage || isLastPageWithOnlyLeft)
        {
            _id++;
            SetSpritesAndText();
            UpdateNavigationButtons();
        }
    }
    
    public void PreviousPage()
    {
        if (_id > 0)
        {
            _id--;
            SetSpritesAndText();
            UpdateNavigationButtons();
        }
    }

    private void SetSpritesAndText()
    {
        _currentPageLeft = _pages[_id * 2];
        _imageLeft.sprite = GetCorrectSprite(_currentPageLeft);
        _textLeft.text = _currentPageLeft.NameKey;
        
        _recipeButtonLeft.onClick.RemoveAllListeners();
        ProductType leftProductType = _currentPageLeft.ProductType;
        _recipeButtonLeft.onClick.AddListener(() => SetRecipe(leftProductType));

        int rightIndex = _id * 2 + 1;
        
        if (rightIndex < _pages.Count)
        {
            _currentPageRight = _pages[rightIndex];
            _imageRight.sprite = GetCorrectSprite(_currentPageRight);
            _textRight.text = _currentPageRight.NameKey;
            _imageRight.gameObject.SetActive(true);
            _textRight.gameObject.SetActive(true);

            _recipeButtonRight.onClick.RemoveAllListeners();
            ProductType rightProductType = _currentPageRight.ProductType;
            _recipeButtonRight.onClick.AddListener(() => SetRecipe(rightProductType));
            _recipeButtonRight.gameObject.SetActive(true);
        }
        else
        {
            _imageRight.sprite = null;
            _imageRight.gameObject.SetActive(false);
            _textRight.gameObject.SetActive(false);
            _recipeButtonRight.gameObject.SetActive(false);
        }
    }

    private Sprite GetCorrectSprite(Page page)
    {
        if (CurrentSelectedProduct != ProductType.None && CurrentSelectedProduct == page.ProductType)
            return page.RecipeActiveSprite;

        return page.RecipeSprite;
    }

    private void UpdateNavigationButtons()
    {
        _navigationButtonLeft.gameObject.SetActive(_id > 0);
        
        bool canGoForward = (_id + 1) * 2 < _pages.Count || 
                           (_pages.Count % 2 != 0 && _id * 2 + 1 < _pages.Count - 1);
        _navigationButtonRight.gameObject.SetActive(canGoForward);
    }

    private void SetRecipe(ProductType productType)
    {
        _imageLeft.sprite = _currentPageLeft.RecipeSprite;
        if (_currentPageRight.ProductType != ProductType.None)
        {
            _imageRight.sprite = _currentPageRight.RecipeSprite;
        }

        if (_currentPageLeft.ProductType == productType)
        {
            _imageLeft.sprite = _currentPageLeft.RecipeActiveSprite;
        }
        else if (_currentPageRight.ProductType == productType)
        {
            _imageRight.sprite = _currentPageRight.RecipeActiveSprite;
        }
        
        CurrentSelectedProduct = productType;
        RecipeChanged?.Invoke(productType);
    }
    
    [Serializable]
    private struct Page
    {
        [SerializeField] public ProductType ProductType;
        [SerializeField] public Sprite RecipeSprite;
        [SerializeField] public Sprite RecipeActiveSprite;
        [SerializeField] public string NameKey;
    }
}
