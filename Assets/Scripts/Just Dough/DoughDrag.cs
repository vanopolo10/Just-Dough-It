using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoughDrag : MonoBehaviour
{
    [SerializeField] private float _collisionCheckRadius = 0.2f;
    [SerializeField] private float _bowlLiftHeight = 0.3f;
    [SerializeField] private float _liftSpeed = 10f;
    [SerializeField] private LayerMask _tableObjectLayer;
    [SerializeField] private LayerMask _cookingSurfaceLayer;
    [SerializeField] private string _doughBowlTag = "DoughBowl";

    private Vector3 _offset;
    private float _zCord;
    private bool _bothHeld;
    private bool _isDragging;
    private bool _dragBlocked;
    private float _baseHeight;
    private Camera _cam;

    public bool IsDragging => _isDragging;

    public event Action DragStarted;
    public event Action DragEnded;

    private void Awake()
    {
        _cam = Camera.main;
        if (_cam == null)
        {
            enabled = false;
            return;
        }

        _baseHeight = transform.position.y;
    }

    private void OnEnable()
    {
        DragCancelService.CancelRequested += OnCancelRequested;
    }

    private void OnDisable()
    {
        DragCancelService.CancelRequested -= OnCancelRequested;
    }

    private void OnCancelRequested()
    {
        if (_isDragging == false)
            return;

        _isDragging = false;
        _bothHeld = false;
        _dragBlocked = true;
        DragEnded?.Invoke();
    }

    private void OnMouseDrag()
    {
        if (_dragBlocked)
        {
            if (Input.GetMouseButton(0) == false && Input.GetMouseButton(1) == false)
                _dragBlocked = false;

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

        if (_bothHeld == false)
        {
            _zCord = _cam.WorldToScreenPoint(transform.position).z;
            _offset = transform.position - Utils.GetMouseWorldPos(_zCord);
            _bothHeld = true;
        }

        if (_isDragging == false)
        {
            _isDragging = true;
            DragStarted?.Invoke();
        }

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, _cookingSurfaceLayer)) return;

        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord) + _offset;
        targetPos.x = hit.point.x;
        targetPos.z = hit.point.z;

        bool isOverBowl = false;
        if (Physics.Raycast(ray, out hit, 100f, _tableObjectLayer))
            isOverBowl = hit.collider.CompareTag(_doughBowlTag);
        float targetY = isOverBowl ? _baseHeight + _bowlLiftHeight : _baseHeight;

        if (!isOverBowl && Physics.CheckSphere(new Vector3(targetPos.x, targetY, targetPos.z), _collisionCheckRadius, _tableObjectLayer, QueryTriggerInteraction.Ignore)) return;

        float currentY = transform.position.y;
        float newY = Mathf.Lerp(currentY, targetY, Time.deltaTime * _liftSpeed);

        transform.position = new Vector3(targetPos.x, newY, targetPos.z);
    }

    private void OnMouseUp()
    {
        if (_isDragging)
        {
            _isDragging = false;
            DragEnded?.Invoke();
        }

        _bothHeld = false;
        _dragBlocked = false;
    }
}