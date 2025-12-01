using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class RollingPin : MonoBehaviour
{
    [SerializeField] private float _raiseBy = 3f;
    [SerializeField] private float _rotationSmooth = 5f;
    [SerializeField] private float _heightSmooth = 10f;
    [SerializeField] private float _rollingThreshold = 0.0001f;

    private Vector3 _offset;
    private float _zCord;
    private Vector3 _lastWorldPos;

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
        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord) + _offset;
        targetPos.y = transform.position.y;

        Vector3 move = targetPos - _lastWorldPos;

        bool rightHeld = Input.GetMouseButton(1);
        _isRolling = _isDragging && rightHeld && move.sqrMagnitude > _rollingThreshold;
        _desiredY = rightHeld ? _baseY : _baseY + _raiseBy;
        
        if (_isRolling == false && move.sqrMagnitude > 0.0001f)
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
