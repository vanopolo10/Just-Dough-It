using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
public class RollingPin : MonoBehaviour
{
    [SerializeField] private float _raiseBy = 3f;
    [SerializeField] private float _rotationSmooth = 5f;
    [SerializeField] private float _heightSmooth = 10f;

    private Vector3 _offset;
    private float _zCord;
    private Vector3 _lastWorldPos;
    private Vector3 _lookDir;

    private float _baseY;
    private float _desiredY;
    private bool _isDragging;
    private bool _isRolling;

    private Quaternion _targetRotation;

    public bool IsRolling => _isRolling;

    private void Awake()
    {
        _baseY = transform.position.y;
        _desiredY = _baseY;
        _targetRotation = transform.rotation;
        _lastWorldPos = transform.position;
    }
    private void StartRolling() {
        _isRolling = true;

        Cursor.visible = false;
    }
    private void StopRolling()
    {
        _isRolling = false;

        Vector3 targetMousePos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        Mouse.current.WarpCursorPosition(new Vector2(targetMousePos.x, targetMousePos.y));
        _lastWorldPos = transform.position;

        Cursor.visible = true;
    }
    private void OnMouseDown()
    {
        _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
        _offset = transform.position - Utils.GetMouseWorldPos(_zCord);
        _lastWorldPos = transform.position;

        _isDragging = true;

        _desiredY = _baseY + _raiseBy;
    }

    private void OnMouseDrag()
    {
        if (Input.GetMouseButton(1) && !_isRolling)
            StartRolling();
        else if (!Input.GetMouseButton(1) && _isRolling)
            StopRolling();
        _desiredY = _isRolling ? _baseY : _baseY + _raiseBy;

        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord);
        targetPos.y = transform.position.y;

        Vector3 move = targetPos - _lastWorldPos;

        if (_isRolling == false && move.sqrMagnitude > 0.0001f)
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

        

        if (_isRolling == true && move.sqrMagnitude > 0.001f) 
        {
            targetPos = _lastWorldPos + Vector3.Project(move, _lookDir);
        }

        transform.position = targetPos;
        _lastWorldPos = targetPos;
    }

    private void OnMouseUp()
    {
        _isDragging = false;
        _desiredY = _baseY;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        float newY = Mathf.Lerp(pos.y, _desiredY, Time.deltaTime * _heightSmooth);
        pos.y = newY;
        transform.position = pos;
        
        if (_isDragging && Input.GetMouseButton(1) == false)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                _targetRotation,
                Time.deltaTime * _rotationSmooth
            );
        }
    }
}
