<<<<<<< HEAD
=======
using NUnit.Framework;
>>>>>>> parent of 09d084a (Revert "Merge branch 'ShopRework'")
using System.Collections.Generic;
using UnityEngine;

public class MegaShopBook : ShopBook
{
    [SerializeField] private List<SingularShopBook> _books;
    [SerializeField] private AnimationClip _finalClip;
    [SerializeField] private float _finalClipSpeed = 1, _finalDelay = 0;
<<<<<<< HEAD
    
    private int _booksLeft;

=======
    private int _booksLeft;
>>>>>>> parent of 09d084a (Revert "Merge branch 'ShopRework'")
    public override void Start()
    {
        base.Start();
        _booksLeft = _books.Count;
        Debug.Log($"Megabook initial position set to {_initialPosition} while actual position is {transform.position}");
    }

    public override void OnMovedOutOfPosition()
    {
<<<<<<< HEAD
        _canvas.SetActive(false);
        Debug.Log($"Megabook moving out of position, from {transform.position} to {_initialPosition}");

        foreach (var book in _books)
            book.OnMovedOutOfPosition();
=======
        _canvas.SetActive( false );
        Debug.Log($"Megabook moving out of position, from {transform.position} to {_initialPosition}");
        for (int i = 0; i < _books.Count; i++)
        {    
            _books[i].OnMovedOutOfPosition();
        }
>>>>>>> parent of 09d084a (Revert "Merge branch 'ShopRework'")
    }

    public override void OnMovedToPosition()
    {
        _canvas.SetActive(_bought);
<<<<<<< HEAD

        foreach (var book in _books)
            book.OnMovedToPosition();
    }

    public void OnChildBookPurchase()
    {
=======
        for (int i = 0; i < _books.Count; i++)
        {
            _books[i].OnMovedToPosition();
        }
    }

    public void OnChildBookPurchase() { 
>>>>>>> parent of 09d084a (Revert "Merge branch 'ShopRework'")
        _booksLeft--;
        Debug.Log("Megabook child purchased. books left: " + _booksLeft);
        if (_booksLeft <= 0)
        {
            Debug.Log("Megabook last child was purchased. activating canvas");
            _bought = true;
            _canvas.SetActive(true);
            Debug.Log($"Megabook canvas ({_canvas.name}) activity set to {_canvas.activeSelf}");
        }
<<<<<<< HEAD
    }

    public void DisposeOfBook()
    {
        ShopManager shopManager = GetComponentInParent<ShopManager>();

        if (shopManager == null)
        {
=======
        
    }

    public void DisposeOfBook() { 
        ShopManager shopManager = GetComponentInParent<ShopManager>();
        if (shopManager == null) {
>>>>>>> parent of 09d084a (Revert "Merge branch 'ShopRework'")
            Debug.Log("[MegaShopBook] found no manager to dispose of self");
        }

        _canvas.SetActive(false);
        OnMovedOutOfPosition();
        _animator.SetTrigger("Dispose");

        shopManager.Invoke(nameof(shopManager.CycleBook), (_finalClip.length) / _finalClipSpeed + _finalDelay);
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> parent of 09d084a (Revert "Merge branch 'ShopRework'")
