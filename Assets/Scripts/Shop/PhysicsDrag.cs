using Unity.VisualScripting;
using UnityEngine;

public class PhysicsDrag : MonoBehaviour
{
    // public GameObject debug;
    [SerializeField] private float _lerpSpeed = 10f, _targetY = 1.3f;
    private bool _isDragging = false, _isOverriden = false, _freezeOnRelease = false;
    private Vector3 _targetPosition = Vector3.zero;
    private Quaternion _targetRotation = Quaternion.identity;
    private Rigidbody rb;
    public bool IsDragging { get { return _isDragging; } }

    public void SetFreeze(bool b) {  _freezeOnRelease = b; }
    public bool CompareOverride(Transform other) 
    {
        return true;
        //DEPRECATED
        //if (!_isOverriden || other.position != _targetPosition || other.rotation != _targetRotation)
            //return false;
        //else 
            //return true;
    }
    public void Override(Transform target)
    {
        _isOverriden = true;
        _targetPosition = target.position;
        _targetRotation = target.rotation;
    }
    public void CancelOverride()
    {
        _isOverriden = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void StartDragging() {
        Debug.Log(name + "Started dragging");
        rb.isKinematic = true;

        _isDragging = true;
    }
    public void StopDragging() {
        Debug.Log(name + "Stopped dragging");
        if(!_freezeOnRelease) rb.isKinematic = false;

        _isDragging = false;
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
        
        if (_isDragging && !_isOverriden)
        {

            _targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
            _targetPosition.y = _targetY;

            Vector3 tmp = Vector3.zero;
            tmp.y = transform.rotation.eulerAngles.y;
            _targetRotation = Quaternion.Euler(tmp);
            // debug.transform.position = _targetPosition;
        }

        if (_isDragging)
        {
            if (!Input.GetMouseButton(0)) StopDragging();
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * _lerpSpeed);
        }
    }

}

