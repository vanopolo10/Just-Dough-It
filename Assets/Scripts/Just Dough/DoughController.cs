using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JustDough;

[DisallowMultipleComponent]
[RequireComponent(typeof(DoughVisualSwitcher),typeof(DoughDrag), typeof(DoughBakeManager))]
[RequireComponent(typeof(Collider))]
public class DoughController : MonoBehaviour
{
    [Header("Состояния теста")]
    [SerializeField] private DoughState _startState = DoughState.Raw;
    [SerializeField] private DoughVisualSwitcher _doughVisualSwitcher;

    private readonly Dictionary<CraftZone, bool> _comboZones = new();

    private bool _canRoll = true;
    
    private Vector3 _rollEnterLocalPos;
    private Quaternion _rollRotation;
    private bool _isRollingInside;
    private bool _rollFromAlongSide;

    public event Action StateChanged;

    public DoughState OldState { get; private set; }

    public DoughState State { get; private set; }

    private void Awake()
    {
        if (_doughVisualSwitcher == null)
            _doughVisualSwitcher = GetComponent<DoughVisualSwitcher>();

        State = _startState;
        OldState = State;
    }

    private void Start()
    {
        ResetBunCombo();
        StateChanged?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out RollingPin rollingPin) == false)
            return;

        if (rollingPin.IsRolling == false)
            return;

        if (_canRoll == false)
            return;

        _canRoll = false;
        _isRollingInside = true;
        
        _rollEnterLocalPos = transform.InverseTransformPoint(other.transform.position);
        
        float absEnterX = Mathf.Abs(_rollEnterLocalPos.x);
        float absEnterZ = Mathf.Abs(_rollEnterLocalPos.z);
        _rollFromAlongSide = absEnterZ >= absEnterX;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out RollingPin rollingPin) == false)
            return;

        if (rollingPin.IsRolling == false)
            return;

        if (_isRollingInside == false)
        {
            _canRoll = true;
            return;
        }

        _isRollingInside = false;
        _canRoll = true;
        _rollRotation = rollingPin.transform.rotation;

        Vector3 exitLocalPos = transform.InverseTransformPoint(other.transform.position);

        if (_rollFromAlongSide)
        {
            float enterSignZ = Mathf.Sign(_rollEnterLocalPos.z);
            float exitSignZ = Mathf.Sign(exitLocalPos.z);
            
            if (Mathf.Approximately(enterSignZ, 0f) || Mathf.Approximately(exitSignZ, 0f))
                return;

            if (Mathf.Approximately(enterSignZ, exitSignZ))
                return;

            ApplyAction(DoughCraftAction.Roll);
        }
        else
        {
            float enterSignX = Mathf.Sign(_rollEnterLocalPos.x);
            float exitSignX = Mathf.Sign(exitLocalPos.x);

            if (Mathf.Approximately(enterSignX, 0f) || Mathf.Approximately(exitSignX, 0f))
                return;

            if (Mathf.Approximately(enterSignX, exitSignX))
                return;

            ApplyAction(DoughCraftAction.RollSheer);
        }
    }

    public bool ApplyAction(DoughCraftAction action, CraftZone craftZone = null)
    {
        if (craftZone != null && craftZone.IsComboZone)
        {
            action = DoughCraftAction.ComboClick;
            _comboZones[craftZone] = true;

            if (_comboZones.Values.Any(b => b == false))
                return false;
        }

        if (DoughCraftTree.TryGetNext(State, action, out var next) == false)
        {
            Debug.Log($"[DoughCraftController] Для состояния {State} нет перехода по действию {action}");
            return false;
        }
        
        if (next.Equals(State))
            return false;
        
        OldState = State;
        State = next;

        if (State == DoughState.Flat || State == DoughState.LongFlat)
            transform.rotation = _rollRotation;

        Debug.Log($"[DoughCraftController] {OldState} --{action}--> {next}");

        ResetBunCombo();
        StateChanged?.Invoke();
        
        return true;
    }

    public void SetState(DoughState doughState)
    {
        OldState = State;
        State = doughState;
        ResetBunCombo();
        StateChanged?.Invoke();
    }

    private void ResetBunCombo()
    {
        _comboZones.Clear();

        if (_doughVisualSwitcher == null)
            return;

        if (_doughVisualSwitcher.Map.TryGetValue(State, out GameObject go) == false || go == null)
            return;

        foreach (CraftZone zone in go.GetComponentsInChildren<CraftZone>())
        {
            if (zone.IsComboZone)
                _comboZones.Add(zone, false);
        }
    }
}
