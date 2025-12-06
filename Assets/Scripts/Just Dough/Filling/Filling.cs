using System;
using UnityEngine;

public class Filling : MonoBehaviour
{
    private Vector3 _offset;
    private float _zCord;
    private bool _mouseHeld;
    private bool _isDragging;

    [SerializeField] private FillingType _type;
    [SerializeField] private GameObject splatter;

    public FillingType Type => _type;
    public bool IsDragging => _isDragging;

    public event Action DragStarted;
    public event Action DragEnded;

    private FillingManager _manager = null;
    private Vector3 _homePosition;
    private MeshRenderer _renderer;

    private void Start()
    {
        _homePosition = transform.position;
        _renderer = GetComponent<MeshRenderer>();
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Filling entered trigger");
        if (other.gameObject.TryGetComponent<FillingManager>(out _manager))
            Debug.Log("Filling area entered");
    }
    public void OnTriggerExit(Collider other)
    {
        _manager = null;
    }
    private void OnMouseDrag()
    {
        bool isHeldNow = Input.GetMouseButton(0);

        if (isHeldNow == false)
        {
            if (_isDragging)
            {
                _isDragging = false;
                DragEnded?.Invoke();
            }

            _mouseHeld = false;
            return;
        }

        if (_mouseHeld == false)
        {
            _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
            _offset = transform.position - Utils.GetMouseWorldPos(_zCord);
            _mouseHeld = true;
        }

        if (_isDragging == false)
        {
            _isDragging = true;
            DragStarted?.Invoke();
        }

        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord) + _offset;
        targetPos.y = transform.position.y;
        transform.position = targetPos;
        _renderer.enabled = true;
    }

    private void OnMouseUp()
    {
        if(_manager != null) 
            _manager.SetFilling(_type);

        _renderer.enabled = false;
        transform.position = _homePosition;
    }
}
