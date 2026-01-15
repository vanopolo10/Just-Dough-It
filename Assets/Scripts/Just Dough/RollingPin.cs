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
    private Vector3 _lookDir;

    private float _baseY;
    private float _desiredY;

    private bool _dragAllowed;
    private bool _isDragging;

    private Quaternion _targetRotation;

    public bool IsRolling { get; private set; }

    private void Awake()
    {
        _baseY = transform.position.y;
        _desiredY = _baseY;
        _targetRotation = transform.rotation;
    }

    private void OnEnable()
    {
        DragCancelService.CancelRequested += OnCancelRequested;
        _cameraController.DragAllowedChanged += OnDragAllowedChanged;
    }

    private void OnDisable()
    {
        DragCancelService.CancelRequested -= OnCancelRequested;
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
        if (!_dragAllowed)
            return;

        _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
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

        bool rightHeld = Input.GetMouseButton(1);

        if (rightHeld && !IsRolling)
            StartRolling();
        else if (!rightHeld && IsRolling)
            StopRolling();

        _desiredY = IsRolling ? _baseY : _baseY + _raiseBy;

        if (IsRolling && move.sqrMagnitude > 0.00001f)
            UpdateRotation(move);

        transform.position = targetPos;
    }

    private void UpdateRotation(Vector3 move)
    {
        _lookDir = new Vector3(move.x, 0f, move.z);

        if (_lookDir.sqrMagnitude < 0.0001f)
            return;

        _lookDir.Normalize();

        Vector3 currentForward = transform.forward;
        currentForward.y = 0f;

        if (currentForward.sqrMagnitude < 0.0001f)
            currentForward = Vector3.forward;
        else
            currentForward.Normalize();

        if (Vector3.Dot(_lookDir, currentForward) < 0f)
            _lookDir = -_lookDir;

        _targetRotation = Quaternion.LookRotation(_lookDir, Vector3.up);
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, _desiredY, Time.deltaTime * _heightSmooth);
        transform.position = pos;

        if (_isDragging && IsRolling)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                _targetRotation,
                Time.deltaTime * _rotationSmooth
            );
        }
    }

    private void StartRolling()
    {
        IsRolling = true;
        RollStarted.Invoke();
    }

    private void StopRolling()
    {
        IsRolling = false;
        RollEnded.Invoke();
    }

    private void CancelDrag()
    {
        if (!_isDragging && !IsRolling)
            return;

        StopRolling();
        _isDragging = false;
        _desiredY = _baseY;
    }

    private void OnCancelRequested()
    {
        CancelDrag();
    }
}
