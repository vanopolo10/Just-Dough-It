using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BakeManager : MonoBehaviour
{
    [Header("Время прожарки, сек")]
    [SerializeField] private float _rareInSeconds = 3f;
    [SerializeField] private float _doneInSeconds = 6f;
    [SerializeField] private float _burnStartInSeconds = 10f;
    [SerializeField] private float _burnFullInSeconds = 20f;

    [Header("Возврат на полку")]
    [SerializeField] private float _returnDuration = 0.25f;

    private float _timeInOven;
    private bool _isBaking;

    private bool _invokedRare;
    private bool _invokedDone;
    private bool _invokedBurn;

    private Tray _tray;
    private Shelf _shelf;

    private bool _isOnShelf;
    private Transform _shelfAnchor;

    private bool _isDragging;
    private bool _dragBlocked;
    private float _dragZ;
    private Vector3 _dragOffset;

    private int _perfectActionCount;

    public event Action Rare;
    public event Action Done;
    public event Action Burned;

    public event Action<float, float> VisualChanged;

    public BakeState BakeState { get; private set; } = BakeState.Raw;
    public int PerfectActionCount => _perfectActionCount;

    public float CurrentBakeBlend { get; private set; }
    public float CurrentBurnAmount { get; private set; }

    private void OnEnable()
    {
        DragCancelService.CancelRequested += OnCancelRequested;
    }

    private void OnDisable()
    {
        DragCancelService.CancelRequested -= OnCancelRequested;
    }

    private void Update()
    {
        if (_isBaking == false)
            return;

        _timeInOven += Time.deltaTime;
        UpdateBakeLogic();
    }

    private void UpdateBakeLogic()
    {
        float t = _timeInOven;

        float bakeT;
        float burnT;

        if (t <= _rareInSeconds)
        {
            bakeT = 0f;
            burnT = 0f;
            BakeState = BakeState.Raw;
        }
        else if (t <= _doneInSeconds)
        {
            bakeT = Mathf.InverseLerp(_rareInSeconds, _doneInSeconds, t);
            burnT = 0f;
            BakeState = BakeState.Rare;
        }
        else if (t <= _burnStartInSeconds)
        {
            bakeT = 1f;
            burnT = 0f;
            BakeState = BakeState.Done;
        }
        else
        {
            bakeT = 1f;
            burnT = Mathf.Clamp01(Mathf.InverseLerp(_burnStartInSeconds, _burnFullInSeconds, t));
            BakeState = BakeState.Burn;
        }

        CurrentBakeBlend = bakeT;
        CurrentBurnAmount = burnT;

        VisualChanged?.Invoke(bakeT, burnT);

        if (_invokedRare == false && t >= _rareInSeconds)
        {
            _invokedRare = true;
            Rare?.Invoke();
        }

        if (_invokedDone == false && t >= _doneInSeconds)
        {
            _invokedDone = true;
            Done?.Invoke();
        }

        if (_invokedBurn == false && t >= _burnStartInSeconds)
        {
            _invokedBurn = true;
            Burned?.Invoke();
        }
    }

    private void OnCancelRequested()
    {
        if (_isOnShelf == false || _isDragging == false)
            return;

        _isDragging = false;
        _dragBlocked = true;

        if (_shelfAnchor != null)
            StartCoroutine(ReturnToShelfRoutine(_shelfAnchor.position, _shelfAnchor.rotation));
    }

    public void Setup(Tray tray, Shelf shelf)
    {
        _tray = tray;
        _shelf = shelf;
        _isOnShelf = false;
        _shelfAnchor = null;
    }

    public void OnPlacedOnShelf(Transform anchor)
    {
        _isOnShelf = true;
        _shelfAnchor = anchor;
    }

    public void OnPlacedOnTray()
    {
        _isOnShelf = false;
        _shelfAnchor = null;
    }

    public void SetPerfectActionCount(int count)
    {
        _perfectActionCount = Mathf.Max(0, count);
    }

    public void BeginBake()
    {
        _isBaking = true;
    }

    public void StopBake()
    {
        _isBaking = false;
    }

    private void OnMouseDown()
    {
        if (_dragBlocked)
            return;

        if (_isOnShelf)
        {
            StartShelfDrag();
            return;
        }

        if (_tray == null || _shelf == null)
            return;

        if (_tray.IsInOven || _tray.IsMoving)
            return;

        if (BakeState == BakeState.Raw)
            return;

        if (_tray.TryTakeBun(this, out BakeManager taken) == false)
            return;

        taken.StopBake();
        _shelf.Place(taken);
        Debug.Log(_perfectActionCount);
    }

    private void OnMouseDrag()
    {
        if (_isOnShelf == false || _isDragging == false)
            return;

        if (_dragBlocked)
        {
            if (Input.GetMouseButton(0) == false)
                _dragBlocked = false;

            return;
        }

        Vector3 worldPos = GetMouseWorldPos();
        transform.position = worldPos + _dragOffset;
    }

    private void OnMouseUp()
    {
        if (_isOnShelf == false)
        {
            _dragBlocked = false;
            return;
        }

        if (_isDragging == false)
        {
            _dragBlocked = false;
            return;
        }

        _isDragging = false;
        _dragBlocked = false;

        if (_shelfAnchor != null)
            StartCoroutine(ReturnToShelfRoutine(_shelfAnchor.position, _shelfAnchor.rotation));
    }

    private void StartShelfDrag()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        _dragZ = screenPos.z;

        _dragOffset = transform.position - GetMouseWorldPos();
        _isDragging = true;
    }

    private Vector3 GetMouseWorldPos()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return transform.position;

        Vector3 mouse = Input.mousePosition;
        mouse.z = _dragZ;
        return cam.ScreenToWorldPoint(mouse);
    }

    private IEnumerator ReturnToShelfRoutine(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float time = 0f;

        while (time < _returnDuration)
        {
            float t = time / _returnDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}
