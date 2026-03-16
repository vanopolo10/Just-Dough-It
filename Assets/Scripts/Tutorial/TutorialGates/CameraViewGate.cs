using System;
using UnityEngine;

public class CameraViewGate : ITutorialGate
{
    private CameraController _camera;
    private CameraController.CameraViewType _target;

    public event Action Completed;

    public GameObject IconObject { get; }

    public CameraViewGate(CameraController camera, CameraController.CameraViewType target, GameObject icon = null)
    {
        _camera = camera;
        _target = target;
        IconObject = icon;
    }

    public void Enter()
    {
        _camera.ViewChanged += OnViewChanged;
    }

    private void OnViewChanged(CameraController.CameraViewType view)
    {
        if (view != _target)
            return;

        _camera.ViewChanged -= OnViewChanged;
        Completed?.Invoke();
    }
}