using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private List<CameraView> _views;
    [SerializeField] private float _transitionDuration = 0.7f;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private float _transitionTime;
    private bool _isTransition;

    public int ViewID { get; private set; }

    private void Start()
    {
        ViewID = Mathf.Clamp(ViewID, 0, _views.Count - 1);
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

        ViewID = ViewID == 0 ? _views.Count - 1 : ViewID - 1;
        BeginTransition();
    }

    private void OnRight()
    {
        if (_views.Count == 0)
            return;

        ViewID = (ViewID + 1) % _views.Count;
        BeginTransition();
    }
    private void OnBack()
    {
        if (_views.Count == 0)
            return;

        ViewID = (ViewID + 2) % _views.Count;
        BeginTransition();
    }

    private void BeginTransition()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        _targetPosition = _views[ViewID].Position;
        _targetRotation = _views[ViewID].Rotation;

        _transitionTime = 0f;
        _isTransition = true;
    }

    private void SnapToCurrentView()
    {
        if (_views.Count == 0)
            return;

        transform.position = _views[ViewID].Position;
        transform.rotation = _views[ViewID].Rotation;
    }

    [Serializable]
    private struct CameraView
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
