using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DoughBucket : MonoBehaviour
{
    [SerializeField] private DoughController _doughPrefab;
    [SerializeField] private Vector3 _spawnPoint;

    private SphereCollider _collider;
    private DoughController _currentDough;
    private DoughDrag _currentDoughDrag;

    public DoughController CurrentDough => _currentDough;

    public event Action CurrentDoughChanged;
    public event Action<DoughState> DoughStateChanged;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    private void OnDisable()
    {
        SetDough(null);
    }

    private void OnMouseDown()
    {
        if (_currentDough != null)
            return;

        if (_doughPrefab == null)
        {
            Debug.LogWarning("[DoughBucket] Dough prefab is not assigned", this);
            return;
        }

        Vector3 spawnPos = _spawnPoint;

        DoughController instance = Instantiate(_doughPrefab, spawnPos, new Quaternion());
        SetDough(instance);
    }

    public void SetDough(DoughController dough)
    {
        if (_currentDough == dough)
            return;

        if (_currentDough != null)
            _currentDough.StateChanged -= OnDoughStateChangedInternal;

        if (_currentDoughDrag != null)
            _currentDoughDrag.DragEnded -= OnDoughDragEnded;

        _currentDough = dough;
        _currentDoughDrag = null;

        if (_currentDough != null)
        {
            _currentDough.StateChanged += OnDoughStateChangedInternal;

            _currentDoughDrag = _currentDough.GetComponent<DoughDrag>();
            if (_currentDoughDrag != null)
                _currentDoughDrag.DragEnded += OnDoughDragEnded;
        }

        CurrentDoughChanged?.Invoke();

        if (_currentDough != null)
            DoughStateChanged?.Invoke(_currentDough.State);
    }

    public void ClearDough()
    {
        SetDough(null);
    }

    private void OnDoughStateChangedInternal()
    {
        if (_currentDough == null)
            return;

        DoughStateChanged?.Invoke(_currentDough.State);
    }

    private void OnDoughDragEnded()
    {
        if (_currentDough == null)
            return;

        Vector3 pos = _currentDough.transform.position;
        Bounds bounds = _collider.bounds;

        bool inside =
            pos.x >= bounds.min.x && pos.x <= bounds.max.x &&
            pos.y >= bounds.min.y && pos.y <= bounds.max.y &&
            pos.z >= bounds.min.z && pos.z <= bounds.max.z;

        if (inside == false)
            return;

        Destroy(_currentDough.gameObject);
        SetDough(null);
    }
}
