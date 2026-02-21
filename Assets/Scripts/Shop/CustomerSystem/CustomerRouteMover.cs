using System;
using System.Collections;
using UnityEngine;

public class CustomerRouteMover : MonoBehaviour
{
    private static readonly int OpenDoor = Animator.StringToHash("OpenDoor");
    private static readonly int CloseDoor = Animator.StringToHash("CloseDoor");

    [Header("Door")]
    [SerializeField] private Animator _doorAnimator;
    
    [Header("Targets")]
    [SerializeField] private Transform _doorPoint;
    [SerializeField] private Transform _doorLookAt;
    [SerializeField] private Transform _counterPoint;
    [SerializeField] private Transform _doorInside;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private Transform _exitLookAt;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 1.6f;
    [SerializeField] private float _rotateSpeed = 8f;
    [SerializeField] private float _stopDistance = 0.05f;
    [SerializeField] private float _modelForwardOffsetY = 180f;
    [SerializeField] private float _exitDelay = 5;

    private Transform _target;
    private Coroutine _routine;

    private CustomerAnimatorController _animatorController;
    private Customer _customer;

    public event Action ReachedCounter;
    public event Action LeftCafe;

    public void MoveIn(Transform target, CustomerAnimatorController animatorController)
    {
        StopCurrentRoutine();
        _target = target;
        _animatorController = animatorController;
        _customer = _animatorController.GetComponentInParent<Customer>();
        _customer.QuestCompleted += MoveOut;
        
        _routine = StartCoroutine(EnterRoutine());
    }

    public void MoveOut()
    {
        StopCurrentRoutine();
        _routine = StartCoroutine(ExitRoutine());
    }

    private IEnumerator EnterRoutine()
    {
        _animatorController.StartSad();

        _animatorController.StartWalking();
        yield return MoveTo(_doorPoint.position);

        yield return FaceTo(_doorLookAt.position);

        _animatorController.TriggerOpenDoor();
        _doorAnimator.SetTrigger(OpenDoor);

        yield return MoveTo(_counterPoint.position);

        _animatorController.StopWalking();
        _animatorController.ReachedCounter();

        ReachedCounter?.Invoke();
    }

    private IEnumerator ExitRoutine()
    {
        yield return new WaitForSeconds(_exitDelay);
        
        _animatorController.StartLeaving();

        yield return FaceTo(_doorLookAt.position);

        _animatorController.StartWalking();
        yield return MoveTo(_doorInside.position);

        _animatorController.TriggerOpenDoor();
        _doorAnimator.SetTrigger(CloseDoor);

        yield return MoveTo(_exitPoint.position);
        yield return FaceTo(_exitLookAt.position);
        yield return MoveTo(_exitLookAt.position);

        _customer.QuestCompleted -= MoveOut;
        LeftCafe?.Invoke();
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
}
