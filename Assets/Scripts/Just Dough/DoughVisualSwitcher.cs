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

        public DoughState State => _state;
        public GameObject Model => _model;
    }

    [SerializeField] private DoughController _controller;
    [SerializeField] private List<StateVisual> _visuals = new ();

    public readonly Dictionary<DoughState, GameObject> Map = new();

    private void Awake()
    {
        if (_controller == null)
            _controller = GetComponent<DoughController>();

        if (_controller == null)
        {
            Debug.LogError("[DoughVisualSwitcher] Не найден DoughCraftController на объекте", this);
            enabled = false;
            return;
        }

        Map.Clear();

        foreach (var stateVisual in _visuals)
        {
            if (stateVisual == null || stateVisual.Model == null)
                continue;

            if (Map.ContainsKey(stateVisual.State))
                continue;

            Map.Add(stateVisual.State, stateVisual.Model);
        }

        foreach (var model in Map.Values)
            model.SetActive(false);

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
            kvp.Value.SetActive(kvp.Key == _controller.State);
    }
}