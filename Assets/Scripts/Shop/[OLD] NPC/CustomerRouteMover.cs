using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CustomerRouteMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed    = 1.6f;
    [SerializeField] private float _rotateSpeed  = 8f;
    [SerializeField] private float _stopDistance = 0.05f;

    [Header("Маршруты")]
    [SerializeField] private Vector3[] _entryPoints;
    [SerializeField] private Vector3[] _exitPoints;

    [Header("Параметры аниматора")]
    [SerializeField] private string _isWalkingParam = "IsWalking";
    [SerializeField] private string _reachedParam   = "Reached";

    [SerializeField] private float _forwardRotationOffsetY = 180f;

    private Animator  _animator;
    private Coroutine _moveRoutine;

    private int  _isWalkingHash;
    private int  _reachedHash;
    private bool _hasIsWalkingParam;
    private bool _hasReachedBool;
    private bool _hasReachedTrigger;

    private bool _atCounter;

    public event Action<CustomerRouteMover> ReachedCounter;
    public event Action<CustomerRouteMover> LeftCafe;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        foreach (var p in _animator.parameters)
        {
            if (!string.IsNullOrEmpty(_isWalkingParam) &&
                p.name == _isWalkingParam &&
                p.type == AnimatorControllerParameterType.Bool)
            {
                _isWalkingHash     = Animator.StringToHash(_isWalkingParam);
                _hasIsWalkingParam = true;
            }

            if (!string.IsNullOrEmpty(_reachedParam) &&
                p.name == _reachedParam)
            {
                _reachedHash = Animator.StringToHash(_reachedParam);

                if (p.type == AnimatorControllerParameterType.Bool)
                    _hasReachedBool = true;
                else if (p.type == AnimatorControllerParameterType.Trigger)
                    _hasReachedTrigger = true;
            }
        }

        if (!_hasIsWalkingParam)
            Debug.LogWarning("[CustomerRouteMover] Bool parameter '" + _isWalkingParam + "' not found in Animator.");

        if (!_hasReachedBool && !_hasReachedTrigger)
            Debug.LogWarning("[CustomerRouteMover] Parameter '" + _reachedParam + "' (bool or trigger) not found in Animator.");
    }

    public void StartEntry()
    {
        if (_entryPoints == null || _entryPoints.Length == 0)
        {
            Debug.LogError("[CustomerRouteMover] Entry points not set.");
            return;
        }

        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(MovePath(_entryPoints, true));
    }

    public void StartExit()
    {
        if (_exitPoints == null || _exitPoints.Length == 0)
        {
            Debug.LogError("[CustomerRouteMover] Exit points not set.");
            return;
        }

        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(MovePath(_exitPoints, false));
    }

    private IEnumerator MovePath(Vector3[] points, bool notifyCounterAtEnd)
    {
        if (_hasIsWalkingParam)
            _animator.SetBool(_isWalkingHash, true);

        foreach (var target in points)
        {
            while (true)
            {
                Vector3 toTarget = target - transform.position;
                float   distance = toTarget.magnitude;

                if (distance <= _stopDistance)
                    break;

                Vector3 direction = toTarget.normalized;

                transform.position += direction * (_moveSpeed * Time.deltaTime);

                if (direction.sqrMagnitude > 0.0001f)
                {
                    Quaternion lookRot   = Quaternion.LookRotation(direction, Vector3.up);
                    Quaternion offsetRot = Quaternion.Euler(0f, _forwardRotationOffsetY, 0f);
                    Quaternion targetRot = lookRot * offsetRot;

                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRot,
                        _rotateSpeed * Time.deltaTime);
                }

                yield return null;
            }
        }

        if (_hasIsWalkingParam)
            _animator.SetBool(_isWalkingHash, false);

        _moveRoutine = null;

        if (notifyCounterAtEnd)
        {
            _atCounter = true;

            if (_hasReachedBool)
                _animator.SetBool(_reachedHash, _atCounter);
            else if (_hasReachedTrigger)
                _animator.SetTrigger(_reachedHash);

            ReachedCounter?.Invoke(this);
        }
        else
        {
            _atCounter = false;

            if (_hasReachedBool)
                _animator.SetBool(_reachedHash, _atCounter);

            LeftCafe?.Invoke(this);
        }
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
