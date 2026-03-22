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
    [SerializeField] private Transform _doorOutside;
    [SerializeField] private Transform _reception;
    [SerializeField] private Transform _doorInside;
    [SerializeField] private Transform _exit;
    
    [Header("Timings")]
    [SerializeField] private float _exitDelay = 5f;

    private NpcMover _mover;
    private CustomerAnimatorController _animator;
    private Customer _customer;

    public event Action ReachedCounter;
    public event Action LeftCafe;

    public void CopyValues(CustomerRouteMover other = null, NpcMover mover = null) 
    {
        if (other == null) return;
        _doorAnimator = other._doorAnimator;
        _doorOutside = other._doorOutside;
        _reception = other._reception;
        _doorInside = other._doorInside;
        _exit = other._exit;
        _exitDelay = other._exitDelay;
        _mover = mover;
    }
    private void Awake()
    {
        if(_mover==null) _mover = GetComponent<NpcMover>();
    }

    public void Initialize(CustomerAnimatorController animator)
    {
        _animator = animator;
        _customer = animator.GetComponentInParent<Customer>();
        _customer.OnQuestCompleted += MoveOut;

        Debug.Log($"[CustomerRouteMover] Initialized for customer '{_customer.name}'");
        _mover.MoveRoutine(EnterRoutine());
    }

    private IEnumerator EnterRoutine()
    {
        Debug.Log($"[CustomerRouteMover] Routine entered for customer '{_customer.name}'");
        _animator.StartWalking();
        Debug.Log($"[CustomerRouteMover] '{_customer.name}' successfully started walking");

        yield return _mover.FaceTo(_customer.transform, _doorOutside.position);
        Debug.Log($"[CustomerRouteMover] '{_customer.name}' successfully faced to door");
        yield return _mover.MoveTo(_customer.transform, _doorOutside.position);
        Debug.Log($"[CustomerRouteMover] '{_customer.name}' successfully moved to door");

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