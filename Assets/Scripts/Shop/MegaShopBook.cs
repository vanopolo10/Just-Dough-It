using System.Collections.Generic;
using UnityEngine;

public class MegaShopBook : ShopBook
{
    private static readonly int Dispose = Animator.StringToHash("Dispose");
    
    [SerializeField] private List<SingularShopBook> _books;
    [SerializeField] private AnimationClip _finalClip;
    [SerializeField] private float _finalClipSpeed = 1;
    [SerializeField] private float _finalDelay = 0;

    private int _booksLeft;

    public override void Start()
    {
        base.Start();
        _canvas.SetActive(false);

        _booksLeft = _books.Count;
        Debug.Log($"Megabook initial position set to {_initialPosition} while actual position is {transform.position}");
    }

    public override void OnMovedOutOfPosition()
    {
        _canvas.SetActive(false);
        Debug.Log($"Megabook moving out of position, from {transform.position} to {_initialPosition}");

        foreach (var book in _books)
            book.OnMovedOutOfPosition();
    }

    public override void OnMovedToPosition()
    {
        _canvas.SetActive(_bought);

        foreach (var book in _books)
            book.OnMovedToPosition();
    }

    public void OnChildBookPurchase()
    {
        _booksLeft--;
        Debug.Log("Megabook child purchased. books left: " + _booksLeft);
        
        if (_booksLeft > 0) return;
        
        Debug.Log("Megabook last child was purchased. activating canvas");
        _bought = true;
        _canvas.SetActive(true);
        Debug.Log($"Megabook canvas ({_canvas.name}) activity set to {_canvas.activeSelf}");
    }

    public void DisposeOfBook()
    {
        ShopManager shopManager = GetComponentInParent<ShopManager>();
        if (shopManager == null)
        {
            Debug.Log("[MegaShopBook] found no manager to dispose of self");
        }

        _canvas.SetActive(false);
        OnMovedOutOfPosition();
        _animator.SetTrigger(Dispose);

        shopManager.Invoke(nameof(shopManager.CycleBook), (_finalClip.length) / _finalClipSpeed + _finalDelay);
    }
}
