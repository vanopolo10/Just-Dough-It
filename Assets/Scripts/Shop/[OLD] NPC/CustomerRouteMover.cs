using System;
using System.Collections;
using UnityEngine;

public class CustomerRouteMover : MonoBehaviour
{
    [SerializeField] private Transform _doorPoint;
    [SerializeField] private Transform _doorLookAt;
    [SerializeField] private Transform _counterPoint;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private Transform _exitLookAt;

    [SerializeField] private float _moveSpeed = 1.6f;
    [SerializeField] private float _rotateSpeed = 8f;
    [SerializeField] private float _stopDistance = 0.05f;
    [SerializeField] private float _modelForwardOffsetY = 180f;

    private Transform _target;
    private Coroutine _routine;
    private bool _isDoorAnimationFinished;

    private CustomerAnimatorController _animatorController;

    public event Action ReachedCounter;
    public event Action LeftCafe;

    public void MoveIn(Transform target, CustomerAnimatorController animatorController, int sadWalkID)
    {
        StopCurrentRoutine();
        _target = target;
        _animatorController = animatorController;
        _isDoorAnimationFinished = false;

        _animatorController.DoorAnimationFinished += OnDoorAnimationFinished;

        _routine = StartCoroutine(EnterRoutine(sadWalkID));
    }

    public void MoveOut()
    {
        StopCurrentRoutine();
        _routine = StartCoroutine(ExitRoutine());
    }

    private IEnumerator EnterRoutine(int sadWalkID)
    {
        _animatorController.StartSad(sadWalkID);
        _animatorController.LeaveCounter();

        _animatorController.StartWalking();
        yield return MoveTo(_doorPoint.position);

        yield return FaceTo(_doorLookAt.position);

        _animatorController.TriggerOpenDoor();

        yield return MoveTo(_counterPoint.position);

        _animatorController.StopWalking();
        _animatorController.ReachedCounter();

        ReachedCounter?.Invoke();
    }

    private IEnumerator ExitRoutine()
    {
        _animatorController.LeaveCounter();
        _animatorController.StartLeaving();

        yield return FaceTo(_doorLookAt.position);

        _animatorController.StartWalking();
        yield return MoveTo(_doorPoint.position);

        _animatorController.TriggerOpenDoor();

        yield return MoveTo(_exitPoint.position);

        _animatorController.StopWalking();

        yield return FaceTo(_exitLookAt.position);

        LeftCafe?.Invoke();
        Cleanup();
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        while (Vector3.Distance(_target.position, destination) > _stopDistance)
        {
            Vector3 dir = (destination - _target.position).normalized;
            _target.position = Vector3.MoveTowards(_target.position, destination, _moveSpeed * Time.deltaTime);
            RotateTowards(dir);
            yield return null;
        }
    }

    private IEnumerator FaceTo(Vector3 lookAt)
    {
        Vector3 dir = (lookAt - _target.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0, _modelForwardOffsetY, 0);

        while (Quaternion.Angle(_target.rotation, targetRot) > 1f)
        {
            _target.rotation = Quaternion.Slerp(_target.rotation, targetRot, _rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void RotateTowards(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0, _modelForwardOffsetY, 0);
        _target.rotation = Quaternion.Slerp(_target.rotation, targetRot, _rotateSpeed * Time.deltaTime);
    }

    private void StopCurrentRoutine()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private void OnDoorAnimationFinished()
    {
        _isDoorAnimationFinished = true;
    }

    private void Cleanup()
    {
        if (_animatorController)
            _animatorController.DoorAnimationFinished -= OnDoorAnimationFinished;

        _animatorController = null;
    }
}
