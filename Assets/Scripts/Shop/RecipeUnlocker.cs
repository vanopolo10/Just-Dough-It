using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RecipeUnlocker : MonoBehaviour
{
    [SerializeField] private List<RecipeBookPage> _recipesToUnlock;

    public void UnlockAllRecipes() { 
        Book book = GetComponent<Book>();
        if(book == null) book = FindAnyObjectByType<Book>();
        if (book == null)
        {
            Debug.Log("[RecipeUnlocker] No book found in scene");
            return;
        }

        foreach (RecipeBookPage page in _recipesToUnlock) { 
            book.AddNewPage(page);
            Debug.Log($"[RecipeUnlocker] added a new page: {page.NameKey}, {page.ProductType}, {page.RecipeSprite.name}");
        }
    }

}
