using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(BoxCollider))]
public class OvenSender : MonoBehaviour
{
    [SerializeField] private List<DoughState> _finalStates = new()
    {
        DoughState.Pirozhok,
        DoughState.CoolPirozhok,
        DoughState.HotDog
    };

    private Image _image;
    private BoxCollider _collider;
    private DoughController _currentDough;
    private DoughDrag _currentDoughDrag;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
        _image.enabled = false;
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
            _image.enabled = false;
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

        _image.enabled = false;
    }

    private void OnDoughStateChanged()
    {
        RefreshIcon();
    }

    private void RefreshIcon()
    {
        if (_currentDough == null)
        {
            _image.enabled = false;
            return;
        }

        _image.enabled = _finalStates.Contains(_currentDough.State);
    }

    private void OnDoughDragEnded()
    {
        if (_currentDough == null)
            return;

        if (_finalStates.Contains(_currentDough.State) == false)
            return;

        if (_collider.bounds.Contains(_currentDough.transform.position) == false)
            return;

        Debug.Log("[OvenSender] Dough sent to oven");
    }
}
