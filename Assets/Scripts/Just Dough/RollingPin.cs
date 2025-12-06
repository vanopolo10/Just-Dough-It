using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class RollingPin : MonoBehaviour
{
    [SerializeField] private float _raiseBy = 3f;
    [SerializeField] private float _rotationSmooth = 5f;
    [SerializeField] private float _heightSmooth = 10f;

    private float _zCord;
    private Vector3 _lookDir;

    private float _baseY;
    private float _desiredY;
    private bool _isDragging;
    [SerializeField] private bool _isRolling;
    private bool _dragBlocked;

    private Quaternion _targetRotation;

    public bool IsRolling => _isRolling;

    private void Awake()
    {
        _baseY = transform.position.y;
        _desiredY = _baseY;
        _targetRotation = transform.rotation;
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
        if (_isDragging == false && _isRolling == false)
            return;

        _isDragging = false;
        _isRolling = false;
        _desiredY = _baseY;
        _dragBlocked = true;
    }

    private void OnMouseDown()
    {
        if (_dragBlocked)
            return;

        _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;

        _isDragging = true;
        _desiredY = _baseY + _raiseBy;
    }

    private void OnMouseDrag()
    {
        if (_dragBlocked)
        {
            if (Input.GetMouseButton(0) == false)
                _dragBlocked = false;

            return;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord);
        targetPos.y = currentPos.y;

        Vector3 move = targetPos - currentPos;

        bool rightHeld = Input.GetMouseButton(1);
        _isRolling = _isDragging && rightHeld;
        _desiredY = _isRolling ? _baseY : _baseY + _raiseBy;

        if (_isRolling && move.sqrMagnitude > 0.00001f)
        {
            _lookDir = new Vector3(move.x, 0f, move.z);

            if (_lookDir.sqrMagnitude > 0.0001f)
            {
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
        }

        transform.position = targetPos;
    }

    private void OnMouseUp()
    {
        _isDragging = false;
        _isRolling = false;
        _desiredY = _baseY;
        _dragBlocked = false;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        float newY = Mathf.Lerp(pos.y, _desiredY, Time.deltaTime * _heightSmooth);
        pos.y = newY;
        transform.position = pos;

        if (_isDragging && _isRolling)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                _targetRotation,
                Time.deltaTime * _rotationSmooth
            );
        }
    }
}
