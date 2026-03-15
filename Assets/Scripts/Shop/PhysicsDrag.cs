using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsDrag : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private float _lerpSpeed = 10f;
    [SerializeField] private float _targetY = 1.3f;
    [SerializeField] private bool _freezeOnRelease;
    [SerializeField] private Vector3 _lockedOffset;

    private bool _isOverridden;
    private bool _isLocked;
    private bool _wasSetToKinematic = false;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private GameObject _LockPoint;

    private Rigidbody _rb;
    private Camera _mainCamera;

    public bool IsDragging { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
    }

    public void Override(Transform target, bool setToKinematic = false)
    {
        if (target == null) return;

        _isOverridden = true;
        _LockPoint = target.gameObject;

        if (setToKinematic)
        {
            _rb.isKinematic = true;
            _wasSetToKinematic = true;
        }
    }
    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        if(_isLocked) StopDragging();
    }

    public void CancelOverride()
    {
        _isOverridden = false;

        if(_wasSetToKinematic)
        {
            _rb.isKinematic = false;
            _wasSetToKinematic = false;
        }
    }
    public void TryStartDragging() { 
        if(Input.GetMouseButton(0)) StartDragging();
        else UpdateTargetFromMouse();
    }
    public void StartDragging()
    {
        if (IsDragging) return;

        print($"{name} started dragging");

        _rb.isKinematic = true;
        IsDragging = true;
    }

    private void StopDragging()
    {
        if (!IsDragging) return;

        print($"{name} stopped dragging");

        if (!_freezeOnRelease && !_wasSetToKinematic)
            _rb.isKinematic = false;

        IsDragging = false;
    }

    private void OnMouseDown()
    {
        if (!_isLocked) StartDragging();
    }

    private void OnMouseUp()
    {
        if (!_isLocked) StopDragging();
    }

    private void Update()
    {
        if (_isOverridden)
        {
            _targetPosition = _LockPoint.transform.position + _lockedOffset;
            _targetRotation = _LockPoint.transform.rotation;
        }
        if (IsDragging)
        {
            UpdateTargetFromMouse();
        }
        

        if(IsDragging || _isOverridden) ApplyTransform();
    }

    private void UpdateTargetFromMouse()
    {
        float depth = _mainCamera.WorldToScreenPoint(transform.position).z;

        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth)
        );

        mouseWorldPos.y = _targetY;
        _targetPosition = mouseWorldPos;

        _targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }

    private void ApplyTransform()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            _targetPosition,
            Time.deltaTime * _lerpSpeed
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            _targetRotation,
            Time.deltaTime * _lerpSpeed
        );
    }
}
