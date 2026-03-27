using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class SingularShopBook : ShopBook
{
    [SerializeField] protected GameObject _baseButton, _boughtButton, _noMoneyButton, _boughtOverlay;
    [SerializeField] protected int _price = 600;

    private MegaShopBook _parentMegaBook;
    private bool _hasParentBook= false;
    public override void Start()
    {
        base.Start();
        PaidEvent[] events = GetComponentsInChildren<PaidEvent>();
        for (int i = 0; i < events.Length; i++)
        {
            events[i].SetPrice(_price);
        }

        _hasParentBook = transform.parent.TryGetComponent<MegaShopBook>(out _parentMegaBook);
    }

    public void OnEnable()
    {
        _moneyManager = FindAnyObjectByType<MoneyManager>();
        _moneyManager.OnBalanceChanged += UpdateButtons;
    }
    public void OnDisable()
    {
        _moneyManager.OnBalanceChanged -= UpdateButtons;
    }
    private void UpdateButtons()
    {
        _baseButton.SetActive(false);
        _boughtButton.SetActive(false);
        _noMoneyButton.SetActive(false);
        _boughtOverlay.SetActive(false);

        if (_bought)
        {
            _boughtButton.SetActive(true);
            _boughtOverlay.SetActive(true);
        }
        else if (_price > _moneyManager.Money)
        {
            _noMoneyButton.SetActive(true);
        }
        else
        {
            _baseButton.SetActive(true);
        }
    }
    public override void OnMovedToPosition()
    {
        UpdateButtons();
        _canvas.SetActive(true);
    }

    public override void OnMovedOutOfPosition()
    {
        _canvas.SetActive(false);
    }

    public void OnSuccessfulPurchase()
    {
        _bought = true;
        UpdateButtons();

        if (_hasParentBook) _parentMegaBook.OnChildBookPurchase();
        else Debug.Log("Shop book purchased, but has no parent");
    }
}
public class ShopBook : MonoBehaviour
{
    [SerializeField] protected Transform _target;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected GameObject _canvas;

    protected GameObject _book;
    protected MoneyManager _moneyManager;
    protected bool _bought = false;

    protected Vector3 _targetPosition;
    protected Vector3 _initialPosition;
    public GameObject Book => _book;
    public Vector3 TargetPosition => _targetPosition;
    public Vector3 InitialPosition => _initialPosition;
    public Animator Animator => _animator;

    public virtual void Start()
    {
        if (_book == null) _book = gameObject;
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        if (_canvas == null) {
            Canvas canvas = GetComponentInChildren<Canvas>();
            if(canvas != null) _canvas = canvas.gameObject;
            else _canvas = new GameObject("Canvas");
        }
        if (_target == null) _target = transform;

        _initialPosition = transform.position;
        _targetPosition = _target.position;

        _canvas.SetActive(false);
    }

    public virtual void OnMovedToPosition() { }
    public virtual void OnMovedOutOfPosition() { }
}

