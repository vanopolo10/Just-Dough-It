using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private List<CameraView> _views;
    [SerializeField] private float _transitionDuration = 0.7f;

    private PlayerInput _playerInput;
    private int _viewID;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private float _transitionTime;
    private bool _isTransition;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _viewID = Mathf.Clamp(_viewID, 0, _views.Count - 1);
        SnapToCurrentView();
    }

    private void Update()
    {
        if (_isTransition == false)
            return;

        if (_transitionDuration <= 0f)
        {
            SnapToCurrentView();
            _isTransition = false;
            return;
        }

        _transitionTime += Time.deltaTime;
        float t = Mathf.Clamp01(_transitionTime / _transitionDuration);

        transform.position = Vector3.Lerp(_startPosition, _targetPosition, t);
        transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, t);

        if (t >= 1f)
            _isTransition = false;
    }

    private void OnLeft()
    {
        if (_views.Count == 0)
            return;

        _viewID = _viewID == 0 ? _views.Count - 1 : _viewID - 1;
        BeginTransition();
    }

    private void OnRight()
    {
        if (_views.Count == 0)
            return;

        _viewID = (_viewID + 1) % _views.Count;
        BeginTransition();
    }
    private void OnBack()
    {
        if (_views.Count == 0)
            return;

        _viewID = (_viewID + 2) % _views.Count;
        BeginTransition();
    }

    private void BeginTransition()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        _targetPosition = _views[_viewID].Position;
        _targetRotation = _views[_viewID].Rotation;

        _transitionTime = 0f;
        _isTransition = true;
    }

    private void SnapToCurrentView()
    {
        if (_views.Count == 0)
            return;

        transform.position = _views[_viewID].Position;
        transform.rotation = _views[_viewID].Rotation;
    }

    [Serializable]
    private struct CameraView
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
