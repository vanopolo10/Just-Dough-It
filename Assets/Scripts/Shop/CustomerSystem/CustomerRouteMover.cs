using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NpcMover))]
public class CustomerRouteMover : MonoBehaviour
{
    private static readonly int OpenDoor = Animator.StringToHash("OpenDoor");
    private static readonly int CloseDoor = Animator.StringToHash("CloseDoor");

    [Header("Door")]
    [SerializeField] private Animator _doorAnimator;

    [Header("Points")] 
    [SerializeField] private Transform _door;
    [SerializeField] private Transform _reception;
    [SerializeField] private Transform _doorInside;
    [SerializeField] private Transform _doorOutside;
    [SerializeField] private Transform _exit;
    
    [Header("Timings")]
    [SerializeField] private float _exitDelay = 1.5f;

    private NpcMover _mover;
    private CustomerAnimatorController _animator;
    private Customer _customer;

    public event Action ReachedCounter;
    public event Action LeftCafe;

    private void Awake()
    {
        _mover = GetComponent<NpcMover>();
    }

    public void Initialize(CustomerAnimatorController animator)
    {
        _animator = animator;
        _customer = animator.GetComponentInParent<Customer>();
        _customer.OnQuestCompleted += MoveOut;

        _mover.MoveRoutine(EnterRoutine());
    }

    private IEnumerator EnterRoutine()
    {
        _animator.StartWalking();

        yield return _mover.FaceTo(_customer.transform, _door.position);
        yield return _mover.MoveTo(_customer.transform, _door.position);

        _animator.TriggerOpenDoor();
        _doorAnimator.SetTrigger(OpenDoor);
        yield return _mover.FaceTo(_customer.transform, _reception.position);
        yield return _mover.MoveTo(_customer.transform, _reception.position);

        _animator.StopWalking();
        _animator.ReachedCounter();

        ReachedCounter?.Invoke();
    }

    private void MoveOut()
    {
        _mover.MoveRoutine(ExitRoutine());
    }

    private IEnumerator ExitRoutine()
    {
        yield return new WaitForSeconds(_exitDelay);

        _animator.StartLeaving();
        yield return _mover.FaceTo(_customer.transform, _doorOutside.position);

        _animator.StartWalking();
        yield return _mover.MoveTo(_customer.transform, _doorInside.position);

        _animator.TriggerOpenDoor();
        _doorAnimator.SetTrigger(CloseDoor);
        
        yield return _mover.MoveTo(_customer.transform, _doorOutside.position);
        yield return _mover.FaceTo(_customer.transform, _exit.position);
        yield return _mover.MoveTo(_customer.transform, _exit.position);

        _customer.OnQuestCompleted -= MoveOut;

        _mover.StopAll();
        LeftCafe?.Invoke();
    }
}