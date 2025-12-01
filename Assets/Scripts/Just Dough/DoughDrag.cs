using System;
using UnityEngine;

public class DoughDrag : MonoBehaviour
{
    private Vector3 _offset;
    private float _zCord;
    private bool _bothHeld;
    private bool _isDragging;

    public bool IsDragging => _isDragging;

    public event Action DragStarted;
    public event Action DragEnded;

    private void OnMouseDrag()
    {
        bool isBothNow = Input.GetMouseButton(0) && Input.GetMouseButton(1);

        if (isBothNow == false)
        {
            if (_isDragging)
            {
                _isDragging = false;
                DragEnded?.Invoke();
            }

            _bothHeld = false;
            return;
        }

        if (_bothHeld == false)
        {
            _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
            _offset = transform.position - Utils.GetMouseWorldPos(_zCord);
            _bothHeld = true;
        }

        if (_isDragging == false)
        {
            _isDragging = true;
            DragStarted?.Invoke();
        }

        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord) + _offset;
        targetPos.y = transform.position.y;
        transform.position = targetPos;
    }

    private void OnMouseUp()
    {
        if (_isDragging)
        {
            _isDragging = false;
            DragEnded?.Invoke();
        }

        _bothHeld = false;
    }
}
