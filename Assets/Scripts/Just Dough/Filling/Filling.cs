using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Filling : MonoBehaviour
{
    [SerializeField] private FillingType _type;
    [SerializeField] float _downBy = 0f;
    
    private float _zCord;
    private bool _mouseHeld;
    private bool _dragBlocked;
    
    private FillingManager _manager;
    private MeshRenderer _renderer;
    private Vector3 _homePosition;
    
    public bool IsDragging { get; private set; }

    public event Action Destroyed;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _homePosition = transform.position;
    }
    
    private void OnEnable()
    {
        DragCancelService.CancelRequested += OnCancelRequested;
    }

    private void OnDisable()
    {
        DragCancelService.CancelRequested -= OnCancelRequested;
    }
    
    private void OnTriggerEnter(Collider other) =>
        other.gameObject.TryGetComponent(out _manager);
    
    private void OnTriggerExit(Collider other) =>
        _manager = null;

    private void OnMouseDown()
    {
        if (_dragBlocked)
            return;
        
        _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
        _mouseHeld = true;
        IsDragging = true;
    
        _renderer.enabled = true;
    }

    private void OnMouseDrag()
    {
        if (_dragBlocked)
        {
            if (Input.GetMouseButton(0) == false && Input.GetMouseButton(1) == false)
                _dragBlocked = false;

            return;
        }
    
        if (_mouseHeld == false)
            return;

        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord);

        Vector3 currentPos = transform.position;
        currentPos.y = _homePosition.y;

        if (_downBy > 0.1f)
            targetPos.y = _homePosition.y - (Mathf.Clamp(Vector2.Distance(_homePosition, currentPos), 0.1f, _downBy) - 0.1f);
        else
            targetPos.y = _homePosition.y;
        
        transform.position = targetPos;
    }

    private void OnMouseUp()
    {
        _mouseHeld = false;
        IsDragging = false;

        if (_manager != null)
            _manager.SetFilling(_type);

        Destroyed?.Invoke();
        _renderer.enabled = false;
        transform.position = _homePosition;
    }

    public void SetCanGrab(bool canGrab) => _dragBlocked = !canGrab;
    
    private void OnCancelRequested()
    {
        if (IsDragging == false)
            return;

        IsDragging = false;
        _mouseHeld = false;
        _dragBlocked = true;
    }
}