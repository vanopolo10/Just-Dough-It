using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    [SerializeField] private List<PositionAndRotation> _positions;
    private int _selectedPosition = 0;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _positions[_selectedPosition].Position, 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, _positions[_selectedPosition].Rotation, 0.1f);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _positions[_selectedPosition].CameraFOV, 0.1f);
    }

    public void SetPosition(int newPosition)
    {
        _selectedPosition = newPosition;
    }

    [Serializable]
    private struct PositionAndRotation
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float CameraFOV;
    }
}
