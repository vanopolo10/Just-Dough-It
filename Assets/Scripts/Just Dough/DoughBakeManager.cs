using System;
using System.Collections;
using UnityEngine;

public class DoughBakeManager : MonoBehaviour
{
    private const int BurnInSeconds = 60;
    private const int DoneInSeconds = 45;
    
    private int _secondsInOven;

    public event Action Done;
    public event Action Burn;

    public void BeginBake()
    {
        StartCoroutine(Bake());
    }

    public void StopBake()
    {
        StopCoroutine(Bake());
    }
    
    private IEnumerator Bake()
    {
        _secondsInOven++;
        CheckState();
        yield return new WaitForSeconds(1);
    }

    private void CheckState()
    {
        switch (_secondsInOven)
        {
            case DoneInSeconds:
                Done?.Invoke();
                break;
            case BurnInSeconds:
                Burn?.Invoke();
                break;
        }
    }
}
