using System;
using UnityEngine;

public class Filling : MonoBehaviour
{
    [SerializeField] private FillingType _type;

    private Vector3 _offset;
    private float _zCord;
    private bool _mouseHeld;
    private bool _isDragging;
    private bool _dragBlocked;
    
    private FillingManager _manager;
    private MeshRenderer _renderer;

    public event Action Destroyed;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }
    
    private void OnEnable()
    {
        DragCancelService.CancelRequested += OnCancelRequested;
    }

    private void OnDisable()
    {
        DragCancelService.CancelRequested -= OnCancelRequested;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Filling entered trigger");
        
        if (other.gameObject.TryGetComponent(out _manager))
            Debug.Log("Filling area entered");
    }
    
    public void OnTriggerExit(Collider other)
    {
        _manager = null;
    }
    
    private void OnMouseDrag()
    {
        if (_dragBlocked)
        {
            if (Input.GetMouseButton(0) == false && Input.GetMouseButton(1) == false)
                _dragBlocked = false;

            return;
        }
        
        bool isHeldNow = Input.GetMouseButton(0);

        if (isHeldNow == false)
        {
            _mouseHeld = false;
            _isDragging = false;
            return;
        }

        if (_mouseHeld == false)
        {
            _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
            _offset = transform.position - Utils.GetMouseWorldPos(_zCord);
            _mouseHeld = true;
            _isDragging = true;
        }

        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord) + _offset;
        targetPos.y = transform.position.y;
        transform.position = targetPos;

        if (_renderer != null)
            _renderer.enabled = true;
    }

    private void OnMouseUp()
    {
        _mouseHeld = false;
        _isDragging = false;

        if (_manager == null)
            return;
        
        _manager.SetFilling(_type);
        Destroyed?.Invoke();
        Destroy(gameObject);
    }
    
    private void OnCancelRequested()
    {
        if (_isDragging == false)
            return;

        _isDragging = false;
        _mouseHeld = false;
        _dragBlocked = true;
    }
}
