using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CustomerAnimatorController : MonoBehaviour
{
    [SerializeField] private string _acceptTrigger = "Accept";
    [SerializeField] private string _declineTrigger = "Decline";
    [SerializeField] private string _reachedTrigger = "Reached";

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void MarkReachedCounter()
    {
        if (string.IsNullOrEmpty(_reachedTrigger))
            return;

        _animator.SetTrigger(_reachedTrigger);
    }

    public void PlayAccept()
    {
        _animator.ResetTrigger(_declineTrigger);
        _animator.SetTrigger(_acceptTrigger);
    }

    public void PlayDecline()
    {
        _animator.ResetTrigger(_acceptTrigger);
        _animator.SetTrigger(_declineTrigger);
    }
}