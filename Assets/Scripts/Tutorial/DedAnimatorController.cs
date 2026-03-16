using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DedAnimatorController : CustomerAnimatorController
{
    public override void StartWalking()
    {
        _animator.SetBool(IsWalking, true);
        StopSubIdleRoutine();
    }

    public override void StopWalking()
    {
        _animator.SetBool(IsWalking, false);
    }
    
    public override void ReachedCounter()
    {
        _animator.SetBool(IsWalking, false);
        StartSubIdleRoutine();
    }
    
    protected override IEnumerator SubIdleCoroutine()
    {
        while (true)
        {
            float delay = Random.Range(_subIdleDelayMin, _subIdleDelayMax);
            yield return new WaitForSeconds(delay);

            _animator.SetTrigger(PlaySubIdle);
        }
    }
}
