using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class RollingPinAlt : MonoBehaviour
{
    [SerializeField] private float _raiseBy = 3f;
    [SerializeField] private float _rotationSmooth = 5f;
    [SerializeField] private float _heightSmooth = 10f;
    [SerializeField] private float _rollingThreshold = 0.0001f;
    [SerializeField] private float _floorEpsilon = 0.01f;

    private Vector3 _offset;
    private float _zCord;
    private Vector3 _lastWorldPos;

    private float _baseY;
    private float _desiredY;
    private bool _isDragging;
    private bool _isRolling;
    private bool _cursorHidden;

    private Quaternion _targetRotation;

    public bool IsRolling => _isRolling;

    private void Awake()
    {
        _baseY = transform.position.y;
        _desiredY = _baseY;
        _targetRotation = transform.rotation;
        _lastWorldPos = transform.position;
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
    
    private void OnMouseDown()
    {
        if (Camera.main == null)
            return;

        _zCord = Camera.main.WorldToScreenPoint(transform.position).z;
        _lastWorldPos = transform.position;

        _isDragging = true;
        _isRolling = false;
        _desiredY = _baseY + _raiseBy;
    }

    private void OnMouseDrag()
    {
        if (_isDragging == false || Camera.main == null)
            return;

        Vector3 rawTargetPos = Utils.GetMouseWorldPos(_zCord);
        rawTargetPos.y = transform.position.y;

        Vector3 move = rawTargetPos - _lastWorldPos;

        bool rightHeld = Input.GetMouseButton(1);

        if (rightHeld && _cursorHidden == false)
        {
            Cursor.visible = false;
            _cursorHidden = true;
        }
        else if (rightHeld == false && _cursorHidden)
        {
            Cursor.visible = true;
            _cursorHidden = false;
        }

        _desiredY = rightHeld ? _baseY : _baseY + _raiseBy;

        bool atFloor = Mathf.Abs(transform.position.y - _baseY) < _floorEpsilon;
        bool canStartRolling = _isDragging && rightHeld && atFloor && move.sqrMagnitude > _rollingThreshold;

        if (canStartRolling)
            _isRolling = true;
        if (rightHeld == false)
            _isRolling = false;

        Vector3 targetPos = rawTargetPos;

        if (_isRolling)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;
            else
                forward.Normalize();

            Vector3 moveAlong = Vector3.Project(move, forward);
            targetPos = _lastWorldPos + moveAlong;
        }
        else if (move.sqrMagnitude > 0.0001f)
        {
            Vector3 lookDir = new Vector3(move.x, 0f, move.z);

            if (lookDir.sqrMagnitude > 0.0001f)
            {
                lookDir.Normalize();

                Vector3 currentForward = transform.forward;
                currentForward.y = 0f;

                if (currentForward.sqrMagnitude < 0.0001f)
                    currentForward = Vector3.forward;
                else
                    currentForward.Normalize();

                if (Vector3.Dot(lookDir, currentForward) < 0f)
                    lookDir = -lookDir;

                _targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            }
        }

        transform.position = targetPos;
        _lastWorldPos = targetPos;
    }

    private void OnMouseUp()
    {
        _isDragging = false;
        _isRolling = false;
        _desiredY = _baseY;

        if (_cursorHidden)
        {
            Cursor.visible = true;
            _cursorHidden = false;
        }
    }
}
