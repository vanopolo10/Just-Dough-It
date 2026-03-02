using UnityEngine;
using UnityEngine.Splines;

public class CuttingManager : MonoBehaviour
{
    [SerializeField] private Transform _lockPoint;
    [SerializeField] private GameObject _cutPoint;
    [SerializeField] private float _progressFalloffFactor = 3f;
    [SerializeField] private float _cuttingSpeed = 1f;
    [SerializeField] private bool _isAltCuttingZone = false;

    private SplineAnimate _splineAnimate;
    private Spline _spline;
    private Knife _knife;
    private bool _isCutting = false;
    private float _animationTime = 0f;

    public Transform GetLockPoint() => _lockPoint;
    public bool IsAltCuttingZone() => _isAltCuttingZone;

    private void Start()
    {
        _splineAnimate = _lockPoint.GetComponent<SplineAnimate>();
        _spline = GetComponentInChildren<SplineContainer>().Spline;
    }
    private void DisableCraftZones() { 
        //TODO: disable aa other craft zones on object
    }
    private void LockInputs() {
        //TODO: Lock player inputs
        GameObject parent = transform.parent.gameObject;
        foreach (CraftZone zone in parent.GetComponentsInChildren<CraftZone>())
        {
            if (zone != null) zone.gameObject.SetActive(false);
        }
    }
    public void StartCutting(Knife knife)
    {
        if (_isCutting) return;
        _knife = knife;

        LockInputs();
        DisableCraftZones();

        _isCutting = true;
        //_cutPoint.SetActive(false);
        _animationTime = 0f;
    }

    private void PerformCuttingCalculations() {
        //1) get mouse vector in 2d
        Vector2 mousePos = Input.mousePosition;
        Vector2 lockPointScreenPos = Camera.main.WorldToScreenPoint(_lockPoint.position);

        Vector2 mouseVector = (mousePos - lockPointScreenPos).normalized;

        //2) get spline tangent in 2d
        Vector3 splineTangent3D = _spline.EvaluateTangent(_animationTime);

        Vector2 splineTangent = new Vector2(splineTangent3D.x, splineTangent3D.z).normalized;
        Vector2 forwardDirection = new Vector2(_lockPoint.forward.x, _lockPoint.forward.z);
        Vector2 rightDirection = new Vector2(_lockPoint.right.x, _lockPoint.right.z);

        float angle = Vector2.Angle(splineTangent, forwardDirection);
        
        float dot = Vector2.Dot(rightDirection, splineTangent);
        Quaternion rotation;
        if (dot < 0) rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        else rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector2 rotatedTangent = (rotation * splineTangent).normalized;

        //debug
        //Debug.Log($"Mouse Vector: {mouseVector}, Spline Tangent: {splineTangent}, abs angle: {angle}, Rotated Tangent: {rotatedTangent}");

        //3) process projection
        Vector3 projection = Vector3.Project(mouseVector, rotatedTangent);
        float projectionMagnitude = projection.magnitude;
        float progress = Mathf.Pow(projectionMagnitude, _progressFalloffFactor);

        dot = Vector2.Dot(mouseVector, rotatedTangent);
        if(dot < 0 || progress < 0.001f) progress = 0;


        //4) progress animation based on projection result
        //сам ты нейронка, я просто комментирую код
        _animationTime += progress * Time.deltaTime * _cuttingSpeed;
        //Debug.Log($"Projection Magnitude: {projectionMagnitude}, Progress: {progress}, Animation Time: {_animationTime}");

        if(_animationTime >= 1f)
        {
            _animationTime = 1f;
            CompleteCut();
        }
        _splineAnimate.NormalizedTime = _animationTime;
        
    }

    private void CompleteCut() {
        _knife.FinishCutting();
        _isCutting = false;

        transform.parent.GetComponentInParent<DoughController>().ProgressCutting(_isAltCuttingZone);
    }

    private void Update()
    {
        if (_isCutting) PerformCuttingCalculations();
    }
}

