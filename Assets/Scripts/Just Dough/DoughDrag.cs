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
    private DoughController _doughController;

    public event Action DragStarted;
    public event Action DragEnded;

    private bool CanMove => _dragBlocked == false && 
                            _dragBlockedCamera == false && 
                            _rollingPin.IsRolling == false;

    private void OnEnable()
    {
        _rollingPin = GameObject.FindGameObjectWithTag("RollingPin").GetComponent<RollingPin>();
        if (!_rollingPin) enabled = false;
        
        _doughController = GetComponentInParent<DoughController>();
        
        DragCancelService.CancelRequested += OnCancelRequested;
    }

    private void OnDisable()
    {
        DragCancelService.CancelRequested -= OnCancelRequested;
    }

    public void SetIsDragBlocked(bool isDragBlocked) => _dragBlocked = isDragBlocked;

    private void OnCancelRequested()
    {
        if (_isDragging == false)
            return;

        _isDragging = false;
        _bothHeld = false;
        _dragBlockedCamera = true;
        DragEnded?.Invoke();
        _doughController?.OnChildDragEnded();
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
                _doughController?.OnChildDragEnded();
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
            _doughController?.OnChildDragStarted();
        }

        if (Physics.Raycast(ray, 100f, LayerMask.GetMask("CookingSurface")))
        {
            Vector3 targetPos = Utils.GetMouseWorldPos(_zCord) + _offset;
            targetPos.y = transform.position.y;
            transform.position = targetPos;
        }
    }
}