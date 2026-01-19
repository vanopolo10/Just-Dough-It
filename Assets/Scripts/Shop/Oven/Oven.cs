using System;
using System.Collections;
using UnityEngine;

public class Oven : MonoBehaviour
{
    private const int MaxFirePower = 100;

    [SerializeField] private int _woodFullPowerTime = 20;
    
    private int _woodPower = 20;
    
    public event Action<int> FirePowerChanged;
    
    public int FirePower {get; private set;}

    public void AddWood()
    {
        StartCoroutine(FireWood());
    }

    private IEnumerator FireWood()
    {
        for (int i = 0; i < _woodPower; i++)
        {
            FirePower = Math.Clamp(FirePower + 1, 0, MaxFirePower);
            FirePowerChanged?.Invoke(FirePower);
            yield return new WaitForSeconds(1);
        }
        
        StartCoroutine(WaitFullPower());
    }
    
    private IEnumerator WaitFullPower()
    {
        yield return new WaitForSeconds(_woodFullPowerTime);
        StartCoroutine(FadeWood());
    }
    
    private IEnumerator FadeWood()
    {
        for (int i = 0; i < _woodPower; i++)
        {
            FirePower = Math.Clamp(FirePower - 1, 0, MaxFirePower);
            FirePowerChanged?.Invoke(FirePower);
            yield return new WaitForSeconds(1);
        }
    }
}
