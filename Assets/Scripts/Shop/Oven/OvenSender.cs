using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class OvenSender : MonoBehaviour
{
    [SerializeField] private List<DoughState> _finalStates = new()
    {
        DoughState.SimplePie,
        DoughState.CoolPie,
        DoughState.HotDog
    };

    [SerializeField] private Image _image;
    [SerializeField] private Tray _tray;
    
    private BoxCollider _collider;
    private DoughController _currentDough;
    private DoughDrag _currentDoughDrag;

    public event Action DoughSent;

    private void Awake()
    {
        if (_image == null)
            _image = GetComponentInChildren<Image>();
        
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
        _image.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (Cafe.Instance != null)
            Cafe.Instance.DoughChanged += OnDoughChanged;

        OnDoughChanged();
    }

    private void OnDisable()
    {
        if (Cafe.Instance != null)
            Cafe.Instance.DoughChanged -= OnDoughChanged;

        DetachFromDough();
    }

    private void OnDoughChanged()
    {
        DetachFromDough();

        if (Cafe.Instance == null || Cafe.Instance.CurrentDough == null)
        {
            _image.gameObject.SetActive(false);
            return;
        }

        _currentDough = Cafe.Instance.CurrentDough;
        _currentDough.StateChanged += OnDoughStateChanged;

        _currentDoughDrag = _currentDough.GetComponent<DoughDrag>();
        if (_currentDoughDrag != null)
            _currentDoughDrag.DragEnded += OnDoughDragEnded;

        RefreshIcon();
    }

    private void DetachFromDough()
    {
        if (_currentDough != null)
        {
            _currentDough.StateChanged -= OnDoughStateChanged;
            _currentDough = null;
        }

        if (_currentDoughDrag != null)
        {
            _currentDoughDrag.DragEnded -= OnDoughDragEnded;
            _currentDoughDrag = null;
        }

        _image.gameObject.SetActive(false);
    }

    private void OnDoughStateChanged()
    {
        RefreshIcon();
    }

    private void RefreshIcon()
    {
        if (_currentDough == null)
        {
            _image.gameObject.SetActive(false);
            return;
        }

        if (_tray != null && _tray.IsFull)
        {
            _image.gameObject.SetActive(false);
            return;
        }

        _image.gameObject.SetActive(_finalStates.Contains(_currentDough.State));
    }

    private void OnDoughDragEnded()
    {
        if (_currentDough == null)
            return;

        if (_tray != null && _tray.IsFull)
            return;

        if (_finalStates.Contains(_currentDough.State) == false)
            return;

        if (_collider.bounds.Contains(_currentDough.transform.position) == false)
            return;

        DoughSent?.Invoke();
    }
}
