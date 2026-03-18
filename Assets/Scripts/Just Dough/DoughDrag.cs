using System;
using UnityEngine;

public class DoughDrag : MonoBehaviour
{
    private Vector3 _offset;
    private float _zCord;
    private bool _bothHeld;
    private bool _isDragging;
    private bool _dragBlockedCamera;
    private bool _dragBlocked;

    private RollingPin _rollingPin;

    public event Action DragStarted;
    public event Action DragEnded;

    private bool CanMove => _dragBlocked == false & _dragBlockedCamera == false;

    private void Awake()
    {
        _rollingPin = GameObject.FindGameObjectWithTag("RollingPin").GetComponent<RollingPin>();
        if (!_rollingPin) enabled = false;
    }

    private void OnEnable()
    {
        DragCancelService.CancelRequested += OnCancelRequested;
    }

    private void OnDisable()
    {
        DragCancelService.CancelRequested -= OnCancelRequested;
    }

    public void Block()
    {
        _dragBlocked = true;
    }

    public void Unblock()
    {
        _dragBlocked = false;
    }

    private void OnCancelRequested()
    {
        if (_isDragging == false)
            return;

        _isDragging = false;
        _bothHeld = false;
        _dragBlockedCamera = true;
        DragEnded?.Invoke();
    }

    private void FixedUpdate()
    {
        if (CanMove == false || _rollingPin.IsDragging)
        {
            if (Input.GetMouseButton(0) == false && Input.GetMouseButton(1) == false)
                _dragBlockedCamera = false;

            return;
        }

        bool isBothNow = Input.GetMouseButton(0) && Input.GetMouseButton(1);

        if (isBothNow == false)
        {
            if (_isDragging)
            {
                _isDragging = false;
                DragEnded?.Invoke();
            }

            _bothHeld = false;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (_bothHeld == false)
        {
            _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
            _offset = transform.position - Utils.GetMouseWorldPos(_zCord);
            if (Physics.Raycast(ray, 100f, LayerMask.GetMask("Dough")))
                _bothHeld = true;
        }

        if (_isDragging == false)
        {
            _isDragging = true;
            DragStarted?.Invoke();
        }

        if (Physics.Raycast(ray, 100f, LayerMask.GetMask("CookingSurface")))
        {
            Vector3 targetPos = Utils.GetMouseWorldPos(_zCord) + _offset;
            targetPos.y = transform.position.y;
            transform.position = targetPos;
        }
    }
}