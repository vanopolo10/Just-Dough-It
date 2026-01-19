using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class RollingPin : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private float _raiseBy = 3f;
    [SerializeField] private float _rotationSmooth = 5f;
    [SerializeField] private float _heightSmooth = 10f;

    [Header("Events")]
    public UnityEvent DoughEntered = new();
    public UnityEvent DoughExited = new();
    public UnityEvent RollStarted = new();
    public UnityEvent RollEnded = new();

    private float _zCord;
    private float _baseY;
    private float _desiredY;

    private bool _dragAllowed = true;
    private bool _isDragging;

    private Quaternion _targetRotation;

    public bool IsRolling { get; private set; }

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
        if (_cam == null)
        {
            Debug.LogError("RollingPin: Camera with tag MainCamera not found.");
            enabled = false;
            return;
        }

        _baseY = transform.position.y;
        _desiredY = _baseY;
        _targetRotation = transform.rotation;
    }

    private void OnEnable()
    {
        DragCancelService.CancelRequested += CancelDrag;
        _cameraController.DragAllowedChanged += OnDragAllowedChanged;
    }

    private void OnDisable()
    {
        DragCancelService.CancelRequested -= CancelDrag;
        _cameraController.DragAllowedChanged -= OnDragAllowedChanged;
    }

    private void OnDragAllowedChanged(bool allowed)
    {
        _dragAllowed = allowed;
        
        if (!allowed)
            CancelDrag();
    }

    private void OnMouseDown()
    {
        if (!_dragAllowed) return;

        _zCord = _cam.WorldToScreenPoint(transform.position).z;
        _isDragging = true;
        _desiredY = _baseY + _raiseBy;
    }

    private void OnMouseUp()
    {
        CancelDrag();
    }

    private void OnMouseDrag()
    {
        if (!_dragAllowed || !_isDragging)
            return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord);
        targetPos.y = currentPos.y;
        Vector3 move = targetPos - currentPos;
        transform.position = targetPos;

        bool rightHeld = Input.GetMouseButton(1);

        if (rightHeld && !IsRolling)
            StartRolling();
        else if (!rightHeld && IsRolling)
            StopRolling();

        _desiredY = IsRolling ? _baseY : _baseY + _raiseBy;

        if (IsRolling && move.sqrMagnitude > 0.00001f)
            UpdateRotation(move);

        if (IsRolling)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                _targetRotation,
                Time.deltaTime * _rotationSmooth
            );
        }
    }

    private void UpdateRotation(Vector3 move)
    {
        Vector3 dir = new Vector3(move.x, 0f, move.z);
        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();

        Vector3 forward = transform.forward;
        forward.y = 0f;
        if (Vector3.Dot(dir, forward) < 0f)
            dir = -dir;

        _targetRotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, _desiredY, Time.deltaTime * _heightSmooth);
        transform.position = pos;
    }

    private void StartRolling()
    {
        IsRolling = true;
        RollStarted.Invoke();
    }

    private void StopRolling()
    {
        if (!IsRolling) return;

        IsRolling = false;
        RollEnded.Invoke();
    }

    private void CancelDrag()
    {
        if (!_isDragging)
            return;

        StopRolling();
        _isDragging = false;
        _desiredY = _baseY;
    }
}
