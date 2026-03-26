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
    [SerializeField] private Texture2D _give;

    [Header("Settings")]
    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private float _raycastDistance = 100f;

    private List<WindowPainter> _windowPainters;
    private bool _isHoveringDraw;
    
    private Camera _mainCamera;

    private CursorPriority _currentPriority = CursorPriority.None;

    private enum CursorPriority
    {
        None = 0,
        Draw = 1,
        PickMe = 2,
        Glove = 3,
        PickDough = 4,
        Drag = 5,
        Give = 6
    }
    
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
    
    private void OnDrawPointerEntered(WindowPainter window)
    {
        _isHoveringDraw = true;
    }
    
    private void OnDrawPointerExited(WindowPainter window)
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
        _currentPriority = CursorPriority.None;

        var draggingBake = GetDraggingBakeManager();
        if (draggingBake != null)
        {
            if (draggingBake.IsInReceptionArea)
            {
                _currentPriority = CursorPriority.Give;
                return;
            }

            _currentPriority = CursorPriority.Drag;
            return;
        }

        if (IsDoughDragging())
        {
            _currentPriority = CursorPriority.Drag;
            return;
        }
        
        if (IsRollingPinDragging())
        {
            _currentPriority = CursorPriority.Drag;
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, _raycastDistance, _raycastLayers))
        {
            GameObject hitObject = hit.collider.gameObject;

            Filling filling = hitObject.GetComponent<Filling>();
            if (filling != null)
            {
                if (filling.IsDragging)
                {
                    _currentPriority = CursorPriority.Drag;
                    return;
                }
                _currentPriority = CursorPriority.PickMe;
                return;
            }

            Tray tray = hitObject.GetComponent<Tray>();
            if (tray != null)
            {
                _currentPriority = CursorPriority.Glove;
                return;
            }

            DoughBucket doughBucket = hitObject.GetComponent<DoughBucket>();
            if (doughBucket != null)
            {
                _currentPriority = CursorPriority.PickDough;
                return;
            }

            DoughController doughController = hitObject.GetComponentInParent<DoughController>();
            if (doughController != null)
            {
                _currentPriority = CursorPriority.PickMe;
                return;
            }
            
            BakeManager bakeManager = hitObject.GetComponentInParent<BakeManager>();
            if (bakeManager != null)
            {
                _currentPriority = CursorPriority.PickDough;
                return;
            }

            RollingPin rollingPin = hitObject.GetComponent<RollingPin>();
            if (rollingPin != null)
            {
                _currentPriority = CursorPriority.PickMe;
                return;
            }
        }

        if (_isHoveringDraw && _currentPriority == CursorPriority.None)
        {
            _currentPriority = CursorPriority.Draw;
        }
    }

    private bool IsDoughDragging()
    {
        return _doughBucket != null && _doughBucket.CurrentDough != null && _doughBucket.CurrentDough.IsDragging;
    }
    
    private bool IsRollingPinDragging()
    {
        return _rollingPin != null && _rollingPin.IsDragging;
    }

    private BakeManager GetDraggingBakeManager()
    {
        return FindObjectsByType<BakeManager>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .FirstOrDefault(b => b.IsDragging);
    }
    
    private void UpdateCursor()
    {
        Texture2D cursorToSet = null;
        
        switch (_currentPriority)
        {
            case CursorPriority.Give:
                cursorToSet = _give;
                break;
            case CursorPriority.Drag:
                cursorToSet = _drag;
                break;
            case CursorPriority.Glove:
                cursorToSet = _glove;
                break;
            case CursorPriority.PickDough:
                cursorToSet = _pickDough;
                break;
            case CursorPriority.PickMe:
                cursorToSet = _pickMe;
                break;
            case CursorPriority.Draw:
                cursorToSet = _draw;
                break;
            default:
                cursorToSet = _normal;
                break;
        }
        
        if (cursorToSet != null)
        {
            Cursor.SetCursor(cursorToSet, new Vector2(0.8f, 0), CursorMode.Auto);
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