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

    [SerializeField] private DoughCraftController _controller;
    [SerializeField] private List<StateVisual> _visuals = new ();

    public readonly Dictionary<DoughState, GameObject> Map = new();

    private void Awake()
    {
        if (_controller == null)
            _controller = GetComponent<DoughCraftController>();

        if (_controller == null)
        {
            Debug.LogError("[DoughVisualSwitcher] Не найден DoughCraftController на объекте", this);
            enabled = false;
            return;
        }

        Map.Clear();
        
        foreach (var stateVisual in _visuals.Where(stateVisual => stateVisual != null && stateVisual.Model != null))
        {
            if (Map.ContainsKey(stateVisual.State))
            {
                Debug.LogWarning($"[DoughVisualSwitcher] Дубликат состояния {stateVisual.State} в списке визуалов", this);
                continue;
            }

            Map.Add(stateVisual.State, stateVisual.Model);
        }

        foreach (var model in Map.Values)
            model.SetActive(false);
        
        _controller.StateChanged += OnStateChanged;
        OnStateChanged();
    }

    private void OnDisable()
    {
        if (_controller != null)
            _controller.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged()
    {
        Map[_controller.OldState].SetActive(false);
        Map[_controller.CurrentState].SetActive(true);
    }
}
