using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DoughVisualSwitcher : MonoBehaviour
{
    [Serializable]
    private class StateVisual
    {
        [SerializeField] private DoughState _state;
        [SerializeField] private GameObject _model;
        [SerializeField] private Vector3 _colliderCenter = Vector3.zero;
        [SerializeField] private Vector3 _colliderSize = Vector3.one;
        [SerializeField] private bool _useBoxCollider = true;

        public DoughState State => _state;
        public GameObject Model => _model;
        public Vector3 ColliderCenter => _colliderCenter;
        public Vector3 ColliderSize => _colliderSize;
        public bool UseBoxCollider => _useBoxCollider;

        public void AutoFillColliderFromMesh()
        {
            if (_model == null) return;

            MeshFilter meshFilter = _model.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) return;

            Bounds bounds = meshFilter.sharedMesh.bounds;
            _colliderCenter = bounds.center;

            Vector3 localScale = _model.transform.localScale;
            _colliderSize = new Vector3(
                bounds.size.x * localScale.x,
                bounds.size.y * localScale.y,
                bounds.size.z * localScale.z
            );
        }
    }

    [SerializeField] private DoughController _controller;
    [SerializeField] private List<StateVisual> _visuals = new();
    [SerializeField] private bool _updateColliderOnStateChange = true;
    [SerializeField] private bool _autoCalculateColliders = true;

    private Collider _currentCollider;
    private Dictionary<DoughState, StateVisual> _stateVisualMap = new();
    public readonly Dictionary<DoughState, GameObject> Map = new();

    private void Awake()
    {
        if (_controller == null)
            _controller = GetComponent<DoughController>();

        if (_controller == null)
        {
            enabled = false;
            return;
        }

        _currentCollider = GetComponent<Collider>();
        if (_currentCollider == null)
        {
            _updateColliderOnStateChange = false;
        }

        InitializeVisuals();
    }

    private void InitializeVisuals()
    {
        Map.Clear();
        _stateVisualMap.Clear();

        foreach (var stateVisual in _visuals)
        {
            if (stateVisual == null || stateVisual.Model == null)
                continue;

            if (Map.ContainsKey(stateVisual.State))
                continue;

            Map.Add(stateVisual.State, stateVisual.Model);
            _stateVisualMap.Add(stateVisual.State, stateVisual);

            if (_autoCalculateColliders && stateVisual.ColliderSize == Vector3.one)
            {
                stateVisual.AutoFillColliderFromMesh();
            }

            stateVisual.Model.SetActive(false);
        }

        _controller.StateChanged += OnStateChanged;
    }

    private void OnEnable()
    {
        if (_controller != null)
            OnStateChanged();
    }

    private void OnDisable()
    {
        if (_controller != null)
            _controller.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged()
    {
        foreach (var kvp in Map.Where(kvp => kvp.Value != null))
            kvp.Value.SetActive(false);

        DoughState currentState = _controller.State;
        if (Map.TryGetValue(currentState, out GameObject currentVisual) && currentVisual != null)
        {
            currentVisual.SetActive(true);

            if (_updateColliderOnStateChange && _currentCollider != null)
            {
                UpdateColliderForState(currentState);
            }
        }
    }

    private void UpdateColliderForState(DoughState state)
    {
        if (!_stateVisualMap.TryGetValue(state, out StateVisual stateVisual) || stateVisual == null)
        {
            return;
        }

        bool needBoxCollider = stateVisual.UseBoxCollider;
        bool isCurrentBox = _currentCollider is BoxCollider;

        if (needBoxCollider != isCurrentBox)
        {
            Destroy(_currentCollider);

            if (needBoxCollider)
            {
                _currentCollider = gameObject.AddComponent<BoxCollider>();
            }
            else
            {
                _currentCollider = gameObject.AddComponent<CapsuleCollider>();
            }

            _currentCollider.isTrigger = true;
        }

        if (needBoxCollider && _currentCollider is BoxCollider boxCollider)
        {
            boxCollider.center = stateVisual.ColliderCenter;
            boxCollider.size = stateVisual.ColliderSize;
        }
        else if (!needBoxCollider && _currentCollider is CapsuleCollider capsuleCollider)
        {
            capsuleCollider.center = stateVisual.ColliderCenter;

            Vector3 size = stateVisual.ColliderSize;
            capsuleCollider.radius = Mathf.Max(size.x, size.z) * 0.5f;
            capsuleCollider.height = size.y;

            if (size.x >= size.z)
                capsuleCollider.direction = 0;
            else if (size.y >= size.x && size.y >= size.z)
                capsuleCollider.direction = 1;
            else
                capsuleCollider.direction = 2;
        }
    }

    [ContextMenu("Auto Fill All Colliders")]
    public void AutoFillAllColliders()
    {
        foreach (var stateVisual in _visuals)
        {
            if (stateVisual != null)
            {
                stateVisual.AutoFillColliderFromMesh();
            }
        }

        if (_controller != null && _currentCollider != null)
        {
            UpdateColliderForState(_controller.State);
        }
    }
}