using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("Info")] 
    [SerializeField] private DoughBucket _doughBucket;
    [SerializeField] private RollingPin _rollingPin;
    
    [Header("Textures")]
    [SerializeField] private Texture2D _normal;
    [SerializeField] private Texture2D _pickMe;
    [SerializeField] private Texture2D _drag;
    [SerializeField] private Texture2D _pickDough;
    [SerializeField] private Texture2D _draw;
    [SerializeField] private Texture2D _glove;

    [Header("Settings")]
    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private float _raycastDistance = 100f;

    private List<WindowPainter> _windowPainters;
    private bool _isHoveringDraw;
    
    private Camera _mainCamera;
    private bool _isHoveringInteractable;
    private bool _isDragging;
    private bool _isHoveringPickDough;
    private bool _isHoveringFilling;
    private bool _isHoveringTray;

    private void Awake()
    {
        _mainCamera = Camera.main;
        
        _windowPainters = FindObjectsByType<WindowPainter>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

        foreach (var windowPainter in _windowPainters)
        {
            windowPainter.PointerEntered += OnDrawPointerEntered;
            windowPainter.PointerExited += OnDrawPointerExited;
        }
    }
    
    private void OnDestroy()
    {
        foreach (var windowPainter in _windowPainters.Where(windowPainter => windowPainter != null))
        {
            windowPainter.PointerEntered -= OnDrawPointerEntered;
            windowPainter.PointerExited -= OnDrawPointerExited;
        }
    }
    
    private void OnDrawPointerEntered()
    {
        _isHoveringDraw = true;
    }
    
    private void OnDrawPointerExited()
    {
        _isHoveringDraw = false;
    }
    
    private void Update()
    {
        UpdateMouseStates();
        UpdateCursor();
    }
    
    private void UpdateMouseStates()
    {
        _isHoveringInteractable = false;
        _isDragging = false;
        _isHoveringPickDough = false;
        _isHoveringFilling = false;
        _isHoveringTray = false;

        bool isDoughDragging = IsDoughDragging();
        bool isRollingPinDragging = IsRollingPinDragging();
        
        if (isDoughDragging || isRollingPinDragging)
        {
            _isDragging = true;
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, _raycastDistance, _raycastLayers))
        {
            GameObject hitObject = hit.collider.gameObject;

            DoughController doughController = hitObject.GetComponentInParent<DoughController>();
            if (doughController != null)
            {
                _isHoveringInteractable = true;
            }

            RollingPin rollingPin = hitObject.GetComponent<RollingPin>();
            if (rollingPin != null)
            {
                _isHoveringInteractable = true;
            }

            Filling filling = hitObject.GetComponent<Filling>();
            if (filling != null)
            {
                _isHoveringFilling = true;
                if (filling.IsDragging)
                {
                    _isDragging = true;
                    return;
                }
            }

            Tray tray = hitObject.GetComponent<Tray>();
            if (tray != null)
            {
                _isHoveringTray = true;
            }

            DoughBucket doughBucket = hitObject.GetComponent<DoughBucket>();
            if (doughBucket != null)
            {
                _isHoveringPickDough = true;
            }

            BakeManager bakeManager = hitObject.GetComponent<BakeManager>();
            if (bakeManager != null && (bakeManager.IsInTray | bakeManager.IsInShelf))
            {
                _isHoveringPickDough = true;
            }
        }
    }
    
    private bool IsDoughDragging()
    {
        if (_doughBucket != null && _doughBucket.CurrentDough != null)
        {
            return _doughBucket.CurrentDough.IsDragging;
        }
        return false;
    }
    
    private bool IsRollingPinDragging()
    {
        return _rollingPin != null && _rollingPin.IsDragging;
    }
    
    private void UpdateCursor()
    {
        if (_isHoveringDraw && _draw != null)
        {
            Cursor.SetCursor(_draw, new Vector2(0.8f, 0), CursorMode.Auto);
        }
        else if (_isDragging && _drag != null)
        {
            Cursor.SetCursor(_drag, new Vector2(0.8f, 0), CursorMode.Auto);
        }
        else if (_isHoveringTray && _glove != null)
        {
            Cursor.SetCursor(_glove, new Vector2(0.8f, 0), CursorMode.Auto);
        }
        else if (_isHoveringPickDough && _pickDough != null)
        {
            Cursor.SetCursor(_pickDough, new Vector2(0.8f, 0), CursorMode.Auto);
        }
        else if ((_isHoveringInteractable || _isHoveringFilling) && _pickMe != null)
        {
            Cursor.SetCursor(_pickMe, new Vector2(0.8f, 0), CursorMode.Auto);
        }
        else if (_normal != null)
        {
            Cursor.SetCursor(_normal, new Vector2(0.8f, 0), CursorMode.Auto);
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || _mainCamera == null) return;
        
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * _raycastDistance);

        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _raycastLayers))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.05f);
        }
    }
    #endif
}