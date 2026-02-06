using System;
using System.Collections;
using UnityEngine;

public class Oven : MonoBehaviour
{
    private const int MaxFirePower = 100;

    [SerializeField, Tooltip("Сколько силы дает одно бревно")] 
    private int _woodPower = 20;
    [SerializeField, Tooltip("Сколько длится горение бревна на своем пике")] 
    private int _woodFullPowerTime = 20;
    [SerializeField, Tooltip("Скорость разгорания бревна")]
    private float _speedСoef = Math.Abs(2f);
    [SerializeField, Tooltip("В сколько раз оно затухает медленнее, чем разгорается")] 
    private float _fadeCoef = Math.Abs(2f);
    
    public event Action<int> FirePowerChanged;

    public int FirePower { get; private set; } = 0;

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
            yield return new WaitForSeconds(1 / _speedСoef);
        }

        yield return new WaitForSeconds(_woodFullPowerTime);
        yield return StartCoroutine(FadeWood());
    }
    
    private IEnumerator FadeWood()
    {
        for (int i = 0; i < _woodPower; i++)
        {
            FirePower = Math.Clamp(FirePower - 1, 0, MaxFirePower);
            FirePowerChanged?.Invoke(FirePower);
            yield return new WaitForSeconds( (1 / _speedСoef) * _fadeCoef );
        }
    }
}
