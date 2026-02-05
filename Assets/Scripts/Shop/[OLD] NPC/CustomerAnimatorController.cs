using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerAnimatorController : MonoBehaviour
{
    private static readonly int IsAtCounter = Animator.StringToHash("IsAtCounter");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsLeaving = Animator.StringToHash("IsLeaving");
    private static readonly int IsSad = Animator.StringToHash("IsSad");
    private static readonly int SadWalkID = Animator.StringToHash("SadWalkID");
    private static readonly int StartIdle = Animator.StringToHash("StartIdle");
    private static readonly int IsAtCount = Animator.StringToHash("IsAtCount");
    private static readonly int OpenDoor = Animator.StringToHash("OpenDoor");
    private static readonly int NahID = Animator.StringToHash("NahID");
    private static readonly int Nah = Animator.StringToHash("Nah");
    private static readonly int Bulka = Animator.StringToHash("Bulka");
    private static readonly int SubIdleID = Animator.StringToHash("SubIdleID");

    [Header("SubIdle settings")]
    [SerializeField] private int _subIdleVariants = 3;
    [SerializeField] private float _subIdleDelayMin = 1f;
    [SerializeField] private float _subIdleDelayMax = 4f;
    
    private Animator _animator;
    private Coroutine _subIdleRoutine;

    public event Action DoorAnimationFinished;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnStartWalking()
    {
        _animator.SetBool(IsWalking, true);
        _animator.SetBool(IsLeaving, false);
        StopSubIdleRoutine();
    }

    public void OnStopWalking()
    {
        _animator.SetBool(IsWalking, false);
    }

    public void OnStartSad(int sadWalkID)
    {
        _animator.SetBool(IsSad, true);
        _animator.SetInteger(SadWalkID, sadWalkID);
    }

    public void OnStopSad()
    {
        _animator.SetBool(IsSad, false);
    }

    public void OnReachedCounter()
    {
        _animator.SetBool(IsAtCounter, true);
        _animator.SetBool(IsWalking, false);

        StartSubIdleRoutine();
    }
    
    public void OnLeaveCounter()
    {
        _animator.SetBool(IsAtCount, false);
        StopSubIdleRoutine();
    }

    public void OnStartLeaving()
    {
        _animator.SetBool(IsLeaving, true);
        _animator.SetBool(IsWalking, true);
        StopSubIdleRoutine();
    }

    public void OnTriggerOpenDoor()
    {
        _animator.SetTrigger(OpenDoor);
    }
    public void OnGreeting()
    {
        // тут добавить анимацию приветствия 
        // после того как он дошёл до стола, но до того как он ушёл в айдл
    }
    public void OnQuestStarted() { 
        // по хорошему нужна анимация где он объясняет заказ (всё равно нужны будут анимации диалога)
    }
    public void OnItemRejected()
    {
        int variant = Random.Range(0, 2);

        _animator.SetInteger(NahID, variant);
        _animator.SetTrigger(Nah);
    }

    public void OnItemAccepted()
    {
        _animator.SetTrigger(Bulka);
        // тут нужно сделать так, чтобы после этого он возвращался в айдл, так как у нас может быть несколько булок в заказе
    }

    public void OnQuestFinished()
    { 
        OnItemAccepted();
        // тут добавить анимацию завершения
        // типо, может проигрывается последняя анимация приёма, начинает уходить, итд.
    }

    public void NotifyDoorAnimationFinished()
    {
        DoorAnimationFinished?.Invoke();
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
            float delay = UnityEngine.Random.Range(_subIdleDelayMin, _subIdleDelayMax);
            yield return new WaitForSeconds(delay);

            int subIdle = UnityEngine.Random.Range(0, _subIdleVariants);
            _animator.SetInteger(SubIdleID, subIdle);

            _animator.SetTrigger(StartIdle);
        }
    }
}
