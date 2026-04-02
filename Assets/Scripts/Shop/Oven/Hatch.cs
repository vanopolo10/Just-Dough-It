using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(AudioSource))]
public class Hatch : MonoBehaviour
{
    [SerializeField] private AnimationCurve _openCurve;
    [SerializeField] private float _duration = 0.5f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _finalStateClip;

    private bool _canMove = true;
    private bool _isMoving;
    private Coroutine _animationCoroutine;
    private AudioSource _audioSource;

    public event Action Moved;
    public event Action<bool> StateChanged;

    public bool IsOpen { get; private set; }
    public float OpenPercentage { get; private set; } = 0;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        if (_isMoving || !_canMove)
            return;

        TogglePosition();
    }

    private void TogglePosition()
    {
        if (IsOpen)
            Close();
        else
            Open();
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
        _audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        _audioSource.Play();
        _isMoving = true;

        float startX = NormalizeAngle(transform.localEulerAngles.x);
        float endX = open ? 0f : 90f;

        float t = 0f;

        while (t < _duration)
        {
            t += Time.deltaTime;
            float normalized = t / _duration;

            float curveValue = _openCurve.Evaluate(normalized);
            float x = Mathf.Lerp(startX, endX, curveValue);

            OpenPercentage = 1 - x / 90f;

            transform.localRotation = Quaternion.Euler(x, 0f, 0f);

            yield return null;
        }

        transform.localRotation = Quaternion.Euler(endX, 0f, 0f);

        IsOpen = open;
        _isMoving = false;
        _animationCoroutine = null;

        Moved?.Invoke();
        StateChanged?.Invoke(IsOpen); // ? ÊËÞ×ÅÂÎÅ

        _audioSource.Stop();
        _audioSource.PlayOneShot(_finalStateClip, 0.2f);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}