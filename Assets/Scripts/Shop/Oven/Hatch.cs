using System.Collections;
using UnityEngine;

public class Hatch : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private AnimationCurve _openCurve;
    [SerializeField] private float _duration = 0.5f;

    private bool _isOpen;
    private Coroutine _animationCoroutine;

    private void OnEnable()
    {
        _cameraController.ViewChanged += OnViewChanged;
    }

    private void OnDisable()
    {
        _cameraController.ViewChanged -= OnViewChanged;
    }

    private void OnViewChanged(CameraController.CameraViewType cameraViewType)
    {
        if (cameraViewType == CameraController.CameraViewType.OvenDown)
            Open();
        else if (_isOpen)
            Close();
    }

    private void Open()
    {
        StartAnimation(true);
        _isOpen = true;
    }

    private void Close()
    {
        StartAnimation(false);
        _isOpen = false;
    }

    private void StartAnimation(bool open)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(Animate(open));
    }

    private IEnumerator Animate(bool open)
    {
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
        _animationCoroutine = null;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}