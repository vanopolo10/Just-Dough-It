using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class CustomerAnimatorController : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsSad = Animator.StringToHash("IsSad");
    private static readonly int SadWalkID = Animator.StringToHash("SadWalkID");
    private static readonly int PlaySubIdle = Animator.StringToHash("PlaySubIdle");
    private static readonly int OpenDoor = Animator.StringToHash("OpenDoor");
    private static readonly int NahID = Animator.StringToHash("NahID");
    private static readonly int Nah = Animator.StringToHash("Nah");
    private static readonly int Accepted = Animator.StringToHash("Bulka");
    private static readonly int SubIdleID = Animator.StringToHash("SubIdleID");
    private static readonly int IdleID = Animator.StringToHash("IdleID");

    [SerializeField] private int _idleVariants = 3;
    [SerializeField] private int _walkVariants = 3;
    [SerializeField] private int _subIdleVariants = 2;
    [SerializeField] private int _nahVariants = 2;
    [SerializeField] private float _subIdleDelayMin = 1f;
    [SerializeField] private float _subIdleDelayMax = 4f;

    private Animator _animator;
    private Coroutine _subIdleRoutine;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void StartWalking()
    {
        _animator.SetBool(IsWalking, true);
        _animator.SetInteger(SadWalkID, Random.Range(0, _walkVariants));
        StopSubIdleRoutine();
    }

    public void StopWalking()
    {
        _animator.SetBool(IsWalking, false);
        _animator.SetInteger(IdleID, Random.Range(0, _idleVariants));
    }
    
    public void ReachedCounter()
    {
        _animator.SetBool(IsWalking, false);
        StartSubIdleRoutine();
    }
    
    public void StartLeaving()
    {
        _animator.SetBool(IsWalking, true);
        StopSubIdleRoutine();
    }

    public void TriggerOpenDoor()
    {
        _animator.SetTrigger(OpenDoor);
    }

    public void OnGreeting() { }

    public void OnQuestStarted() { }

    public void OnItemRejected()
    {
        _animator.SetInteger(NahID, Random.Range(0, _nahVariants));
        _animator.SetTrigger(Nah);
    }

    public void OnItemAccepted()
    {
        _animator.SetTrigger(Accepted);
    }

    public void OnQuestFinished()
    {
        OnItemAccepted();
        _animator.SetBool(IsSad, false);
    }

    private void StartSubIdleRoutine()
    {
        StopSubIdleRoutine();
        _subIdleRoutine = StartCoroutine(SubIdleCoroutine());
    }

    private void StopSubIdleRoutine()
    {
        if (_subIdleRoutine != null)
        {
            StopCoroutine(_subIdleRoutine);
            _subIdleRoutine = null;
        }
    }

    private IEnumerator SubIdleCoroutine()
    {
        while (true)
        {
            float delay = Random.Range(_subIdleDelayMin, _subIdleDelayMax);
            yield return new WaitForSeconds(delay);

            int subIdle = Random.Range(0, _subIdleVariants);
            _animator.SetInteger(SubIdleID, subIdle);
            _animator.SetTrigger(PlaySubIdle);
        }
    }
}
