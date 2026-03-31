using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hatch : MonoBehaviour
{
    [SerializeField] private AnimationCurve _openCurve;
    [SerializeField] private float _duration = 0.5f;

    private bool _canMove = true;
    private bool _isMoving;
    private Coroutine _animationCoroutine;

    public event Action Moved;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;
    }

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;
    }

    private void OnMouseDown()
    {
        if (_isMoving || !_canMove)
            return;

        TogglePosition();
    }

    private void Open()
    {
        if (_isMoving || IsOpen)
            return;

        StartAnimation(true);
    }

    private void Close()
    {
        if (_isMoving || !IsOpen)
            return;

        StartAnimation(false);
    }

    private void TogglePosition()
    {
        if (_isMoving || !_canMove)
            return;

        if (IsOpen)
            Close();
        else
            Open();
    }

    public void SetCanMove(bool can)
    {
        _canMove = can;
    }

    private void StartAnimation(bool open)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(Animate(open));
    }

    private IEnumerator Animate(bool open)
    {
        _isMoving = true;

        float startX = transform.localEulerAngles.x;
        startX = NormalizeAngle(startX);

        float endX = open ? 0f : 90f;

        float t = 0f;

        while (t < _duration)
        {
            t += Time.deltaTime;
            float normalized = t / _duration;

            float curveValue = _openCurve.Evaluate(normalized);
            float x = Mathf.Lerp(startX, endX, curveValue);

            transform.localRotation = Quaternion.Euler(x, 0f, 0f);

            yield return null;
        }

        transform.localRotation = Quaternion.Euler(endX, 0f, 0f);
        
        IsOpen = open;
        _isMoving = false;
        _animationCoroutine = null;
        Moved?.Invoke();
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}