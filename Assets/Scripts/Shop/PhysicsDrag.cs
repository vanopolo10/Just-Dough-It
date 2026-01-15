using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsDrag : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private float _lerpSpeed = 10f;
    [SerializeField] private float _targetY = 1.3f;
    [SerializeField] private bool _freezeOnRelease;

    private bool _isOverridden;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;

    private Rigidbody _rb;
    private Camera _mainCamera;

    public bool IsDragging { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
    }

    public void Override(Transform target)
    {
        if (target == null) return;

        _isOverridden = true;
        _targetPosition = target.position;
        _targetRotation = target.rotation;
    }

    public void CancelOverride()
    {
        _isOverridden = false;
    }

    public void StartDragging()
    {
        if (IsDragging) return;

        Debug.Log($"{name} started dragging");

        _rb.isKinematic = true;
        IsDragging = true;
    }

    private void StopDragging()
    {
        if (!IsDragging) return;

        Debug.Log($"{name} stopped dragging");

        if (!_freezeOnRelease)
            _rb.isKinematic = false;

        IsDragging = false;
    }

    private void OnMouseDown()
    {
        StartDragging();
    }

    private void OnMouseUp()
    {
        StopDragging();
    }

    private void Update()
    {
        if (!IsDragging)
            return;

        if (!_isOverridden)
            UpdateTargetFromMouse();

        ApplyTransform();
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
