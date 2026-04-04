using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using static CameraController;

public class CuttingZone : MonoBehaviour
{
    [SerializeField] private Transform _splinePoint;
    [SerializeField] private Transform _knifePoint;
    [SerializeField] private GameObject _cutPoint;
    [SerializeField] private float _progressFalloffFactor = 3f;
    [SerializeField] private float _cuttingSpeed = 1f;
    [SerializeField] private List<GameObject> _destroyOnCut;
    [SerializeField] private List<GameObject> _detachOnCut;
    [SerializeField] private DoughDrag _drag;
    [SerializeField] private GameObject _guide;

    private static CameraViewType _activeView = CameraViewType.Craft;
    private CameraController _cameraController;

    private DoughController _controller;
    private SplineAnimate _splineAnimate;
    private Spline _spline;
    private Knife _knife;
    private bool _isCutting = false;
    private float _animationTime = 0f;

    private void Start()
    {
        if( ! _splinePoint.TryGetComponent<SplineAnimate>(out _splineAnimate) )
        { 
            _splineAnimate = _splinePoint.GetComponentInParent<SplineAnimate>();
        }
        _spline = GetComponentInChildren<SplineContainer>().Spline;
        _cameraController = Camera.main.GetComponent<CameraController>();
        _controller = transform.parent.GetComponentInParent<DoughController>();
        if(_drag==null) _drag = transform.parent.GetComponentInParent<DoughDrag>();
        Debug.Log($"CuttingZone {name} initialized, found components: " +
                  $"SplineAnimate: {_splineAnimate != null}, " +
                  $"Spline: {_spline != null}, " +
                  $"CameraController: {_cameraController != null}, " +
                  $"DoughController: {_controller != null}, " +
                  $"DoughDrag: {_drag != null}");
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

        foreach (RollingAgent agent in parent.GetComponentsInChildren<RollingAgent>())
        {
            if (agent != null) agent.gameObject.SetActive(false);
        }

        //no need to block fillings
    }
    public void StartCutting(Knife knife)
    {
        if (_isCutting) return;
        _knife = knife;
        knife.LockToPoint(_knifePoint);

        //LockInputs();
        //DisableCraftZones();
        if (_drag == null) _drag = transform.parent.GetComponentInParent<DoughDrag>();
        _drag.SetDragBlock(true);

        _isCutting = true;
        _cutPoint.SetActive(false);
        _animationTime = 0f;
    }

    private void PerformCuttingCalculations() {
        if (_cameraController.ViewType != _activeView) return; // could be optimised OPT

        //1) get mouse vector in 2d
        Vector2 mousePos = Input.mousePosition;
        Vector2 lockPointScreenPos = Camera.main.WorldToScreenPoint(_splinePoint.position);

        Vector2 mouseVector = (mousePos - lockPointScreenPos).normalized;

        //2) get spline tangent in 2d
        Vector3 splineTangent3D = _spline.EvaluateTangent(_animationTime);

        Vector2 splineTangent = new Vector2(splineTangent3D.x, splineTangent3D.z).normalized;
        Vector2 forwardDirection = new Vector2(_splinePoint.forward.x, _splinePoint.forward.z);
        Vector2 rightDirection = new Vector2(_splinePoint.right.x, _splinePoint.right.z);

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

        _controller.ProgressCutting();
        _drag.SetDragBlock(false);

        _guide.SetActive(false);
        HandleObjectLists();
    }

    private void HandleObjectLists()
    {
        foreach (GameObject obj in _destroyOnCut)
        {
            Destroy(obj);
        }

        foreach (GameObject obj in _detachOnCut)
        {
            obj.transform.parent = transform.root.parent;

            if (obj.TryGetComponent<ShrinkDespawner>(out ShrinkDespawner despawner)) {
                despawner.DespawnSelf();
            }
        }
    }

    private void Update()
    {
        if (_isCutting) PerformCuttingCalculations();
    }
}

