using System;
using System.Collections.Generic;
using JustDough;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(DoughVisualSwitcher))]
public class DoughController : MonoBehaviour
{
    [Header("Состояния теста")]
    [SerializeField] private DoughState _startState = DoughState.Raw;
    [SerializeField] private DoughVisualSwitcher _doughVisualSwitcher;
    [SerializeField] private FillingType _filling = FillingType.None;
    [SerializeField] private int _cuttingZonesLeft;

    private List<DoughDrag> _doughDrags;
    private int _comboClicksTotal;
    private int _comboClicksLeft;
    private Vector3 _rollEnterLocalPos;
    private Quaternion _rollRotation;
    private bool _isRollingInside;
    private bool _rollFromAlongSide;
    private bool _lastActionPerfect;
    private int _perfectActionCount;
    private int _imperfectActionCount;

    private bool _isDragging;

    public FillingType Filling => _filling;

    public event Action FillingChanged;
    public event Action StateChanged;
    public event Action ActionPerfected;
    public event Action DragStarted;
    public event Action DragEnded;

    public bool CanApplyAction { get; private set; } = true;
    public DoughState OldState { get; private set; }
    public DoughState State { get; private set; }
    public bool IsDragging => _isDragging;
    public bool LastActionPerfect => _lastActionPerfect;
    public int PerfectActionCount => _perfectActionCount;
    public int ImperfectActionCount => _imperfectActionCount;

    private void Awake()
    {
        if (_doughVisualSwitcher == null)
            _doughVisualSwitcher = GetComponent<DoughVisualSwitcher>();

        _doughDrags = new List<DoughDrag>(GetComponentsInChildren<DoughDrag>(true));
        
        State = _startState;
        OldState = State;
        _perfectActionCount = 0;
        _imperfectActionCount = 0;
    }

    private void Start()
    {
        ResetSpecialZones();
        StateChanged?.Invoke();
    }

    public void OnChildDragStarted()
    {
        _isDragging = true;
        DragStarted?.Invoke();
    }

    public void OnChildDragEnded()
    {
        _isDragging = false;
        DragEnded?.Invoke();
    }

    public void SetRollRotation(Quaternion rollRotation)
    {
        _rollRotation = rollRotation;
    }

    public void SetGlobalFilling(FillingType toSet)
    {
        if (_filling == toSet) return;

        _filling = toSet;
        FillingChanged?.Invoke();
    }

    public void SetCanDrag(bool can)
    {
        foreach (var doughDrag in _doughDrags)
            doughDrag.SetIsDragBlocked(!can);
    }
    
    public void SetCanActing(bool can) => CanApplyAction = can;

    public bool TryApplyAction(DoughCraftAction action, bool isClick = false, CraftZone craftZone = null, bool isPerfect = false)
    {
        if (CanApplyAction == false) return false;

        bool isComboZoneAction = craftZone != null && craftZone.IsComboZone;

        if (isComboZoneAction && isClick)
        {
            action = DoughCraftAction.ComboClick;

            _lastActionPerfect = isPerfect;

            if (isPerfect)
                _perfectActionCount++;
            else
                _imperfectActionCount++;

            if (isPerfect)
                _comboClicksLeft -= 2;
            else
                _comboClicksLeft -= 1;

            UpdateComboAnimation();

            bool comboComplete = _comboClicksLeft <= 0;

            print(
                $"[DoughController] ComboClick zone={craftZone.name}, " +
                $"perfect={isPerfect}, perfectTotal={_perfectActionCount}, " +
                $"imperfectTotal={_imperfectActionCount}, comboComplete={comboComplete}, " +
                $"state={State}, filling={_filling}"
            );

            if (comboComplete == false)
                return true;
        }

        if (DoughCraftTree.TryGetNext(State, action, out var next) == false)
        {
            print($"[DoughController] Для состояния {State} нет перехода по действию {action}");
            return false;
        }

        if (next.Equals(State))
            return false;

        OldState = State;
        State = next;
        _lastActionPerfect = isPerfect;

        if (_lastActionPerfect)
            ActionPerfected?.Invoke();

        bool isRollingAction = action is DoughCraftAction.Roll or DoughCraftAction.RollSheer;

        if (isComboZoneAction == false)
        {
            if (isPerfect)
                _perfectActionCount++;
            else if (isRollingAction == false)
                _imperfectActionCount++;
        }

        if (State is DoughState.Flat or DoughState.LongFlat)
            transform.rotation = _rollRotation;

        print(
            $"[DoughController] {OldState} --{action}--> {next}, " +
            $"perfectTotal={_perfectActionCount}, imperfectTotal={_imperfectActionCount}, " +
            $"state={State}, filling={_filling}"
        );

        ResetSpecialZones();

        StateChanged?.Invoke();

        return true;
    }

    public void SetState(DoughState doughState)
    {
        OldState = State;
        State = doughState;
        _lastActionPerfect = false;

        if (doughState == DoughState.Raw)
        {
            _perfectActionCount = 0;
            _imperfectActionCount = 0;
        }

        ResetSpecialZones();
        StateChanged?.Invoke();
    }

    private void UpdateComboAnimation()
    {
        if (_doughVisualSwitcher.Map[State].TryGetComponent(out Animator animator) == false) return;

        float progress = (_comboClicksTotal - _comboClicksLeft) / (_comboClicksTotal - 1f);
        animator.Play("Completion", 0, progress);
    }

    private void ResetSpecialZones()
    {
        ResetCombo();
        ResetCutting();
    }

    private void ResetCombo()
    {
        if (_doughVisualSwitcher == null)
            return;

        if (_doughVisualSwitcher.Map.TryGetValue(State, out GameObject go) == false || go == null)
            return;

        _comboClicksTotal = 0;

        foreach (PerfectComboZone zone in go.GetComponentsInChildren<PerfectComboZone>())
            _comboClicksTotal += 2;

        _comboClicksLeft = _comboClicksTotal;
        UpdateComboAnimation();
    }

    private void ResetCutting()
    {
        if (_doughVisualSwitcher == null)
            return;

        if (_doughVisualSwitcher.Map.TryGetValue(State, out GameObject go) == false || go == null)
            return;

        _cuttingZonesLeft = 0;

        foreach (CuttingZone cut in go.GetComponentsInChildren<CuttingZone>())
            _cuttingZonesLeft++;
    }

    public void Destroy()
    {
        OnChildDragEnded();
        Destroy(gameObject);
    }
    
    public void ProgressCutting()
    {
        _cuttingZonesLeft--;
        if (_cuttingZonesLeft <= 0)
            TryApplyAction(DoughCraftAction.FinishCutting);
    }
}