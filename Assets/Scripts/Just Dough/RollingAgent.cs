using JustDough;
using Unity.Mathematics;
using UnityEngine;

public class RollingAgent : MonoBehaviour
{
    [SerializeField] private DoughCraftAction _action;
    [SerializeField] private Vector3 _finalScale;
    [SerializeField] private float _speedMultiplier = 50f, _speedCap = 0.03f, _completionThreshold = 0.8f;
    [SerializeField] private DoughState _initialState;
    private float _progress = 0f; // goes from 0 to 1
    private RollingPin _rollingPin;
    private DoughController _controller;
    private GameObject _dough;
    private Vector3 _appliedScale = Vector3.one;
    private Vector3 _recordedPosition;
    private bool _isRollingNow = false;
    
    

    private void Awake()
    {
        _dough = gameObject;
        while(_dough.CompareTag("Click_Dough") == false) 
            _dough = _dough.transform.parent.gameObject;

        _controller = _dough.GetComponentInParent<DoughController>();
    }

    private void ApplyScaling() { 
        Vector3 newScale = ((_finalScale - Vector3.one) * _progress) + Vector3.one;

        Vector3 doughScale = _dough.transform.localScale;
        doughScale.x = (doughScale.x / _appliedScale.x) * newScale.x;
        doughScale.y = (doughScale.y / _appliedScale.y) * newScale.y;
        doughScale.z = (doughScale.z / _appliedScale.z) * newScale.z;

        _dough.transform.localScale = doughScale;
        _appliedScale = newScale;
    }

    private void OnEnable()
    {
        _progress = 0f;
        ApplyScaling();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out _rollingPin) == false)
            return;

        if (_rollingPin.IsRolling == false)
            return;                         

        _recordedPosition = other.transform.position;
        _isRollingNow = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if(_isRollingNow == false) return;

        Vector3 pinMovement = _rollingPin.transform.position - _recordedPosition;
        _recordedPosition = _rollingPin.transform.position;
        if (pinMovement.magnitude < 0.0000001f) return;

        float addProgress = Vector3.Project(pinMovement, transform.forward).magnitude;
        addProgress = math.clamp(addProgress*_speedMultiplier*Time.deltaTime, 0f, _speedCap);

        _progress = math.clamp(_progress+addProgress, 0f, 1f);
        ApplyScaling();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out RollingPin rollingPin) == false)
            return;

        if (rollingPin.IsRolling == false)
            return;

        if (_isRollingNow == false)
            return;

        _isRollingNow = false;

        if (_progress >= _completionThreshold) 
            if(_controller.State == _initialState) 
                _controller.ApplyAction(_action);
    }
}
